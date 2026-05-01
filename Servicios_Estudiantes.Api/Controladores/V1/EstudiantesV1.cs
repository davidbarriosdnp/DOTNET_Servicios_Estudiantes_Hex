using Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using MediatR;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class EstudiantesV1
    {
        public static RouteGroupBuilder MapEstudiantes(this RouteGroupBuilder group)
        {
            group.RequireAuthorization();

            group.MapGet("catalogo/materias", async (IMediator mediator, int? programaCreditoId, bool soloActivos, CancellationToken cancellationToken) =>
            {
                Respuesta<IReadOnlyList<MateriaCatalogoDto>> resultado =
                    await mediator.Send(new ListarMateriasCatalogoQuery(programaCreditoId, soloActivos), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapGet("{estudianteId:int}/materias/{materiaId:int}/companeros", async (
                IMediator mediator,
                int estudianteId,
                int materiaId,
                CancellationToken cancellationToken) =>
            {
                Respuesta<IReadOnlyList<string>> resultado =
                    await mediator.Send(new ListarCompanerosPorMateriaQuery(estudianteId, materiaId), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapGet("{estudianteId:int}/inscripcion", async (IMediator mediator, int estudianteId, bool soloActivas, CancellationToken cancellationToken) =>
            {
                Respuesta<IReadOnlyList<InscripcionEstudianteDto>> resultado =
                    await mediator.Send(new ListarInscripcionEstudianteQuery(estudianteId, soloActivas), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapPost("{estudianteId:int}/inscripcion/fila", async (IMediator mediator, int estudianteId, InscripcionFilaCuerpo cuerpo, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new AgregarInscripcionFilaCommand(estudianteId, cuerpo.MateriaId), cancellationToken)));

            group.MapDelete("{estudianteId:int}/inscripcion/materias/{materiaId:int}", async (IMediator mediator, int estudianteId, int materiaId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new EliminarInscripcionFilaCommand(estudianteId, materiaId), cancellationToken)));

            group.MapPut("{estudianteId:int}/inscripcion/materia", async (IMediator mediator, int estudianteId, CambiarMateriaCuerpo cuerpo, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new CambiarMateriaInscripcionCommand(estudianteId, cuerpo.MateriaIdAnterior, cuerpo.MateriaIdNueva), cancellationToken)));

            group.MapGet("{estudianteId:int}", async (IMediator mediator, int estudianteId, CancellationToken cancellationToken) =>
            {
                Respuesta<EstudianteDetalleDto> resultado =
                    await mediator.Send(new ObtenerEstudiantePorIdQuery(estudianteId), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapGet("", async (IMediator mediator, bool soloActivos, CancellationToken cancellationToken) =>
            {
                Respuesta<IReadOnlyList<EstudianteRegistroDto>> resultado =
                    await mediator.Send(new ListarRegistrosEstudiantesQuery(soloActivos), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapPost("", async (IMediator mediator, CrearEstudianteCommand comando, CancellationToken cancellationToken) =>
            {
                Respuesta<int> resultado = await mediator.Send(comando, cancellationToken);
                return Results.Created($"/api/v1/Estudiantes/{resultado.Resultado}", resultado);
            });

            group.MapPut("{estudianteId:int}", async (
                IMediator mediator,
                int estudianteId,
                ActualizarEstudianteCuerpo cuerpo,
                CancellationToken cancellationToken) =>
            {
                Respuesta<bool> resultado = await mediator.Send(
                    new ActualizarEstudianteCommand(estudianteId, cuerpo.Nombre, cuerpo.Email, cuerpo.ProgramaCreditoId, cuerpo.Estado),
                    cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapDelete("{estudianteId:int}", async (IMediator mediator, int estudianteId, CancellationToken cancellationToken) =>
            {
                Respuesta<bool> resultado = await mediator.Send(new EliminarEstudianteCommand(estudianteId), cancellationToken);
                return Results.Ok(resultado);
            });

            group.MapPost("{estudianteId:int}/inscripcion", async (
                IMediator mediator,
                int estudianteId,
                InscripcionSolicitudApi cuerpo,
                CancellationToken cancellationToken) =>
            {
                Respuesta<bool> resultado = await mediator.Send(
                    new RegistrarInscripcionCommand(
                        estudianteId,
                        cuerpo.MateriaId1,
                        cuerpo.MateriaId2,
                        cuerpo.MateriaId3),
                    cancellationToken);
                return Results.Ok(resultado);
            });

            return group;
        }
    }

    public sealed record ActualizarEstudianteCuerpo(string Nombre, string Email, int? ProgramaCreditoId, byte? Estado);

    public sealed record InscripcionSolicitudApi(int MateriaId1, int MateriaId2, int MateriaId3);

    public sealed record InscripcionFilaCuerpo(int MateriaId);

    public sealed record CambiarMateriaCuerpo(int MateriaIdAnterior, int MateriaIdNueva);
}
