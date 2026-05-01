using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas
{
    public sealed record ListarMateriasCatalogoQuery(int? ProgramaCreditoId, bool SoloActivos = true) : IRequest<Respuesta<IReadOnlyList<MateriaCatalogoDto>>>;

    public sealed class ListarMateriasCatalogoQueryHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ListarMateriasCatalogoQuery, Respuesta<IReadOnlyList<MateriaCatalogoDto>>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<MateriaCatalogoDto>>> Handle(
            ListarMateriasCatalogoQuery solicitud,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<MateriaCatalogoDto> lista =
                await _repositorio.ListarMateriasPorProgramaAsync(solicitud.ProgramaCreditoId, solicitud.SoloActivos, cancellationToken)
                    .ConfigureAwait(false);

            return Respuesta<IReadOnlyList<MateriaCatalogoDto>>.Ok(lista);
        }
    }
}
