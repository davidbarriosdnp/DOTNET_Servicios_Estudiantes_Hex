using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using MediatR;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class ProgramasCreditoV1
    {
        public static RouteGroupBuilder MapProgramasCredito(this RouteGroupBuilder group)
        {
            group.MapGet("", async (IMediator m, bool soloActivos, CancellationToken ct) =>
                Results.Ok(await m.Send(new ListarProgramasCreditoQuery(soloActivos), ct)));

            group.MapGet("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new ObtenerProgramaCreditoPorIdQuery(id), ct)));

            group.MapPost("", async (IMediator m, CrearProgramaCreditoCommand cmd, CancellationToken ct) =>
            {
                Respuesta<int> r = await m.Send(cmd, ct);
                return Results.Created($"/api/v1/ProgramasCredito/{r.Resultado}", r);
            });

            group.MapPut("{id:int}", async (IMediator m, int id, ActualizarProgramaCreditoCuerpo c, CancellationToken ct) =>
                Results.Ok(await m.Send(new ActualizarProgramaCreditoCommand(id, c.Nombre, c.CreditosPorMateria, c.MaxMateriasPorEstudiante, c.Estado), ct)));

            group.MapDelete("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new EliminarProgramaCreditoCommand(id), ct)));

            return group;
        }
    }

    public sealed record ActualizarProgramaCreditoCuerpo(string Nombre, byte CreditosPorMateria, byte MaxMateriasPorEstudiante, byte Estado);
}
