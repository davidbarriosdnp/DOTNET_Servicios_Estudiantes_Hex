using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Aplicacion.Utilidades;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion
{
    public sealed record RefrescarTokenCommand(string TokenRenovacion) : IRequest<Respuesta<TokenParDto>>;

    public sealed class RefrescarTokenCommandHandler(IRepositorioUsuarios repositorio, IGeneradorTokensJwt generadorTokens)
        : IRequestHandler<RefrescarTokenCommand, Respuesta<TokenParDto>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;
        private readonly IGeneradorTokensJwt _generadorTokens = generadorTokens;

        public async Task<Respuesta<TokenParDto>> Handle(RefrescarTokenCommand solicitud, CancellationToken cancellationToken)
        {
            string hash = HashTokenRenovacion.AHexMinuscula(solicitud.TokenRenovacion);
            RefreshTokenValidoDto? fila = await _repositorio.ObtenerRefreshValidoPorHashAsync(hash, cancellationToken).ConfigureAwait(false);
            if (fila is null)
                return Respuesta<TokenParDto>.Fail("Token de renovación inválido o expirado.");

            UsuarioDetalleDto? usuario = await _repositorio.ObtenerUsuarioPorIdAsync(fila.UsuarioId, cancellationToken).ConfigureAwait(false);
            if (usuario is null || usuario.Estado != 1)
                return Respuesta<TokenParDto>.Fail("Usuario no disponible.");

            await _repositorio.RevocarRefreshPorHashAsync(hash, cancellationToken).ConfigureAwait(false);

            ResultadoEmisionTokenAcceso acceso = _generadorTokens.CrearTokenAcceso(usuario.UsuarioId, usuario.NombreUsuario, usuario.Rol);
            string refreshPlano = _generadorTokens.CrearTokenRenovacion();
            string nuevoHash = HashTokenRenovacion.AHexMinuscula(refreshPlano);
            DateTime expiraRefresh = _generadorTokens.CalcularExpiracionRenovacionUtc();
            await _repositorio.InsertarRefreshTokenAsync(usuario.UsuarioId, nuevoHash, expiraRefresh, cancellationToken).ConfigureAwait(false);

            int segundos = (int)Math.Max(1, (acceso.ExpiraUtc - DateTime.UtcNow).TotalSeconds);
            return Respuesta<TokenParDto>.Ok(new TokenParDto(acceso.Token, refreshPlano, segundos), "Token renovado.");
        }
    }

    public sealed class RefrescarTokenCommandValidator : AbstractValidator<RefrescarTokenCommand>
    {
        public RefrescarTokenCommandValidator()
        {
            RuleFor(c => c.TokenRenovacion).NotEmpty().MaximumLength(2048);
        }
    }
}
