using Microsoft.AspNetCore.Http;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Utilidades
{
    /// <summary>Respuestas HTTP coherentes con el envoltorio <see cref="Respuesta{T}"/> del API.</summary>
    public static class RespuestaAccesoApi
    {
        public static IResult Prohibido(string mensaje = "No tiene permiso para realizar esta operación.") =>
            Results.Json(
                Respuesta<object?>.Fail(mensaje),
                JsonRespuestaEscritorio.Serializer,
                contentType: "application/json; charset=utf-8",
                statusCode: StatusCodes.Status403Forbidden);
    }
}
