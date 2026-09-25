namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    /// <summary>
    /// Valores de <c>Firmante.EstatusCertificado</c>, calculados al leer el PFX contra la
    /// vigencia del certificado. TODO: confirmar con negocio los valores reales del catálogo.
    /// </summary>
    public static class EstatusCertificadoFirmante
    {
        public const string Vigente = "Vigente";
        public const string Vencido = "Vencido";
        public const string PorIniciar = "PorIniciar";
    }
}
