using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Comandos
{
    public sealed record CrearUsuarioCommand(string NombreUsuario, string Email, string Password, string Rol) : IRequest<Respuesta<int>>;

    public sealed class CrearUsuarioCommandHandler(IRepositorioUsuarios repositorio, IPasswordHasher<string> passwordHasher)
        : IRequestHandler<CrearUsuarioCommand, Respuesta<int>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;
        private readonly IPasswordHasher<string> _passwordHasher = passwordHasher;

        public async Task<Respuesta<int>> Handle(CrearUsuarioCommand solicitud, CancellationToken cancellationToken)
        {
            string hash = _passwordHasher.HashPassword(string.Empty, solicitud.Password);
            int id = await _repositorio.InsertarUsuarioAsync(
                solicitud.NombreUsuario,
                solicitud.Email,
                hash,
                solicitud.Rol,
                cancellationToken).ConfigureAwait(false);

            return Respuesta<int>.Ok(id, "Usuario creado.");
        }
    }

    public sealed class CrearUsuarioCommandValidator : AbstractValidator<CrearUsuarioCommand>
    {
        public CrearUsuarioCommandValidator()
        {
            RuleFor(c => c.NombreUsuario).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(c => c.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
            RuleFor(c => c.Rol).NotEmpty().MaximumLength(64);
        }
    }
}
