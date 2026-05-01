using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Servicios_Estudiantes.Api.Utilidades;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Interceptores
{
    /// <summary>
    /// Devuelve <see cref="Respuesta{T}"/> en JSON para 403 (usuario autenticado sin permiso).
    /// </summary>
    public sealed class ManejoAutorizacionRespuestaJson : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _predeterminado = new();

        public async Task HandleAsync(
            RequestDelegate siguiente,
            HttpContext contexto,
            AuthorizationPolicy? politica,
            PolicyAuthorizationResult resultado)
        {
            if (resultado.Succeeded)
            {
                await _predeterminado.HandleAsync(siguiente, contexto, politica!, resultado).ConfigureAwait(false);
                return;
            }

            bool autenticado = contexto.User.Identity?.IsAuthenticated == true;

            if (autenticado && resultado.Forbidden && !contexto.Response.HasStarted)
            {
                IEnumerable<string> detalles = resultado.AuthorizationFailure?.FailedRequirements?
                    .Select(r => r.GetType().Name) ?? Enumerable.Empty<string>();

                Respuesta<string?> cuerpo = Respuesta<string?>.Fail(
                    "No tiene permiso para acceder a este recurso.",
                    detalles);

                await JsonRespuestaEscritorio.EscribirAsync(
                    contexto.Response,
                    StatusCodes.Status403Forbidden,
                    cuerpo,
                    contexto.RequestAborted).ConfigureAwait(false);
                return;
            }

            await _predeterminado.HandleAsync(siguiente, contexto, politica!, resultado).ConfigureAwait(false);
        }
    }
}
