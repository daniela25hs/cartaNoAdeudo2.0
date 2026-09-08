using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Auth
{
    /// <summary>
    /// Lee los claims del usuario autenticado desde el HttpContext. Los nombres de
    /// claim son los que emite el JWT de SIGA (ver docs/INTEGRACION_SIGA.md §1).
    /// Con la integración SIGA comentada, User es null y todo regresa vacío.
    /// </summary>
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public Guid? UserId =>
            Guid.TryParse(User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

        public string? Email => User?.FindFirstValue(JwtRegisteredClaimNames.Email);

        public Guid? GrupoId =>
            Guid.TryParse(User?.FindFirstValue("grupo"), out var g) ? g : null;

        public Guid? AppId =>
            Guid.TryParse(User?.FindFirst("appId")?.Value, out var a) ? a : null;

        public IReadOnlyList<string> RolesSiga =>
            User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    }
}
