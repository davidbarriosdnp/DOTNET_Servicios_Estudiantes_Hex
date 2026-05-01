using System.Net.Mime;
using System.Text.Json;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Utilidades
{
    public static class JsonRespuestaEscritorio
    {
        internal static readonly JsonSerializerOptions Serializer = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public static async Task EscribirAsync<T>(
            HttpResponse respuesta,
            int codigoHttp,
            Respuesta<T> modelo,
            CancellationToken cancellationToken = default)
        {
            respuesta.StatusCode = codigoHttp;
            respuesta.ContentType = MediaTypeNames.Application.Json;

            respuesta.Headers.Remove("WWW-Authenticate");

            string json = JsonSerializer.Serialize(modelo, Serializer);
            await respuesta.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Intenta traducir el mensaje técnico de IdentityModel al cliente.</summary>
        public static string MensajeFirmaJwt(string? texto)
        {
            if (string.IsNullOrEmpty(texto)) return "Credenciales o token inválidos.";
            string t = texto;
            return t.IndexOf("signature key was not found", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   t.IndexOf("IDX10517", StringComparison.OrdinalIgnoreCase) >= 0
                ? "No se puede validar el token (clave JWT desalineada o token emitido por otra configuración)."
                : t;
        }
    }
}
