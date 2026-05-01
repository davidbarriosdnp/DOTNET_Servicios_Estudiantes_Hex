using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using MediatR;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class ProfesoresV1
    {
        public static RouteGroupBuilder MapProfesores(this RouteGroupBuilder group)
        {
            group.MapGet("", async (IMediator m, bool soloActivos, CancellationToken ct) =>
                Results.Ok(await m.Send(new ListarProfesoresQuery(soloActivos), ct)));

            group.MapGet("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new ObtenerProfesorPorIdQuery(id), ct)));

            group.MapPost("", async (IMediator m, CrearProfesorCommand cmd, CancellationToken ct) =>
            {
                Respuesta<int> r = await m.Send(cmd, ct);
                return Results.Created($"/api/v1/Profesores/{r.Resultado}", r);
            });

            group.MapPut("{id:int}", async (IMediator m, int id, ActualizarProfesorCuerpo c, CancellationToken ct) =>
                Results.Ok(await m.Send(new ActualizarProfesorCommand(id, c.Nombre, c.Estado), ct)));

            group.MapDelete("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new EliminarProfesorCommand(id), ct)));

            return group;
        }
    }

    public sealed record ActualizarProfesorCuerpo(string Nombre, byte Estado);
}
