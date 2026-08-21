using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Consultas
{
    public sealed record ListarProgramasCreditoQuery(bool SoloActivos = false) : IRequest<Respuesta<IReadOnlyList<ProgramaCreditoDto>>>;

    public sealed class ListarProgramasCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<ListarProgramasCreditoQuery, Respuesta<IReadOnlyList<ProgramaCreditoDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<ProgramaCreditoDto>>> Handle(ListarProgramasCreditoQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<ProgramaCreditoDto>>.Ok(await repo.ListarProgramasCreditoAsync(r.SoloActivos, ct).ConfigureAwait(false));
    }
}
