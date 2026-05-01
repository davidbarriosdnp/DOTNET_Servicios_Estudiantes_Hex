using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Servicios_Estudiantes.Api.Configuraciones;

namespace Servicios_Estudiantes.Api.Seguridad
{
    /// <summary>
    /// Unifica la creación del material de firma HS256 entre emisión y validación del JWT,
    /// evitando inconsistencias (“signature key was not found”).
    /// </summary>
    public sealed class FirmaSymmetricaJwt
    {
        public FirmaSymmetricaJwt(IOptions<JwtOpciones> opciones)
        {
            JwtOpciones o = opciones.Value;
            o.Validar();

            Opciones = o;
            byte[] bytes = Encoding.UTF8.GetBytes(o.ClaveSecreta.Trim());
            // Sin KeyId: el bearer valida contra una sola IssuerSigningKey sin exigir "kid" en el encabezado.
            Clave = new SymmetricSecurityKey(bytes);
            CredencialesFirma = new SigningCredentials(Clave, SecurityAlgorithms.HmacSha256);
        }

        public JwtOpciones Opciones { get; }

        public SymmetricSecurityKey Clave { get; }

        public SigningCredentials CredencialesFirma { get; }
    }
}
