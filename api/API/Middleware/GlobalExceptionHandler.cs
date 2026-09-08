using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.Exceptions;

namespace CartaNoAdeudoApi.API.Middleware
{
    /// <summary>
    /// Traduce excepciones a ProblemDetails. Toda regla de negocio debe lanzar
    /// una <see cref="BusinessException"/>; cualquier otra cosa cae en 500 y se
    /// registra como error (ver docs/GUIA_DESARROLLO.md §5).
    /// </summary>
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            var (statusCode, title) = exception switch
            {
                ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
                NotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
                DependencyException => (StatusCodes.Status409Conflict, "Dependencia"),
                ServiceUnavailableException => (StatusCodes.Status503ServiceUnavailable, "Servicio no disponible"),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Sin permisos"),
                _ => (0, (string?)null)
            };

            if (statusCode == 0)
            {
                logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);
                return false;
            }

            logger.LogWarning(
                "{ExceptionType} → {StatusCode} en {Method} {Path}: {Message}",
                exception.GetType().Name, statusCode,
                context.Request.Method, context.Request.Path, exception.Message);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message
            }, ct);

            return true;
        }
    }
}
