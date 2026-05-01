using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos
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

    public sealed record ListarProgramasCreditoQuery(bool SoloActivos = false) : IRequest<Respuesta<IReadOnlyList<ProgramaCreditoDto>>>;
    public sealed class ListarProgramasCreditoHandler(IRepositorioAcademico repo) : IRequestHandler<ListarProgramasCreditoQuery, Respuesta<IReadOnlyList<ProgramaCreditoDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<ProgramaCreditoDto>>> Handle(ListarProgramasCreditoQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<ProgramaCreditoDto>>.Ok(await repo.ListarProgramasCreditoAsync(r.SoloActivos, ct).ConfigureAwait(false));
    }
}
