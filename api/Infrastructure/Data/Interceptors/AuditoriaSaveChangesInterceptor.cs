using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CartaNoAdeudoApi.Core.Entities;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Rellena los campos de <see cref="AuditableEntity"/> en cada SaveChanges.
    /// Se registra en <c>AddDatabase</c> y se engancha al DbContext.
    /// </summary>
    public class AuditoriaSaveChangesInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            AplicarAuditoria(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AplicarAuditoria(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AplicarAuditoria(DbContext? context)
        {
            if (context is null) return;

            var ahora = DateTimeOffset.UtcNow;
            var usuarioId = currentUser.UserId;

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.FechaCreo = ahora;
                        entry.Entity.IdCreo ??= usuarioId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.FechaEdito = ahora;
                        entry.Entity.IdEdito = usuarioId;
                        break;
                }
            }
        }
    }
}
