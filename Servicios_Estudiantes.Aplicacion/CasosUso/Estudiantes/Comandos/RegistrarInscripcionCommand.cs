using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    public sealed record RegistrarInscripcionCommand(
        int EstudianteId,
        int MateriaId1,
        int MateriaId2,
        int MateriaId3) : IRequest<Respuesta<bool>>;

    public sealed class RegistrarInscripcionCommandHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<RegistrarInscripcionCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(RegistrarInscripcionCommand solicitud, CancellationToken cancellationToken)
        {
            await _repositorio.RegistrarInscripcionAsync(
                solicitud.EstudianteId,
                solicitud.MateriaId1,
                solicitud.MateriaId2,
                solicitud.MateriaId3,
                cancellationToken).ConfigureAwait(false);

            return Respuesta<bool>.Ok(true, "Inscripción registrada (3 materias, 9 créditos).");
        }
    }

    public sealed class RegistrarInscripcionCommandValidator : AbstractValidator<RegistrarInscripcionCommand>
    {
        public RegistrarInscripcionCommandValidator()
        {
            RuleFor(c => c.EstudianteId).GreaterThan(0);
            RuleFor(c => c.MateriaId1).GreaterThan(0);
            RuleFor(c => c.MateriaId2).GreaterThan(0);
            RuleFor(c => c.MateriaId3).GreaterThan(0);
            RuleFor(c => c).Must(c =>
                    c.MateriaId1 != c.MateriaId2 &&
                    c.MateriaId1 != c.MateriaId3 &&
                    c.MateriaId2 != c.MateriaId3)
                .WithMessage("Las tres materias deben ser distintas.");
        }
    }
}
