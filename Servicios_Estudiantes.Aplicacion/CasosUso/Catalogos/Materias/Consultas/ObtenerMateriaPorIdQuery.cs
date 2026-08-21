using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Consultas
{
    public sealed record ObtenerMateriaPorIdQuery(int MateriaId) : IRequest<Respuesta<MateriaDetalleDto>>;

    public sealed class ObtenerMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<ObtenerMateriaPorIdQuery, Respuesta<MateriaDetalleDto>>
    {
        public async Task<Respuesta<MateriaDetalleDto>> Handle(ObtenerMateriaPorIdQuery r, CancellationToken ct)
        {
            MateriaDetalleDto? d = await repo.ObtenerMateriaPorIdAsync(r.MateriaId, ct).ConfigureAwait(false);
            if (d is null) throw new KeyNotFoundException("Materia no encontrada.");
            return Respuesta<MateriaDetalleDto>.Ok(d);
        }
    }

}

