using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas
{
    public sealed record ObtenerEstudiantePorIdQuery(int EstudianteId) : IRequest<Respuesta<EstudianteDetalleDto>>;

    public sealed class ObtenerEstudiantePorIdQueryHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ObtenerEstudiantePorIdQuery, Respuesta<EstudianteDetalleDto>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<EstudianteDetalleDto>> Handle(ObtenerEstudiantePorIdQuery solicitud, CancellationToken cancellationToken)
        {
            EstudianteDetalleDto? fila = await _repositorio.ObtenerEstudiantePorIdAsync(solicitud.EstudianteId, cancellationToken)
                .ConfigureAwait(false);

            if (fila is null)
                throw new KeyNotFoundException("Estudiante no encontrado.");

            return Respuesta<EstudianteDetalleDto>.Ok(fila);
        }
    }
}
