using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Comandos
{
    public sealed record EliminarUsuarioCommand(int UsuarioId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarUsuarioCommandHandler(IRepositorioUsuarios repositorio)
        : IRequestHandler<EliminarUsuarioCommand, Respuesta<bool>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(EliminarUsuarioCommand solicitud, CancellationToken cancellationToken)
        {
            await _repositorio.EliminarUsuarioAsync(solicitud.UsuarioId, cancellationToken).ConfigureAwait(false);
            await _repositorio.RevocarTodosRefreshUsuarioAsync(solicitud.UsuarioId, cancellationToken).ConfigureAwait(false);
            return Respuesta<bool>.Ok(true, "Usuario desactivado y sesiones de renovación revocadas.");
        }
    }

    public sealed class EliminarUsuarioCommandValidator : AbstractValidator<EliminarUsuarioCommand>
    {
        public EliminarUsuarioCommandValidator()
        {
            RuleFor(c => c.UsuarioId).GreaterThan(0);
        }
    }
}
