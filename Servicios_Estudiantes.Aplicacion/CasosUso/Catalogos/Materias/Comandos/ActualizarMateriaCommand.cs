using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Comandos
{
    public sealed record ActualizarMateriaCommand(int MateriaId, string Nombre, byte Creditos, int ProfesorId, int ProgramaCreditoId, int? AulaId, DateOnly? FechaInicio, DateOnly? FechaFin, TimeOnly? HoraInicio, TimeOnly? HoraFin, byte Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<ActualizarMateriaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(ActualizarMateriaCommand r, CancellationToken ct)
        {
            await repo.ActualizarMateriaAsync(r.MateriaId, r.Nombre, r.Creditos, r.ProfesorId, r.ProgramaCreditoId, r.AulaId, r.FechaInicio, r.FechaFin, r.HoraInicio, r.HoraFin, r.Estado, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia actualizada.");
        }
    }

    public sealed class ActualizarMateriaValidator : AbstractValidator<ActualizarMateriaCommand>
    {
        public ActualizarMateriaValidator()
        {
            RuleFor(x => x.MateriaId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Creditos).Equal((byte)3).WithMessage("Cada materia debe valer exactamente 3 créditos.");
            RuleFor(x => x.Estado).InclusiveBetween((byte)0, (byte)1);
        }
    }

}

