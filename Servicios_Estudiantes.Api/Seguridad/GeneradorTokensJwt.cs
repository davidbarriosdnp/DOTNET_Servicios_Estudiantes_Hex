using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Api.Seguridad
{
    public sealed class GeneradorTokensJwt(IOptions<JwtOpciones> opciones) : IGeneradorTokensJwt
    {
        private readonly JwtOpciones _opciones = opciones.Value;

        public ResultadoEmisionTokenAcceso CrearTokenAcceso(int usuarioId, string nombreUsuario, string rol)
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

            SymmetricSecurityKey clave = new(Encoding.UTF8.GetBytes(_opciones.ClaveSecreta));
            SigningCredentials credenciales = new(clave, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                _opciones.Emisor,
                _opciones.Audiencia,
                reclamaciones,
                expires: expiraUtc,
                signingCredentials: credenciales);

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
