using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Servicios_Estudiantes.Api.Seguridad;
using Servicios_Estudiantes.Api.Utilidades;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Api.Configuraciones
{
    /// <summary>
    /// Une validación Bearer con la misma <see cref="FirmaSymmetricaJwt"/> que firma tokens.
    /// </summary>
    public sealed class ConfiguracionJwtBearerConFirma(FirmaSymmetricaJwt firma) : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly FirmaSymmetricaJwt _firma = firma;

        public void PostConfigure(string? nombre, JwtBearerOptions opciones)
        {
            if (nombre != JwtBearerDefaults.AuthenticationScheme) return;

            JwtOpciones c = _firma.Opciones;

            opciones.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = c.Emisor,
                ValidAudience = c.Audiencia,
                IssuerSigningKeys = [_firma.Clave],
                IssuerSigningKeyResolver = (_, _, _, _) => [_firma.Clave],
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name
            };

            opciones.Events = new JwtBearerEvents
            {
                OnChallenge = EventosJwtBearerResponderJson.OnChallenge,
                OnTokenValidated = async contexto =>
                {
                    string? jti = contexto.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                        ?? contexto.Principal?.FindFirst("jti")?.Value;
                    if (string.IsNullOrEmpty(jti)) return;

                    IJwtListaNegra lista = contexto.HttpContext.RequestServices.GetRequiredService<IJwtListaNegra>();
                    if (await lista.EstaRevocadoAsync(jti, contexto.HttpContext.RequestAborted).ConfigureAwait(false))
                        contexto.Fail("Token de acceso revocado.");
                }
            };
        }
    }

    internal static class EventosJwtBearerResponderJson
    {
        public static async Task OnChallenge(JwtBearerChallengeContext contexto)
        {
            if (contexto.Response.HasStarted)
                return;

            contexto.HandleResponse();

            string tecnico = contexto.AuthenticateFailure?.Message
                ?? contexto.ErrorDescription
                ?? contexto.Error
                ?? string.Empty;

            string cliente = JsonRespuestaEscritorio.MensajeFirmaJwt(tecnico);
            string mensaje = string.IsNullOrWhiteSpace(cliente) ? "No autorizado." : cliente;

            Respuesta<string?> fallo = Respuesta<string?>.Fail(mensaje,
                string.IsNullOrWhiteSpace(tecnico) ? [] : [tecnico]);

            await JsonRespuestaEscritorio.EscribirAsync(
                contexto.Response,
                StatusCodes.Status401Unauthorized,
                fallo,
                contexto.HttpContext.RequestAborted).ConfigureAwait(false);
        }
    }
}
