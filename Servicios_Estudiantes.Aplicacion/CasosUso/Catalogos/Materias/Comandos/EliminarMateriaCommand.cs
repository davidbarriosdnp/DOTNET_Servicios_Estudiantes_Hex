using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Comandos
{
    public sealed record EliminarMateriaCommand(int MateriaId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarMateriaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarMateriaCommand r, CancellationToken ct)
        {
            await repo.EliminarMateriaAsync(r.MateriaId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia inactivada.");
        }
    }

    public sealed class EliminarMateriaValidator : AbstractValidator<EliminarMateriaCommand>
    {
        public EliminarMateriaValidator() => RuleFor(x => x.MateriaId).GreaterThan(0);
    }

}

