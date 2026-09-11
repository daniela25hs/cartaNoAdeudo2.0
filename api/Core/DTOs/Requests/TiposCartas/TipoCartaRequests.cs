namespace CartaNoAdeudoApi.Core.DTOs.Requests.TiposCartas
{
    public record CrearTipoCartaRequest(
        string Clave,
        string Descripcion,
        bool Activo
    );

    public record ActualizarTipoCartaRequest(
        string Clave,
        string Descripcion,
        bool Activo
    );
}
