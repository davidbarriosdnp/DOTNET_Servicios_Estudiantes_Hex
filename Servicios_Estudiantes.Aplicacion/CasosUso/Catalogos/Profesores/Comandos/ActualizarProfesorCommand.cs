using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Comandos
{
    public sealed record ActualizarProfesorCommand(int ProfesorId, string Nombre, byte Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<ActualizarProfesorCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(ActualizarProfesorCommand r, CancellationToken ct)
        {
            await repo.ActualizarProfesorAsync(r.ProfesorId, r.Nombre, r.Estado, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Profesor actualizado.");
        }
    }

    public sealed class ActualizarProfesorValidator : AbstractValidator<ActualizarProfesorCommand>
    {
        public ActualizarProfesorValidator()
        {
            RuleFor(x => x.ProfesorId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Estado).InclusiveBetween((byte)0, (byte)1);
        }
    }

}

