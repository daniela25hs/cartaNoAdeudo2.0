using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.TiposCartas;
using CartaNoAdeudoApi.Core.Entities.Bitacora;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Core.Options;
using CartaNoAdeudoApi.Core.Utils;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class CartaService(
        IUnitOfWork uow,
        IValidator<Datos> validator,
        IFirmaCartaService firmaCarta,
        ICertificadoFirmaService certificados,
        ICartaDocumentoService cartaDocumento,
        IEmailService email,
        INotificacionSapService sapNotificador,
        ITareaFirmaQueue tareaFirmaQueue,
        IOptions<TareaFirmaOptions> tareaFirmaOptions,
        ILogger<CartaService> logger) : ICartaService
    {
        private readonly TareaFirmaOptions _tareaFirmaOptions = tareaFirmaOptions.Value;

        public async Task<IReadOnlyList<TipoCartaResponse>> ListarTiposCartaAsync(CancellationToken ct = default)
        {
            var tipos = await uow.TiposCartas.GetActivosAsync(ct);
            return tipos.Select(t => new TipoCartaResponse(t.Id, t.Clave, t.Descripcion, t.Activo)).ToList();
        }

        public async Task<IReadOnlyList<SolicitudListItemResponse>> ListarSolicitudesAsync(EstatusSolicitud? estatus, CancellationToken ct = default)
        {
            var query = uow.Repository<Datos>().AsQueryable()
                .Include(d => d.TipoCarta)
                .Include(d => d.Tareas)
                .OrderByDescending(d => d.FechaHora)
                .AsQueryable();

            if (estatus is not null)
                query = query.Where(d => d.Estatus == estatus);

            var datos = await query.ToListAsync(ct);

            return datos.Select(d =>
            {
                var ultimoError = d.Tareas
                    .Where(t => t.MensajeError != null)
                    .OrderByDescending(t => t.FechaInicio)
                    .FirstOrDefault()?.MensajeError;

                return new SolicitudListItemResponse(
                    d.Id, d.Rfc, d.Nombre, d.TipoCarta!.Descripcion,
                    d.Folio, d.Estatus, d.Vencida, d.Intentos, d.FechaFirmado, d.FechaHora,
                    ultimoError);
            }).ToList();
        }

        public async Task<CartaAceptadaResponse> SolicitarCartaAsync(GenerarCartaRequest request, CancellationToken ct = default)
        {
            var tipoCarta = await uow.TiposCartas.GetByClaveAsync(request.TipoCarta, ct)
                ?? throw new NotFoundException(nameof(TiposCartas), request.TipoCarta);

            if (tipoCarta.Clave == "03" && string.IsNullOrWhiteSpace(request.LicAlcoholes))
                throw new ConflictException("El valor LicAlcoholes es obligatorio para el tipo de carta 03.");

            if (!DateOnly.TryParse(request.InicioVigencia, out var inicioVigencia))
                throw new ConflictException("InicioVigencia no tiene un formato de fecha válido.");

            if (await uow.Repository<Datos>().ExistsAsync(d =>
                    d.Rfc == request.Rfc && d.RO == request.Ro && d.IdTipoCarta == tipoCarta.Id &&
                    d.Estatus != EstatusSolicitud.Firmada, ct))
                throw new ConflictException("Ya existe una solicitud en trámite para ese RFC, RO y tipo de carta.");

            var dato = new Datos
            {
                IdTipoCarta = tipoCarta.Id,
                Nombre = request.Nombre,
                Rfc = request.Rfc,
                RO = request.Ro,
                Email = request.Email,
                InicioVigencia = inicioVigencia,
                // TODO: confirmar con negocio la vigencia real de cada tipo de carta; se asume 1 año
                // mientras no exista esa regla documentada.
                Vencimiento = inicioVigencia.AddYears(1),
                Alcoholes = request.LicAlcoholes,
                Estatus = EstatusSolicitud.SolicitudFirma
            };

            var validacion = await validator.ValidateAsync(dato, ct);
            if (!validacion.IsValid)
                throw new ConflictException(string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage)));

            // Transacción corta solo para el folio + el alta; la firma (llamada externa,
            // puede tardar/reintentar) se hace después, fuera de cualquier transacción abierta.
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                dato.Folio = await AsignarFolioAsync(tipoCarta.Clave, ct);
                await uow.Repository<Datos>().AddAsync(dato, ct);
                await uow.SaveAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }

            var resultado = await IntentarFirmarAsync(dato, tipoCarta, ct);

            if (resultado.RequiereReintento)
                await tareaFirmaQueue.EncolarAsync(new TareaFirmaItem(dato.Id, tipoCarta.Clave), ct);

            return new CartaAceptadaResponse(dato.Id, dato.Estatus);
        }

        public async Task<ValidarCartaResponse> ValidarCartaAsync(ValidarCartaRequest request, CancellationToken ct = default)
        {
            var folio = request.Folio.ToString();
            var dato = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.Firmas)
                .Include(d => d.TipoCarta)
                .FirstOrDefaultAsync(d => d.Rfc == request.Rfc && d.Folio == folio, ct);

            if (dato is null)
                return new ValidarCartaResponse(ResultadoValidacion.NoEncontrado, null, null, null, null);

            var noAutentica = new ValidarCartaResponse(ResultadoValidacion.NoAutentico, dato.Folio, null, null, null);

            if (dato.Estatus != EstatusSolicitud.Firmada)
                return noAutentica;

            var firma = dato.Firmas.OrderByDescending(f => f.Fecha).FirstOrDefault();
            if (firma is null || string.IsNullOrEmpty(firma.Certificado) ||
                string.IsNullOrEmpty(firma.CadenaOriginal) || string.IsNullOrEmpty(firma.Firma))
            {
                logger.LogWarning("La solicitud {IdDato} figura firmada pero no tiene firma verificable.", dato.Id);
                return noAutentica;
            }

            // Autentica = (1) la firma corresponde a la cadena guardada y al certificado guardado, y
            // (2) los datos actuales de la carta siguen produciendo esa misma cadena (no fueron alterados).
            // Ojo: si algún día cambia el formato de CadenaOriginalCarta, (2) fallará en cartas ya emitidas.
            var cadenaActual = CadenaOriginalCarta.Construir(dato, dato.TipoCarta!.Clave);
            if (cadenaActual != firma.CadenaOriginal ||
                !certificados.Verificar(firma.Certificado, firma.CadenaOriginal, firma.Firma))
            {
                logger.LogWarning("La firma de la solicitud {IdDato} no coincide con sus datos o con su certificado.", dato.Id);
                return noAutentica;
            }

            var certificado = certificados.LeerCertificadoPublico(firma.Certificado);
            var fechaFirma = dato.FechaFirmado ?? firma.Fecha;

            return new ValidarCartaResponse(
                ResultadoValidacion.Autentico,
                dato.Folio,
                firma.Descripcion,
                dato.FechaFirmado,
                fechaFirma >= certificado.VigenteDesde && fechaFirma <= certificado.VigenteHasta);
        }

        public async Task<byte[]> ObtenerReportePdfAsync(ValidarCartaRequest request, CancellationToken ct = default)
        {
            var folio = request.Folio.ToString();
            var dato = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.Archivo)
                .FirstOrDefaultAsync(d => d.Rfc == request.Rfc && d.Folio == folio, ct)
                ?? throw new NotFoundException(nameof(Datos), $"{request.Rfc}/{request.Folio}");

            if (dato.Archivo is null)
                throw new ConflictException("La carta aún no ha sido generada; la solicitud sigue en proceso de firma.");

            return Convert.FromBase64String(dato.Archivo.Carta);
        }

        /// <summary>
        /// Solo actúa sobre solicitudes en <c>EstatusSolicitud.ErrorDefinitivo</c> (agotaron sus
        /// <c>TareaFirmaOptions.MaxIntentos</c> reintentos automáticos): reinicia el contador y
        /// las reencola en <see cref="ITareaFirmaQueue"/> para que <c>TareaFirmaWorker</c> les dé
        /// otra vuelta completa. Las que siguen dentro de sus reintentos automáticos, o que ya
        /// fueron firmadas, no se tocan.
        /// </summary>
        public async Task<ReprocesoResultadoResponse> ReprocesarSolicitudesAsync(ReprocesarSolicitudesRequest request, CancellationToken ct = default)
        {
            var resultados = new List<ReprocesoItemResultado>();
            var encolados = 0;

            foreach (var solicitudId in request.SolicitudIds)
            {
                var dato = await uow.Repository<Datos>().AsQueryable()
                    .Include(d => d.TipoCarta)
                    .FirstOrDefaultAsync(d => d.Id == solicitudId, ct);

                if (dato is null)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false, "Solicitud no encontrada"));
                    continue;
                }

                if (dato.Estatus == EstatusSolicitud.Firmada)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false, "La solicitud ya fue firmada"));
                    continue;
                }

                if (dato.Estatus != EstatusSolicitud.ErrorDefinitivo)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false,
                        $"La solicitud sigue en sus reintentos automáticos ({dato.Intentos}/{_tareaFirmaOptions.MaxIntentos}); no hace falta reprocesarla a mano."));
                    continue;
                }

                dato.Intentos = 0;
                dato.Estatus = EstatusSolicitud.SolicitudFirma;
                uow.Repository<Datos>().Update(dato);
                await uow.SaveAsync(ct);

                await tareaFirmaQueue.EncolarAsync(new TareaFirmaItem(dato.Id, dato.TipoCarta!.Clave), ct);

                resultados.Add(new ReprocesoItemResultado(solicitudId, true, null));
                encolados++;
            }

            return new ReprocesoResultadoResponse(request.SolicitudIds.Count, encolados, resultados);
        }

        public async Task<ReprocesoResultadoResponse> ReprocesarTodosAsync(CancellationToken ct = default)
        {
            var ids = await uow.Repository<Datos>().AsQueryable()
                .Where(d => d.Estatus == EstatusSolicitud.ErrorDefinitivo)
                .Select(d => d.Id)
                .ToListAsync(ct);

            return await ReprocesarSolicitudesAsync(new ReprocesarSolicitudesRequest(ids), ct);
        }

        /// <summary>Carga la solicitud y le hace un intento de firma; lo usa <c>TareaFirmaWorker</c>
        /// para cada reintento en background. Idempotente: si ya está Firmada (por ejemplo, otro
        /// intento le ganó la carrera) no vuelve a firmar.</summary>
        public async Task<ResultadoIntentoFirma> ProcesarIntentoFirmaAsync(Guid idDato, CancellationToken ct = default)
        {
            var dato = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.TipoCarta)
                .FirstOrDefaultAsync(d => d.Id == idDato, ct)
                ?? throw new NotFoundException(nameof(Datos), idDato);

            if (dato.Estatus == EstatusSolicitud.Firmada)
                return new ResultadoIntentoFirma(true, false);

            return await IntentarFirmarAsync(dato, dato.TipoCarta!, ct);
        }

        /// <summary>
        /// Llama a <see cref="IFirmaCartaService"/> (firma local con el PFX del firmante activo),
        /// cuenta el intento en <c>Datos.Intentos</c> y registra el resultado en una <c>Tareas</c>
        /// nueva: si firma, crea la <c>Firmas</c> y marca <c>Datos</c> como Firmada; si falla y
        /// aún quedan intentos, deja <c>Datos.Estatus</c> en <c>SolicitudFirma</c> para que
        /// <c>TareaFirmaWorker</c> reintente; si ya no quedan, pasa a <c>ErrorDefinitivo</c>.
        /// </summary>
        private async Task<ResultadoIntentoFirma> IntentarFirmarAsync(Datos dato, TiposCartas tipoCarta, CancellationToken ct)
        {
            dato.Intentos++;

            var tarea = new Tareas { IdDato = dato.Id, FechaInicio = DateTimeOffset.UtcNow };

            var resultado = await firmaCarta.FirmarAsync(dato, tipoCarta.Clave, ct);

            tarea.FechaFin = DateTimeOffset.UtcNow;

            bool requiereReintento;
            string estadoNombre;

            if (resultado.Exitoso)
            {
                var firma = resultado.Firma!;
                await uow.Repository<Firmas>().AddAsync(new Firmas
                {
                    IdDato = dato.Id,
                    // ValidarCartaResponse.NombreServidorPublicoFirmante sale de aquí.
                    Descripcion = resultado.NombreFirmante,
                    Identificador = resultado.IdFirmante?.ToString(),
                    Certificado = firma.Certificado.CertificadoBase64,
                    HexSerie = firma.Certificado.NumeroSerieHex,
                    FingerPrint = firma.Certificado.Huella,
                    CadenaOriginal = resultado.CadenaOriginal,
                    Firma = firma.FirmaBase64
                }, ct);

                dato.Estatus = EstatusSolicitud.Firmada;
                dato.FechaFirmado = tarea.FechaFin;

                estadoNombre = EstadoTarea.Completada;
                tarea.IdEstado = (await ObtenerEstadoTareaAsync(estadoNombre, ct)).Id;
                tarea.Nota = "Carta firmada correctamente.";
                requiereReintento = false;
            }
            else if (dato.Intentos >= _tareaFirmaOptions.MaxIntentos)
            {
                dato.Estatus = EstatusSolicitud.ErrorDefinitivo;

                estadoNombre = EstadoTarea.ErrorDefinitivo;
                tarea.IdEstado = (await ObtenerEstadoTareaAsync(estadoNombre, ct)).Id;
                tarea.MensajeError = resultado.MensajeError;
                requiereReintento = false;

                logger.LogError("Solicitud {IdDato} agotó sus {MaxIntentos} intentos de firma. Último error: {Motivo}",
                    dato.Id, _tareaFirmaOptions.MaxIntentos, resultado.MensajeError);
            }
            else
            {
                estadoNombre = EstadoTarea.Error;
                tarea.IdEstado = (await ObtenerEstadoTareaAsync(estadoNombre, ct)).Id;
                tarea.MensajeError = resultado.MensajeError;
                requiereReintento = true;

                logger.LogWarning("Intento {Intentos}/{MaxIntentos} de firma fallido para {IdDato}: {Motivo}",
                    dato.Intentos, _tareaFirmaOptions.MaxIntentos, dato.Id, resultado.MensajeError);
            }

            uow.Repository<Datos>().Update(dato);
            await uow.Repository<Tareas>().AddAsync(tarea, ct);
            await uow.TareaHistorial.AddAsync(new TareaHistorial
            {
                IdTarea = tarea.Id,
                Estado = estadoNombre,
                Fecha = tarea.FechaFin ?? DateTimeOffset.UtcNow,
                Mensaje = tarea.MensajeError ?? tarea.Nota
            }, ct);
            await uow.SaveAsync(ct);

            if (resultado.Exitoso)
                await GenerarDocumentoYCorreoAsync(dato, tipoCarta, resultado, ct);

            return new ResultadoIntentoFirma(resultado.Exitoso, requiereReintento);
        }

        /// <summary>
        /// Genera el PDF final (RF-002/RF-003, layout .docx + datos de la solicitud ya
        /// combinados y convertidos con LibreOffice) y lo guarda en <c>Archivos</c>; después
        /// notifica por correo al solicitante, con el PDF adjunto si se pudo generar. Ninguna de
        /// las dos cosas puede tumbar la firma: ya quedó Firmada y persistida antes de llegar
        /// aquí, así que un error acá solo se registra — no hay reintento automático todavía
        /// para el PDF ni para el correo (a diferencia de la firma, que sí tiene
        /// <c>TareaFirmaWorker</c>).
        /// </summary>
        private async Task GenerarDocumentoYCorreoAsync(
            Datos dato, TiposCartas tipoCarta, ResultadoFirmaCarta resultado, CancellationToken ct)
        {
            byte[]? pdf = null;
            try
            {
                pdf = await cartaDocumento.GenerarPdfAsync(
                    dato, tipoCarta.Clave, tipoCarta.Descripcion, resultado.NombreFirmante, resultado.PuestoFirmante,
                    resultado.Firma?.FirmaBase64, ct);

                await uow.Repository<Archivos>().AddAsync(new Archivos { IdDato = dato.Id, Carta = Convert.ToBase64String(pdf) }, ct);
                await uow.SaveAsync(ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "No se pudo generar el PDF de la solicitud {IdDato}; queda Firmada pero sin documento.", dato.Id);
            }

            try
            {
                var adjuntos = pdf is null
                    ? null
                    : new List<ArchivoAdjunto> { new($"carta-{dato.Folio}.pdf", pdf, "application/pdf") };

                var imagenesEnlazadas = new List<ImagenEnlazada>
                {
                    new(EmailTemplates.EscudoContentId, EmailTemplates.EscudoBytes, EmailTemplates.EscudoContentType)
                };

                await email.EnviarAsync(
                    dato.Email,
                    $"Tu carta de no adeudo — folio {dato.Folio}",
                    EmailTemplates.BodyConstanciaEmitida(dato.FechaHora, pdf is not null),
                    adjuntos,
                    imagenesEnlazadas,
                    ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "No se pudo enviar el correo de la solicitud {IdDato} a {Email}.", dato.Id, dato.Email);
            }

            try
            {
                await sapNotificador.NotificarAsync(
                    new NotificacionSapPayload(dato.Id, dato.Rfc, dato.Folio, "Entregado a SAP"), ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "No se pudo notificar a SAP el resultado de la solicitud {IdDato}.", dato.Id);
            }
        }

        private async Task<EstadoTarea> ObtenerEstadoTareaAsync(string descripcion, CancellationToken ct) =>
            await uow.EstadosTarea.GetByDescripcionAsync(descripcion, ct)
                ?? throw new ServiceUnavailableException($"El catálogo de estados de tarea no tiene sembrado '{descripcion}'.");

        private async Task<string> AsignarFolioAsync(string tipoCartaClave, CancellationToken ct)
        {
            var folioConsecutivo = (await uow.Repository<FolioConsecutivo>()
                .FindAsync(f => f.TipoCarta == tipoCartaClave, ct)).SingleOrDefault();

            if (folioConsecutivo is null)
            {
                // Primer folio de este tipo: solo Add. Llamar Update sobre una entidad recién
                // agregada la pasa a Modified y EF intenta un UPDATE de una fila que no existe.
                folioConsecutivo = new FolioConsecutivo { TipoCarta = tipoCartaClave, UltimoFolio = 1 };
                await uow.Repository<FolioConsecutivo>().AddAsync(folioConsecutivo, ct);
            }
            else
            {
                folioConsecutivo.UltimoFolio++;
                uow.Repository<FolioConsecutivo>().Update(folioConsecutivo);
            }

            // Folio puramente numérico para que coincida con ValidarCartaRequest.Folio (int).
            // Nota: al ser consecutivo por tipo de carta, dos tipos distintos pueden compartir
            // el mismo número; ValidarCartaRequest no distingue por tipo (ver RF-005 legado).
            return folioConsecutivo.UltimoFolio.ToString();
        }
    }
}
