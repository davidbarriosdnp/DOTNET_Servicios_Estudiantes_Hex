using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Aplicacion.Utilidades;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion
{
    /// <summary>
    /// Solicitud para iniciar sesión con nombre de usuario y contraseña.
    /// </summary>
    public sealed record IniciarSesionCommand(string NombreUsuario, string Password) : IRequest<Respuesta<TokenParDto>>;

    /// <summary>
    /// Manejador de la solicitud de inicio de sesión.
    /// </summary>
    public sealed class IniciarSesionCommandHandler(
        IRepositorioUsuarios repositorio,
        IRepositorioAcademico repositorioAcademico,
        IPasswordHasher<string> passwordHasher,
        IGeneradorTokensJwt generadorTokens)
        : IRequestHandler<IniciarSesionCommand, Respuesta<TokenParDto>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;
        private readonly IRepositorioAcademico _repositorioAcademico = repositorioAcademico;
        private readonly IPasswordHasher<string> _passwordHasher = passwordHasher;
        private readonly IGeneradorTokensJwt _generadorTokens = generadorTokens;

        /// <summary>
        /// Maneja la solicitud, valida credenciales y emite tokens si corresponde.
        /// </summary>
        public async Task<Respuesta<TokenParDto>> Handle(IniciarSesionCommand solicitud, CancellationToken cancellationToken)
        {
            UsuarioCredencialDto? usuario = await _repositorio
                .ObtenerPorNombreUsuarioAsync(solicitud.NombreUsuario, cancellationToken)
                .ConfigureAwait(false);

            if (usuario is null || usuario.Estado != 1)
                return Respuesta<TokenParDto>.Fail("Credenciales inválidas.");

            PasswordVerificationResult verificacion = _passwordHasher.VerifyHashedPassword(string.Empty, usuario.PasswordHash, solicitud.Password);
            if (verificacion is not (PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded))
                return Respuesta<TokenParDto>.Fail("Credenciales inválidas.");

            if (verificacion == PasswordVerificationResult.SuccessRehashNeeded)
            {
                string nuevoHash = _passwordHasher.HashPassword(string.Empty, solicitud.Password);
                await _repositorio.ActualizarPasswordAsync(usuario.UsuarioId, nuevoHash, cancellationToken).ConfigureAwait(false);
            }

            int? estudianteId = await _repositorioAcademico
                .ObtenerEstudianteIdPorUsuarioAsync(usuario.UsuarioId, cancellationToken)
                .ConfigureAwait(false);
            ResultadoEmisionTokenAcceso acceso = _generadorTokens.CrearTokenAcceso(
                usuario.UsuarioId, usuario.NombreUsuario, usuario.Rol, estudianteId);
            string refreshPlano = _generadorTokens.CrearTokenRenovacion();
            string refreshHash = HashTokenRenovacion.AHexMinuscula(refreshPlano);
            DateTime expiraRefresh = _generadorTokens.CalcularExpiracionRenovacionUtc();

            await _repositorio.InsertarRefreshTokenAsync(usuario.UsuarioId, refreshHash, expiraRefresh, cancellationToken).ConfigureAwait(false);

            int segundos = (int)Math.Max(1, (acceso.ExpiraUtc - DateTime.UtcNow).TotalSeconds);
            TokenParDto par = new(acceso.Token, refreshPlano, segundos);
            return Respuesta<TokenParDto>.Ok(par, "Sesión iniciada.");
        }
    }

    /// <summary>
    /// Validador para la solicitud de inicio de sesión.
    /// </summary>
    public sealed class IniciarSesionCommandValidator : AbstractValidator<IniciarSesionCommand>
    {
        public IniciarSesionCommandValidator()
        {
            RuleFor(c => c.NombreUsuario).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Password).NotEmpty().MaximumLength(256);
        }
    }
}
