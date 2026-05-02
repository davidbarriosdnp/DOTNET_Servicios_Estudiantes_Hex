using MediatR;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class MateriasV1
    {
        public static RouteGroupBuilder MapMaterias(this RouteGroupBuilder group)
        {
            group.RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapGet("", async (IMediator m, int? programaCreditoId, bool soloActivos, CancellationToken ct) =>
                Results.Ok(await m.Send(new ListarMateriasPorProgramaQuery(programaCreditoId, soloActivos), ct)));

            group.MapGet("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new ObtenerMateriaPorIdQuery(id), ct)));

            group.MapPost("", async (IMediator m, CrearMateriaCommand cmd, CancellationToken ct) =>
            {
                Respuesta<int> r = await m.Send(cmd, ct);
                return Results.Created($"/api/v1/Materias/{r.Resultado}", r);
            });

            group.MapPut("{id:int}", async (IMediator m, int id, ActualizarMateriaCuerpo c, CancellationToken ct) =>
                Results.Ok(await m.Send(new ActualizarMateriaCommand(id, c.Nombre, c.Creditos, c.ProfesorId, c.ProgramaCreditoId, c.Estado), ct)));

            group.MapDelete("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                Results.Ok(await m.Send(new EliminarMateriaCommand(id), ct)));

            return group;
        }
    }

    public sealed record ActualizarMateriaCuerpo(string Nombre, byte Creditos, int ProfesorId, int ProgramaCreditoId, byte Estado);
}
