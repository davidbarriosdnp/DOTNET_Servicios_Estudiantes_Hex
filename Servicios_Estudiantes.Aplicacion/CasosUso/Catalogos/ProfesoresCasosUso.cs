using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos
{
    public sealed record CrearProfesorCommand(string Nombre) : IRequest<Respuesta<int>>;
    public sealed class CrearProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<CrearProfesorCommand, Respuesta<int>>
    {
        public async Task<Respuesta<int>> Handle(CrearProfesorCommand r, CancellationToken ct) =>
            Respuesta<int>.Ok(await repo.InsertarProfesorAsync(r.Nombre, ct).ConfigureAwait(false), "Profesor creado.");
    }
    public sealed class CrearProfesorValidator : AbstractValidator<CrearProfesorCommand>
    {
        public CrearProfesorValidator() => RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
    }

    public sealed record ActualizarProfesorCommand(int ProfesorId, string Nombre, byte Estado) : IRequest<Respuesta<bool>>;
    public sealed class ActualizarProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<ActualizarProfesorCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(ActualizarProfesorCommand r, CancellationToken ct)
        {
            await repo.ActualizarProfesorAsync(r.ProfesorId, r.Nombre, r.Estado, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Profesor actualizado.");
        }
    }
    public sealed class ActualizarProfesorValidator : AbstractValidator<ActualizarProfesorCommand>
    {
        public ActualizarProfesorValidator()
        {
            RuleFor(x => x.ProfesorId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Estado).InclusiveBetween((byte)0, (byte)1);
        }
    }

    public sealed record EliminarProfesorCommand(int ProfesorId) : IRequest<Respuesta<bool>>;
    public sealed class EliminarProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<EliminarProfesorCommand, Respuesta<bool>>
    {
        public async Task<Respuesta<bool>> Handle(EliminarProfesorCommand r, CancellationToken ct)
        {
            await repo.EliminarProfesorAsync(r.ProfesorId, ct).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Profesor inactivado.");
        }
    }
    public sealed class EliminarProfesorValidator : AbstractValidator<EliminarProfesorCommand>
    {
        public EliminarProfesorValidator() => RuleFor(x => x.ProfesorId).GreaterThan(0);
    }

    public sealed record ObtenerProfesorPorIdQuery(int ProfesorId) : IRequest<Respuesta<ProfesorDto>>;
    public sealed class ObtenerProfesorHandler(IRepositorioAcademico repo) : IRequestHandler<ObtenerProfesorPorIdQuery, Respuesta<ProfesorDto>>
    {
        public async Task<Respuesta<ProfesorDto>> Handle(ObtenerProfesorPorIdQuery r, CancellationToken ct)
        {
            ProfesorDto? d = await repo.ObtenerProfesorPorIdAsync(r.ProfesorId, ct).ConfigureAwait(false);
            if (d is null) throw new KeyNotFoundException("Profesor no encontrado.");
            return Respuesta<ProfesorDto>.Ok(d);
        }
    }

    public sealed record ListarProfesoresQuery(bool SoloActivos = false) : IRequest<Respuesta<IReadOnlyList<ProfesorDto>>>;
    public sealed class ListarProfesoresHandler(IRepositorioAcademico repo) : IRequestHandler<ListarProfesoresQuery, Respuesta<IReadOnlyList<ProfesorDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<ProfesorDto>>> Handle(ListarProfesoresQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<ProfesorDto>>.Ok(await repo.ListarProfesoresAsync(r.SoloActivos, ct).ConfigureAwait(false));
    }
}
