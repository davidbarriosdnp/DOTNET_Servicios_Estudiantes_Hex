using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Comandos
{
    public sealed record CrearProgramaCreditoCommand(string Nombre, byte CreditosPorMateria, byte MaxMateriasPorEstudiante) : IRequest<Respuesta<int>>;

    public sealed class CrearProgramaCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<CrearProgramaCreditoCommand, Respuesta<int>>
    {
        public async Task<Respuesta<int>> Handle(CrearProgramaCreditoCommand r, CancellationToken ct) =>
            Respuesta<int>.Ok(await repo.InsertarProgramaCreditoAsync(r.Nombre, r.CreditosPorMateria, r.MaxMateriasPorEstudiante, ct).ConfigureAwait(false), "Programa creado.");
    }

    public sealed class CrearProgramaCreditoValidator : AbstractValidator<CrearProgramaCreditoCommand>
    {
        public CrearProgramaCreditoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.CreditosPorMateria).GreaterThan((byte)0);
            RuleFor(x => x.MaxMateriasPorEstudiante).GreaterThan((byte)0);
        }
    }

}

