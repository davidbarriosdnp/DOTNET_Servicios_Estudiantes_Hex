using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Comandos
{
    public sealed record ActualizarUsuarioCommand(
        int UsuarioId,
        string NombreUsuario,
        string Email,
        string Rol,
        byte Estado,
        string? NuevaPassword) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarUsuarioCommandHandler(IRepositorioUsuarios repositorio, IPasswordHasher<string> passwordHasher)
        : IRequestHandler<ActualizarUsuarioCommand, Respuesta<bool>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;
        private readonly IPasswordHasher<string> _passwordHasher = passwordHasher;

        public async Task<Respuesta<bool>> Handle(ActualizarUsuarioCommand solicitud, CancellationToken cancellationToken)
        {
            UsuarioDetalleDto? actual = await _repositorio.ObtenerUsuarioPorIdAsync(solicitud.UsuarioId, cancellationToken).ConfigureAwait(false);
            if (actual is null)
                throw new KeyNotFoundException($"Usuario {solicitud.UsuarioId} no encontrado.");

            await _repositorio.ActualizarUsuarioAsync(
                solicitud.UsuarioId,
                solicitud.NombreUsuario,
                solicitud.Email,
                solicitud.Rol,
                solicitud.Estado,
                cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(solicitud.NuevaPassword))
            {
                string hash = _passwordHasher.HashPassword(string.Empty, solicitud.NuevaPassword);
                await _repositorio.ActualizarPasswordAsync(solicitud.UsuarioId, hash, cancellationToken).ConfigureAwait(false);
            }

            return Respuesta<bool>.Ok(true, "Usuario actualizado.");
        }
    }

    public sealed class ActualizarUsuarioCommandValidator : AbstractValidator<ActualizarUsuarioCommand>
    {
        public ActualizarUsuarioCommandValidator()
        {
            RuleFor(c => c.UsuarioId).GreaterThan(0);
            RuleFor(c => c.NombreUsuario).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(c => c.Rol).NotEmpty().MaximumLength(64);
            RuleFor(c => c.NuevaPassword)
                .MaximumLength(256)
                .MinimumLength(8)
                .When(c => !string.IsNullOrEmpty(c.NuevaPassword));
        }
    }
}
