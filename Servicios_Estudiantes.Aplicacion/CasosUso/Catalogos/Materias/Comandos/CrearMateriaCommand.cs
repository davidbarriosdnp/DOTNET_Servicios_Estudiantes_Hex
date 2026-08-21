using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Comandos
{
    public sealed record CrearMateriaCommand(string Nombre, byte Creditos, int ProfesorId, int ProgramaCreditoId, int? AulaId, DateOnly? FechaInicio, DateOnly? FechaFin, TimeOnly? HoraInicio, TimeOnly? HoraFin) : IRequest<Respuesta<int>>;

    public sealed class CrearMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<CrearMateriaCommand, Respuesta<int>>
    {
        public async Task<Respuesta<int>> Handle(CrearMateriaCommand r, CancellationToken ct) =>
            Respuesta<int>.Ok(await repo.InsertarMateriaAsync(r.Nombre, r.Creditos, r.ProfesorId, r.ProgramaCreditoId, r.AulaId, r.FechaInicio, r.FechaFin, r.HoraInicio, r.HoraFin, ct).ConfigureAwait(false), "Materia creada.");
    }

    public sealed class CrearMateriaValidator : AbstractValidator<CrearMateriaCommand>
    {
        public CrearMateriaValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Creditos).Equal((byte)3).WithMessage("Cada materia debe valer exactamente 3 crǸditos.");
            RuleFor(x => x.ProfesorId).GreaterThan(0);
            RuleFor(x => x.ProgramaCreditoId).GreaterThan(0);
        }
    }

}

