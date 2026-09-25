// Contratos alineados con CartaNoAdeudoApi.Core.DTOs.{Requests,Responses}.Cartas
// y CartaNoAdeudoApi.Core.Entities.Cartas.EstatusSolicitud.

export enum EstatusSolicitud {
	SolicitudFirma = 1,
	Firmada = 2,
	Pendiente = 3,
	ErrorDefinitivo = 4,
}

export const ESTATUS_SOLICITUD_LABELS: Record<EstatusSolicitud, string> = {
	[EstatusSolicitud.SolicitudFirma]: 'En proceso de firma',
	[EstatusSolicitud.Firmada]: 'Firmada',
	[EstatusSolicitud.Pendiente]: 'Pendiente',
	[EstatusSolicitud.ErrorDefinitivo]: 'Error al firmar',
};

export interface TipoCartaResponse {
	id: number;
	clave: string;
	descripcion: string;
	activo: boolean;
}

// El literal "03" es la carta de Alcoholes: LicAlcoholes se vuelve obligatorio
// para ese tipo (ver CartaService.SolicitarCartaAsync).
export const CLAVE_TIPO_CARTA_ALCOHOLES = '03';

export interface GenerarCartaRequest {
	rfc: string;
	nombre: string;
	ro: string;
	tipoCarta: string;
	inicioVigencia: string;
	licAlcoholes?: string | null;
	email: string;
}

export interface CartaAceptadaResponse {
	identificadorSolicitud: string;
	estatus: EstatusSolicitud;
}

export interface SolicitudListItemResponse {
	id: string;
	rfc: string;
	nombre: string;
	tipoCarta: string;
	folio: string | null;
	estatus: EstatusSolicitud;
	vencida: boolean;
	intentos: number;
	fechaFirmado: string | null;
	fechaHora: string;
	ultimoError: string | null;
}

export interface ReprocesoItemResultado {
	solicitudId: string;
	aceptado: boolean;
	motivo: string | null;
}
