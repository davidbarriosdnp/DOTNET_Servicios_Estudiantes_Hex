using MediatR;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class ProgramasCreditoV1
    {
        public static RouteGroupBuilder MapProgramasCredito(this RouteGroupBuilder group)
        {
            // Lectura: cualquier usuario autenticado (directorio de estudiantes, contexto académico).
            group.MapGet("", async (IMediator m, bool soloActivos, CancellationToken ct) =>
                    Results.Ok(await m.Send(new ListarProgramasCreditoQuery(soloActivos), ct)))
                .RequireAuthorization();

            group.MapGet("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                    Results.Ok(await m.Send(new ObtenerProgramaCreditoPorIdQuery(id), ct)))
                .RequireAuthorization();

            group.MapPost("", async (IMediator m, CrearProgramaCreditoCommand cmd, CancellationToken ct) =>
                {
                    Respuesta<int> r = await m.Send(cmd, ct);
                    return Results.Created($"/api/v1/ProgramasCredito/{r.Resultado}", r);
                })
                .RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapPut("{id:int}", async (IMediator m, int id, ActualizarProgramaCreditoCuerpo c, CancellationToken ct) =>
                    Results.Ok(await m.Send(new ActualizarProgramaCreditoCommand(id, c.Nombre, c.CreditosPorMateria, c.MaxMateriasPorEstudiante, c.Estado), ct)))
                .RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapDelete("{id:int}", async (IMediator m, int id, CancellationToken ct) =>
                    Results.Ok(await m.Send(new EliminarProgramaCreditoCommand(id), ct)))
                .RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            return group;
        }
    }

    public sealed record ActualizarProgramaCreditoCuerpo(string Nombre, byte CreditosPorMateria, byte MaxMateriasPorEstudiante, byte Estado);
}
