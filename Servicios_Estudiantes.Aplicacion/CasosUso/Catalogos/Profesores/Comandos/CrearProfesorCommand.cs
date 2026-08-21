using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Comandos
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

}

