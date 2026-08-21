using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Consultas
{
    public sealed record ObtenerProgramaCreditoPorIdQuery(int ProgramaCreditoId) : IRequest<Respuesta<ProgramaCreditoDto>>;

    public sealed class ObtenerProgramaCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<ObtenerProgramaCreditoPorIdQuery, Respuesta<ProgramaCreditoDto>>
    {
        public async Task<Respuesta<ProgramaCreditoDto>> Handle(ObtenerProgramaCreditoPorIdQuery r, CancellationToken ct)
        {
            ProgramaCreditoDto? d = await repo.ObtenerProgramaCreditoPorIdAsync(r.ProgramaCreditoId, ct).ConfigureAwait(false);
            if (d is null) throw new KeyNotFoundException("Programa no encontrado.");
            return Respuesta<ProgramaCreditoDto>.Ok(d);
        }
    }

}

