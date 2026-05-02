using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos
{
    public sealed record CrearMateriaCommand(string Nombre, byte Creditos, int ProfesorId, int ProgramaCreditoId) : IRequest<Respuesta<int>>;
    public sealed class CrearMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<CrearMateriaCommand, Respuesta<int>>
    {
        public async Task<Respuesta<int>> Handle(CrearMateriaCommand r, CancellationToken ct) =>
            Respuesta<int>.Ok(await repo.InsertarMateriaAsync(r.Nombre, r.Creditos, r.ProfesorId, r.ProgramaCreditoId, ct).ConfigureAwait(false), "Materia creada.");
    }
    public sealed class CrearMateriaValidator : AbstractValidator<CrearMateriaCommand>
    {
        public CrearMateriaValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Creditos).Equal((byte)3).WithMessage("Cada materia debe valer exactamente 3 créditos.");
            RuleFor(x => x.ProfesorId).GreaterThan(0);
            RuleFor(x => x.ProgramaCreditoId).GreaterThan(0);
        }
    }

    public sealed record ActualizarMateriaCommand(int MateriaId, string Nombre, byte Creditos, int ProfesorId, int ProgramaCreditoId, byte Estado) : IRequest<Respuesta<bool>>;
    public sealed class ActualizarMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<ActualizarMateriaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(ActualizarMateriaCommand r, CancellationToken ct)
        {
            await repo.ActualizarMateriaAsync(r.MateriaId, r.Nombre, r.Creditos, r.ProfesorId, r.ProgramaCreditoId, r.Estado, ct).ConfigureAwait(false);
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

    public sealed record EliminarMateriaCommand(int MateriaId) : IRequest<Respuesta<bool>>;
    public sealed class EliminarMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarMateriaCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarMateriaCommand r, CancellationToken ct)
        {
            await repo.EliminarMateriaAsync(r.MateriaId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Materia inactivada.");
        }
    }
    public sealed class EliminarMateriaValidator : AbstractValidator<EliminarMateriaCommand>
    {
        public EliminarMateriaValidator() => RuleFor(x => x.MateriaId).GreaterThan(0);
    }

    public sealed record ObtenerMateriaPorIdQuery(int MateriaId) : IRequest<Respuesta<MateriaDetalleDto>>;
    public sealed class ObtenerMateriaHandler(IRepositorioAcademico repo) : IRequestHandler<ObtenerMateriaPorIdQuery, Respuesta<MateriaDetalleDto>>
    {
        public async Task<Respuesta<MateriaDetalleDto>> Handle(ObtenerMateriaPorIdQuery r, CancellationToken ct)
        {
            MateriaDetalleDto? d = await repo.ObtenerMateriaPorIdAsync(r.MateriaId, ct).ConfigureAwait(false);
            if (d is null) throw new KeyNotFoundException("Materia no encontrada.");
            return Respuesta<MateriaDetalleDto>.Ok(d);
        }
    }

    public sealed record ListarMateriasPorProgramaQuery(int? ProgramaCreditoId, bool SoloActivos = true) : IRequest<Respuesta<IReadOnlyList<MateriaCatalogoDto>>>;
    public sealed class ListarMateriasPorProgramaHandler(IRepositorioAcademico repo) : IRequestHandler<ListarMateriasPorProgramaQuery, Respuesta<IReadOnlyList<MateriaCatalogoDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<MateriaCatalogoDto>>> Handle(ListarMateriasPorProgramaQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<MateriaCatalogoDto>>.Ok(await repo.ListarMateriasPorProgramaAsync(r.ProgramaCreditoId, r.SoloActivos, ct).ConfigureAwait(false));
    }
}
