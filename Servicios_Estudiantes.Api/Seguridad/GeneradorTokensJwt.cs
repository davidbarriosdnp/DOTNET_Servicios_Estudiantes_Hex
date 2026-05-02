using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Aplicacion.Constantes;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Api.Seguridad
{
    public sealed class GeneradorTokensJwt(FirmaSymmetricaJwt firma) : IGeneradorTokensJwt
    {
        private readonly JwtOpciones _opciones = firma.Opciones;

        public ResultadoEmisionTokenAcceso CrearTokenAcceso(int usuarioId, string nombreUsuario, string rol, int? estudianteId = null)
        {
            string jti = Guid.NewGuid().ToString("N");
            DateTime expiraUtc = DateTime.UtcNow.AddMinutes(Math.Max(1, _opciones.MinutosValidezAcceso));

            List<Claim> reclamaciones =
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString(CultureInfo.InvariantCulture)),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.Name, nombreUsuario),
                new Claim(ClaimTypes.Role, rol)
            ];
            if (estudianteId.HasValue)
                reclamaciones.Add(new Claim(JwtReclamaciones.EstudianteId, estudianteId.Value.ToString(CultureInfo.InvariantCulture)));

            JwtSecurityToken token = new(
                _opciones.Emisor,
                _opciones.Audiencia,
                reclamaciones,
                expires: expiraUtc,
                signingCredentials: firma.CredencialesFirma);

            string serializado = new JwtSecurityTokenHandler().WriteToken(token);
            return new ResultadoEmisionTokenAcceso(serializado, jti, expiraUtc);
        }

        public string CrearTokenRenovacion()
        {
            Span<byte> aleatorio = stackalloc byte[32];
            RandomNumberGenerator.Fill(aleatorio);
            return Convert.ToBase64String(aleatorio).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public DateTime CalcularExpiracionRenovacionUtc() =>
            DateTime.UtcNow.AddDays(Math.Max(1, _opciones.DiasValidezRefresh));
    }
}
