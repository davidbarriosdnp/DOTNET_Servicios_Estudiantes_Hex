using MediatR;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Consultas;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class SedesV1
    {
        public static RouteGroupBuilder MapSedes(this RouteGroupBuilder group)
        {
            group.RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapGet("", async (IMediator mediator, bool soloActivos, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListarSedesQuery(soloActivos), cancellationToken)));

            group.MapGet("{sedeId:int}", async (IMediator mediator, int sedeId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ObtenerSedePorIdQuery(sedeId), cancellationToken)));

            group.MapPost("", async (IMediator mediator, CuerpoCrearSede cuerpo, CancellationToken cancellationToken) =>
            {
                Respuesta<int> resultado = await mediator.Send(
                    new CrearSedeCommand(cuerpo.Nombre, cuerpo.Direccion), 
                    cancellationToken);
                return Results.Created($"/api/v1/Sedes/{resultado.Resultado}", resultado);
            });

            group.MapPut("{sedeId:int}", async (
                IMediator mediator,
                int sedeId,
                CuerpoActualizarSede cuerpo,
                CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(
                    new ActualizarSedeCommand(sedeId, cuerpo.Nombre, cuerpo.Direccion, cuerpo.Estado),
                    cancellationToken)));

            group.MapDelete("{sedeId:int}", async (IMediator mediator, int sedeId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new EliminarSedeCommand(sedeId), cancellationToken)));

            return group;
        }
    }

    public sealed record CuerpoCrearSede(string Nombre, string Direccion);
    public sealed record CuerpoActualizarSede(string Nombre, string Direccion, byte Estado);
}
