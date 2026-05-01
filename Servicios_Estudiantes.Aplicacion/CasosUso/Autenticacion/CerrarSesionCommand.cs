using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Aplicacion.Utilidades;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion
{
    /// <summary>
    /// Revoca el token de acceso actual (lista negra por jti) y, si se indica, el refresh enviado.
    /// </summary>
    public sealed record CerrarSesionCommand(string? EncabezadoAutorizacionBearer, string? TokenRenovacionOpcional) : IRequest<Respuesta<bool>>;

    /// <summary>
    /// Manejador para cerrar sesión: revoca el access token por jti y el refresh si se proporciona.
    /// </summary>
    public sealed class CerrarSesionCommandHandler(IRepositorioUsuarios repositorio, IJwtListaNegra listaNegra)
        : IRequestHandler<CerrarSesionCommand, Respuesta<bool>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;
        private readonly IJwtListaNegra _listaNegra = listaNegra;

        /// <summary>
        /// Maneja la revocación de tokens según la solicitud.
        /// </summary>
        public async Task<Respuesta<bool>> Handle(CerrarSesionCommand solicitud, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(solicitud.EncabezadoAutorizacionBearer))
            {
                string? token = ExtraerBearer(solicitud.EncabezadoAutorizacionBearer);
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                        string? jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                        DateTime exp = jwt.ValidTo;
                        if (!string.IsNullOrEmpty(jti))
                            await _listaNegra.RegistrarRevocacionAsync(jti, exp, cancellationToken).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Token ilegible: aún se intenta revocar refresh si vino.
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(solicitud.TokenRenovacionOpcional))
            {
                string hash = HashTokenRenovacion.AHexMinuscula(solicitud.TokenRenovacionOpcional);
                await _repositorio.RevocarRefreshPorHashAsync(hash, cancellationToken).ConfigureAwait(false);
            }

            return Respuesta<bool>.Ok(true, "Sesión cerrada.");
        }

        /// <summary>
        /// Extrae el token sin el prefijo Bearer del encabezado Authorization.
        /// </summary>
        private static string? ExtraerBearer(string? header)
        {
            if (string.IsNullOrWhiteSpace(header)) return null;
            const string prefijo = "Bearer ";
            return header.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase)
                ? header[prefijo.Length..].Trim()
                : null;
        }
    }
}
