using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Consultas
{
    public sealed record ObtenerProfesorPorIdQuery(int ProfesorId) : IRequest<Respuesta<ProfesorDto>>;

    public sealed class ObtenerProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<ObtenerProfesorPorIdQuery, Respuesta<ProfesorDto>>
    {
        public async Task<Respuesta<ProfesorDto>> Handle(ObtenerProfesorPorIdQuery r, CancellationToken ct)
        {
            ProfesorDto? d = await repo.ObtenerProfesorPorIdAsync(r.ProfesorId, ct).ConfigureAwait(false);
            if (d is null) throw new KeyNotFoundException("Profesor no encontrado.");
            return Respuesta<ProfesorDto>.Ok(d);
        }
    }

}

