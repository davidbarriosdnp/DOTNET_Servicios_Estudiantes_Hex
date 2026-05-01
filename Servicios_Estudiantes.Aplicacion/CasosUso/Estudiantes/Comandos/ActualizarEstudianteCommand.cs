using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    public sealed record ActualizarEstudianteCommand(
        int EstudianteId,
        string Nombre,
        string Email,
        int? ProgramaCreditoId,
        byte? Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarEstudianteCommandHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ActualizarEstudianteCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(ActualizarEstudianteCommand solicitud, CancellationToken cancellationToken)
        {
            await _repositorio.ActualizarEstudianteAsync(
                solicitud.EstudianteId,
                solicitud.Nombre,
                solicitud.Email,
                solicitud.ProgramaCreditoId,
                solicitud.Estado,
                cancellationToken).ConfigureAwait(false);

            return Respuesta<bool>.Ok(true, "Registro actualizado.");
        }
    }

    public sealed class ActualizarEstudianteCommandValidator : AbstractValidator<ActualizarEstudianteCommand>
    {
        public ActualizarEstudianteCommandValidator()
        {
            RuleFor(c => c.EstudianteId).GreaterThan(0);
            RuleFor(c => c.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(c => c.Estado).InclusiveBetween((byte)0, (byte)1).When(c => c.Estado.HasValue);
        }
    }
}
