namespace CartaNoAdeudoApi.Core.DTOs.Responses.TiposCartas
{
    public record TipoCartaResponse(
        int Id,
        string Clave,
        string Descripcion,
        bool Activo
    );
}
