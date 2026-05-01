using MediatR;
using Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class AuthV1
    {
        public static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
        {
            group.MapPost("login", async (IMediator mediator, CuerpoInicioSesion cuerpo, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new IniciarSesionCommand(cuerpo.NombreUsuario, cuerpo.Password), cancellationToken)));

            group.MapPost("refresh", async (IMediator mediator, CuerpoRefrescar cuerpo, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new RefrescarTokenCommand(cuerpo.TokenRenovacion), cancellationToken)));

            group.MapPost("logout", async (HttpContext contexto, IMediator mediator, CuerpoCerrarSesion? cuerpo, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(
                    new CerrarSesionCommand(
                        contexto.Request.Headers.Authorization.ToString(),
                        cuerpo?.TokenRenovacion),
                    cancellationToken))).RequireAuthorization();

            return group;
        }
    }

    public sealed record CuerpoInicioSesion(string NombreUsuario, string Password);

    public sealed record CuerpoRefrescar(string TokenRenovacion);

    public sealed record CuerpoCerrarSesion(string? TokenRenovacion);
}
