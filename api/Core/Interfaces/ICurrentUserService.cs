namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Datos del usuario autenticado en la request actual, extraídos del JWT que
    /// emite SIGA. Lo consume el interceptor de auditoría y cualquier servicio que
    /// necesite saber quién ejecuta la operación.
    ///
    /// Mientras la integración con SIGA esté comentada (ver Program.cs) no hay
    /// pipeline de autenticación, así que todas estas propiedades regresan null /
    /// listas vacías. Ver docs/INTEGRACION_SIGA.md para activarla.
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }                     // claim "sub"
        string? Email { get; }                    // claim "email"
        Guid? GrupoId { get; }                    // claim "grupo" (opcional)
        Guid? AppId { get; }                      // claim "appId" — debe coincidir con el appId de esta API registrado en SIGA
        IReadOnlyList<string> RolesSiga { get; }  // claim "role" — roles ADMINISTRATIVOS DE SIGA, no los de esta app
        bool IsAuthenticated { get; }
    }
}
