using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Auth
{
    /// <summary>
    /// Requisito que exige que el claim "appId" del JWT coincida con el AppKey de
    /// esta API en SIGA — evita que un token válido emitido para otra aplicación
    /// sea aceptado aquí. Se registra como política "SigaApp" en <c>AddSigaAuth</c>.
    /// </summary>
    public class SigaAppRequirement : IAuthorizationRequirement { }

    public class SigaAppHandler(IOptions<SigaOptions> options) : AuthorizationHandler<SigaAppRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, SigaAppRequirement requirement)
        {
            var appIdClaim = context.User.FindFirstValue("appId");
            if (!string.IsNullOrEmpty(appIdClaim) &&
                string.Equals(appIdClaim, options.Value.AppKey, StringComparison.OrdinalIgnoreCase))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
