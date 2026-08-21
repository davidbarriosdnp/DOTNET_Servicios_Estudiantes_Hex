using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Consultas
{
    public sealed record ListarMateriasPorProgramaQuery(int? ProgramaCreditoId, bool SoloActivos = true) : IRequest<Respuesta<IReadOnlyList<MateriaCatalogoDto>>>;

    public sealed class ListarMateriasPorProgramaHandler(IRepositorioAcademico repo) : IRequestHandler<ListarMateriasPorProgramaQuery, Respuesta<IReadOnlyList<MateriaCatalogoDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<MateriaCatalogoDto>>> Handle(ListarMateriasPorProgramaQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<MateriaCatalogoDto>>.Ok(await repo.ListarMateriasPorProgramaAsync(r.ProgramaCreditoId, r.SoloActivos, ct).ConfigureAwait(false));
    }
}
