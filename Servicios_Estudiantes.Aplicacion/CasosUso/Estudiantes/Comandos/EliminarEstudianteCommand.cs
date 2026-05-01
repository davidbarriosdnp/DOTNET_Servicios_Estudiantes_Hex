using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    public sealed record EliminarEstudianteCommand(int EstudianteId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarEstudianteCommandHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<EliminarEstudianteCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(EliminarEstudianteCommand solicitud, CancellationToken cancellationToken)
        {
            await _repositorio.EliminarEstudianteAsync(solicitud.EstudianteId, cancellationToken).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Registro inactivado.");
        }
    }

    public sealed class EliminarEstudianteCommandValidator : AbstractValidator<EliminarEstudianteCommand>
    {
        public EliminarEstudianteCommandValidator() => RuleFor(c => c.EstudianteId).GreaterThan(0);
    }
}
