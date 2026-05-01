using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos
{
    public sealed record AgregarInscripcionFilaCommand(int EstudianteId, int MateriaId) : IRequest<Respuesta<bool>>;

    public sealed class AgregarInscripcionFilaHandler(IRepositorioAcademico repo) : IRequestHandler<AgregarInscripcionFilaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(AgregarInscripcionFilaCommand r, CancellationToken ct)
        {
            await repo.InsertarInscripcionFilaAsync(r.EstudianteId, r.MateriaId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia agregada a la inscripción.");
        }
    }

    public sealed class AgregarInscripcionFilaValidator : AbstractValidator<AgregarInscripcionFilaCommand>
    {
        public AgregarInscripcionFilaValidator()
        {
            RuleFor(x => x.EstudianteId).GreaterThan(0);
            RuleFor(x => x.MateriaId).GreaterThan(0);
        }
    }

    public sealed record EliminarInscripcionFilaCommand(int EstudianteId, int MateriaId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarInscripcionFilaHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarInscripcionFilaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarInscripcionFilaCommand r, CancellationToken ct)
        {
            await repo.EliminarInscripcionFilaAsync(r.EstudianteId, r.MateriaId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia retirada de la inscripción.");
        }
    }

    public sealed class EliminarInscripcionFilaValidator : AbstractValidator<EliminarInscripcionFilaCommand>
    {
        public EliminarInscripcionFilaValidator()
        {
            RuleFor(x => x.EstudianteId).GreaterThan(0);
            RuleFor(x => x.MateriaId).GreaterThan(0);
        }
    }

    public sealed record CambiarMateriaInscripcionCommand(int EstudianteId, int MateriaIdAnterior, int MateriaIdNueva) : IRequest<Respuesta<bool>>;

    public sealed class CambiarMateriaInscripcionHandler(IRepositorioAcademico repo) : IRequestHandler<CambiarMateriaInscripcionCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(CambiarMateriaInscripcionCommand r, CancellationToken ct)
        {
            await repo.ActualizarInscripcionMateriaAsync(r.EstudianteId, r.MateriaIdAnterior, r.MateriaIdNueva, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia de inscripción actualizada.");
        }
    }

    public sealed class CambiarMateriaInscripcionValidator : AbstractValidator<CambiarMateriaInscripcionCommand>
    {
        public CambiarMateriaInscripcionValidator()
        {
            RuleFor(x => x.EstudianteId).GreaterThan(0);
            RuleFor(x => x.MateriaIdAnterior).GreaterThan(0);
            RuleFor(x => x.MateriaIdNueva).GreaterThan(0);
            RuleFor(x => x).Must(x => x.MateriaIdAnterior != x.MateriaIdNueva).WithMessage("La materia nueva debe ser distinta.");
        }
    }
}
