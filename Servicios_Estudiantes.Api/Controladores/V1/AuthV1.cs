using MediatR;
using Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    /// <summary>
    /// Rutas relacionadas con autenticaciùn (v1).
    /// </summary>
    public static class AuthV1
    {
        /// <summary>
        /// Mappea las rutas de autenticaciùn al grupo proporcionado.
        /// </summary>
        public static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
        {
            group.MapPost("registro", async (IMediator mediator, CuerpoRegistroEstudiante cuerpo, CancellationToken cancellationToken) =>
                    Results.Ok(await mediator.Send(
                        new RegistroEstudianteEnLineaCommand(
                            cuerpo.NombreUsuario,
                            cuerpo.Email,
                            cuerpo.Password,
                            cuerpo.NombreCompleto,
                            cuerpo.ProgramaCreditoId),
                        cancellationToken)))
                .AllowAnonymous();

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

    /// <summary>
    /// Cuerpo de la peticiùn de inicio de sesiùn.
    /// </summary>
    public sealed record CuerpoInicioSesion(string NombreUsuario, string Password);

    /// <summary>
    /// Cuerpo de la peticiùn para refrescar tokens.
    /// </summary>
    public sealed record CuerpoRefrescar(string TokenRenovacion);

    /// <summary>
    /// Cuerpo de la peticiùn para cerrar sesiùn. TokenRenovacion es opcional.
    /// </summary>
    public sealed record CuerpoCerrarSesion(string? TokenRenovacion);

    /// <summary>Registro en lÌnea: credenciales, nombre del estudiante y programa de crÈditos.</summary>
    public sealed record CuerpoRegistroEstudiante(
        string NombreUsuario,
        string Email,
        string Password,
        string NombreCompleto,
        int ProgramaCreditoId);
}
