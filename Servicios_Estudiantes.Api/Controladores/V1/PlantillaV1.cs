using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Plantilla.Comandos;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using MediatR;
using System.Text.Json;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class PlantillaV1
    {
        public static RouteGroupBuilder MapPlantilla(this RouteGroupBuilder group)
        {
            group.MapGet("", async (ILoggerFactory loggerFactory, IMediator mediator, ApiConfiguracion configuracion,[AsParameters] ComandoPruebaPlantilla comando) =>
            {
                ILogger logger = loggerFactory.CreateLogger(configuracion.NombreServicio);

                logger.LogInformation(
                    "📥 [GET] Prueba {NombreServicio} - Payload: {Payload}",
                    configuracion.NombreServicio,
                    JsonSerializer.Serialize(comando));

                Respuesta<bool> resultado = await mediator.Send(comando);

                logger.LogInformation(
                    "📤 [GET] {NombreServicio} - Resultado: {Result}",
                    configuracion.NombreServicio,
                    JsonSerializer.Serialize(resultado));

                return resultado;
            });
            return group;
        }
    }
}
