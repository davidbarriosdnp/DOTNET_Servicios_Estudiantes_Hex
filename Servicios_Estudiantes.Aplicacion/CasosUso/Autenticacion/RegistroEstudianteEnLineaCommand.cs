using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Aplicacion.Utilidades;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion
{
    /// <summary>Registro en línea: usuario con rol Estudiante y perfil académico en el programa indicado.</summary>
    public sealed record RegistroEstudianteEnLineaCommand(
        string NombreUsuario,
        string Email,
        string Password,
        string NombreCompleto,
        int ProgramaCreditoId) : IRequest<Respuesta<TokenParDto>>;

    public sealed class RegistroEstudianteEnLineaCommandHandler(
        IRepositorioAcademico repositorioAcademico,
        IRepositorioUsuarios repositorioUsuarios,
        IPasswordHasher<string> passwordHasher,
        IGeneradorTokensJwt generadorTokens)
        : IRequestHandler<RegistroEstudianteEnLineaCommand, Respuesta<TokenParDto>>
    {
        private readonly IRepositorioAcademico _repositorioAcademico = repositorioAcademico;
        private readonly IRepositorioUsuarios _repositorioUsuarios = repositorioUsuarios;
        private readonly IPasswordHasher<string> _passwordHasher = passwordHasher;
        private readonly IGeneradorTokensJwt _generadorTokens = generadorTokens;

        public async Task<Respuesta<TokenParDto>> Handle(RegistroEstudianteEnLineaCommand solicitud, CancellationToken cancellationToken)
        {
            string hash = _passwordHasher.HashPassword(string.Empty, solicitud.Password);
            (int usuarioId, int estudianteId) = await _repositorioAcademico
                .RegistroPublicoEstudianteAsync(
                    solicitud.NombreUsuario,
                    solicitud.Email,
                    hash,
                    solicitud.NombreCompleto,
                    solicitud.ProgramaCreditoId,
                    cancellationToken)
                .ConfigureAwait(false);

            ResultadoEmisionTokenAcceso acceso = _generadorTokens.CrearTokenAcceso(
                usuarioId, solicitud.NombreUsuario, "Estudiante", estudianteId);
            string refreshPlano = _generadorTokens.CrearTokenRenovacion();
            string refreshHash = HashTokenRenovacion.AHexMinuscula(refreshPlano);
            DateTime expiraRefresh = _generadorTokens.CalcularExpiracionRenovacionUtc();
            await _repositorioUsuarios.InsertarRefreshTokenAsync(usuarioId, refreshHash, expiraRefresh, cancellationToken)
                .ConfigureAwait(false);

            int segundos = (int)Math.Max(1, (acceso.ExpiraUtc - DateTime.UtcNow).TotalSeconds);
            return Respuesta<TokenParDto>.Ok(new TokenParDto(acceso.Token, refreshPlano, segundos), "Registro completado. Sesión iniciada.");
        }
    }

    public sealed class RegistroEstudianteEnLineaCommandValidator : AbstractValidator<RegistroEstudianteEnLineaCommand>
    {
        public RegistroEstudianteEnLineaCommandValidator()
        {
            RuleFor(c => c.NombreUsuario).NotEmpty().MaximumLength(120);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(c => c.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
            RuleFor(c => c.NombreCompleto).NotEmpty().MaximumLength(120);
            RuleFor(c => c.ProgramaCreditoId).GreaterThan(0);
        }
    }
}
