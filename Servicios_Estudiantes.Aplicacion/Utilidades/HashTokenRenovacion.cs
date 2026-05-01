using System.Security.Cryptography;
using System.Text;

namespace Servicios_Estudiantes.Aplicacion.Utilidades
{
    public static class HashTokenRenovacion
    {
        public static string AHexMinuscula(string tokenPlano)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlano));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
