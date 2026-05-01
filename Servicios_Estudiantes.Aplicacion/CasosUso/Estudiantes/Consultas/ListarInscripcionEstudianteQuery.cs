using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas
{
    public sealed record ListarInscripcionEstudianteQuery(int EstudianteId, bool SoloActivas = true) : IRequest<Respuesta<IReadOnlyList<InscripcionEstudianteDto>>>;

    public sealed class ListarInscripcionEstudianteQueryHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ListarInscripcionEstudianteQuery, Respuesta<IReadOnlyList<InscripcionEstudianteDto>>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<InscripcionEstudianteDto>>> Handle(
            ListarInscripcionEstudianteQuery solicitud,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<InscripcionEstudianteDto> lista =
                await _repositorio.ListarInscripcionPorEstudianteAsync(solicitud.EstudianteId, solicitud.SoloActivas, cancellationToken)
                    .ConfigureAwait(false);

            return Respuesta<IReadOnlyList<InscripcionEstudianteDto>>.Ok(lista);
        }
    }
}
