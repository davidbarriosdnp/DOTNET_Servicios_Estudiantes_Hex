using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    /// <summary>
    /// Comando para registrar la inscripción de un estudiante en materias (hasta 3).
    /// </summary>
    /// <param name="EstudianteId">Identificador del estudiante.</param>
    /// <param name="MateriaId1">Materia opcional 1.</param>
    /// <param name="MateriaId2">Materia opcional 2.</param>
    /// <param name="MateriaId3">Materia opcional 3.</param>
    public sealed record RegistrarInscripcionCommand(
        int EstudianteId,
        int? MateriaId1,
        int? MateriaId2,
        int? MateriaId3) : IRequest<Respuesta<bool>>;

    /// <summary>
    /// Manejador del comando RegistrarInscripcionCommand.
    /// </summary>
    public sealed class RegistrarInscripcionCommandHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<RegistrarInscripcionCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        /// <summary>
        /// Procesa la solicitud de inscripción delegando al puerto del repositorio.
        /// </summary>
        public async Task<Respuesta<bool>> Handle(RegistrarInscripcionCommand solicitud, CancellationToken cancellationToken)
        {
            await _repositorio.RegistrarInscripcionAsync(
                solicitud.EstudianteId,
                solicitud.MateriaId1,
                solicitud.MateriaId2,
                solicitud.MateriaId3,
                cancellationToken).ConfigureAwait(false);

            return Respuesta<bool>.Ok(true, "Inscripción registrada con éxito.");
        }
    }

    /// <summary>
    /// Validador de negocio del comando de inscripción.
    /// </summary>
    public sealed class RegistrarInscripcionCommandValidator : AbstractValidator<RegistrarInscripcionCommand>
    {
        /// <summary>
        /// Inicializa y define las reglas de validación para RegistrarInscripcionCommand.
        /// </summary>
        public RegistrarInscripcionCommandValidator()
        {
            RuleFor(c => c.EstudianteId).GreaterThan(0);
            
            // Validar que al menos se envíe una materia
            RuleFor(c => c).Must(c => c.MateriaId1.HasValue || c.MateriaId2.HasValue || c.MateriaId3.HasValue)
                .WithMessage("Debe seleccionar al menos una materia para inscribirse.");

            // Validar que si se envían varias materias, sean distintas
            RuleFor(c => c).Must(c => {
                List<int> list = new List<int>();
                if (c.MateriaId1.HasValue) list.Add(c.MateriaId1.Value);
                if (c.MateriaId2.HasValue) list.Add(c.MateriaId2.Value);
                if (c.MateriaId3.HasValue) list.Add(c.MateriaId3.Value);
                return list.Count == list.Distinct().Count();
            }).WithMessage("Las materias seleccionadas deben ser distintas.");
        }
    }
}
