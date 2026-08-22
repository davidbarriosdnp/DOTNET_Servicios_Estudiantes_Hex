using MediatR;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Consultas;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class AulasV1
    {
        public static RouteGroupBuilder MapAulas(this RouteGroupBuilder group)
        {
            group.RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapGet("", async (IMediator mediator, bool soloActivos, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListarAulasQuery(soloActivos), cancellationToken)));

            group.MapGet("{aulaId:int}", async (IMediator mediator, int aulaId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ObtenerAulaPorIdQuery(aulaId), cancellationToken)));

            group.MapPost("", async (IMediator mediator, CuerpoCrearAula cuerpo, CancellationToken cancellationToken) =>
            {
                Respuesta<int> resultado = await mediator.Send(
                    new CrearAulaCommand(cuerpo.Nombre, cuerpo.Capacidad, cuerpo.SedeId), 
                    cancellationToken);
                return Results.Created($"/api/v1/Aulas/{resultado.Resultado}", resultado);
            });

            group.MapPut("{aulaId:int}", async (
                IMediator mediator,
                int aulaId,
                CuerpoActualizarAula cuerpo,
                CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(
                    new ActualizarAulaCommand(aulaId, cuerpo.Nombre, cuerpo.Capacidad, cuerpo.SedeId, cuerpo.Estado),
                    cancellationToken)));

            group.MapDelete("{aulaId:int}", async (IMediator mediator, int aulaId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new EliminarAulaCommand(aulaId), cancellationToken)));

            return group;
        }
    }

    public sealed record CuerpoCrearAula(string Nombre, int Capacidad, int SedeId);
    public sealed record CuerpoActualizarAula(string Nombre, int Capacidad, int SedeId, byte Estado);
}
