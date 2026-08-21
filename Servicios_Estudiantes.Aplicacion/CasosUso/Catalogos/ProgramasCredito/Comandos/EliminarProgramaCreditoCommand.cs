using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Comandos
{
    public sealed record EliminarProgramaCreditoCommand(int ProgramaCreditoId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarProgramaCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarProgramaCreditoCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarProgramaCreditoCommand r, CancellationToken ct)
        {
            await repo.EliminarProgramaCreditoAsync(r.ProgramaCreditoId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Programa inactivado.");
        }
    }

    public sealed class EliminarProgramaCreditoValidator : AbstractValidator<EliminarProgramaCreditoCommand>
    {
        public EliminarProgramaCreditoValidator() => RuleFor(x => x.ProgramaCreditoId).GreaterThan(0);
    }

}

