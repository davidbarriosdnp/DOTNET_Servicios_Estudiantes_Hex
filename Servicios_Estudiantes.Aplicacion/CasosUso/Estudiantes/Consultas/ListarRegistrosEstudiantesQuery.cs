using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas
{
    public sealed record ListarRegistrosEstudiantesQuery(bool SoloActivos = false) : IRequest<Respuesta<IReadOnlyList<EstudianteRegistroDto>>>;

    public sealed class ListarRegistrosEstudiantesQueryHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ListarRegistrosEstudiantesQuery, Respuesta<IReadOnlyList<EstudianteRegistroDto>>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<EstudianteRegistroDto>>> Handle(
            ListarRegistrosEstudiantesQuery solicitud,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<EstudianteRegistroDto> lista =
                await _repositorio.ListarRegistrosEstudiantesAsync(solicitud.SoloActivos, cancellationToken).ConfigureAwait(false);

            return Respuesta<IReadOnlyList<EstudianteRegistroDto>>.Ok(lista);
        }
    }
}
