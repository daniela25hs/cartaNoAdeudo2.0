namespace CartaNoAdeudoApi.Core.DTOs.Requests.Configuracion
{
    public record ActualizarConfiguracionRequest(
        string Valor,
        string? Descripcion
    );
}
