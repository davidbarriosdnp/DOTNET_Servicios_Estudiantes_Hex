using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    public sealed record CrearEstudianteCommand(string Nombre, string Email, int? ProgramaCreditoId) : IRequest<Respuesta<int>>;

    public sealed class CrearEstudianteCommandHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<CrearEstudianteCommand, Respuesta<int>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<int>> Handle(CrearEstudianteCommand solicitud, CancellationToken cancellationToken)
        {
            int id = await _repositorio.InsertarEstudianteAsync(
                solicitud.Nombre,
                solicitud.Email,
                solicitud.ProgramaCreditoId,
                usuarioId: null,
                cancellationToken).ConfigureAwait(false);

            return Respuesta<int>.Ok(id, "Registro creado.");
        }
    }

    public sealed class CrearEstudianteCommandValidator : AbstractValidator<CrearEstudianteCommand>
    {
        public CrearEstudianteCommandValidator()
        {
            RuleFor(c => c.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
        }
    }
}
