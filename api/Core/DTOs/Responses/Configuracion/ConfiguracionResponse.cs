namespace CartaNoAdeudoApi.Core.DTOs.Responses.Configuracion
{
    public record ConfiguracionResponse(
        Guid Id,
        string Clave,
        string Valor,
        string? Descripcion,
        DateTimeOffset FechaModificacion
    );
}
