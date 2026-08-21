using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Comandos
{
    public sealed record ActualizarProgramaCreditoCommand(int ProgramaCreditoId, string Nombre, byte CreditosPorMateria, byte MaxMateriasPorEstudiante, byte Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarProgramaCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<ActualizarProgramaCreditoCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(ActualizarProgramaCreditoCommand r, CancellationToken ct)
        {
            await repo.ActualizarProgramaCreditoAsync(r.ProgramaCreditoId, r.Nombre, r.CreditosPorMateria, r.MaxMateriasPorEstudiante, r.Estado, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Programa actualizado.");
        }
    }

    public sealed class ActualizarProgramaCreditoValidator : AbstractValidator<ActualizarProgramaCreditoCommand>
    {
        public ActualizarProgramaCreditoValidator()
        {
            RuleFor(x => x.ProgramaCreditoId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Estado).InclusiveBetween((byte)0, (byte)1);
        }
    }

}

