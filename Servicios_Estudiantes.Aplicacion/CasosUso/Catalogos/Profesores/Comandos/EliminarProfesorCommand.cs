using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Comandos
{
    public sealed record EliminarProfesorCommand(int ProfesorId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarProfesorCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarProfesorCommand r, CancellationToken ct)
        {
            await repo.EliminarProfesorAsync(r.ProfesorId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Profesor inactivado.");
        }
    }

    public sealed class EliminarProfesorValidator : AbstractValidator<EliminarProfesorCommand>
    {
        public EliminarProfesorValidator() => RuleFor(x => x.ProfesorId).GreaterThan(0);
    }

}

