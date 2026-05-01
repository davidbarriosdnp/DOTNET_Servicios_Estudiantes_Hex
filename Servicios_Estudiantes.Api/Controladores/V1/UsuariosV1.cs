using MediatR;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Consultas;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Api.Controladores.V1
{
    public static class UsuariosV1
    {
        public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group)
        {
            group.RequireAuthorization(InyeccionDependenciasAutenticacion.PoliticaSoloAdministrador);

            group.MapGet("", async (IMediator mediator, bool soloActivos, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListarUsuariosQuery(soloActivos), cancellationToken)));

            group.MapGet("{usuarioId:int}", async (IMediator mediator, int usuarioId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ObtenerUsuarioPorIdQuery(usuarioId), cancellationToken)));

            group.MapPost("", async (IMediator mediator, CrearUsuarioCommand comando, CancellationToken cancellationToken) =>
            {
                Respuesta<int> resultado = await mediator.Send(comando, cancellationToken);
                return Results.Created($"/api/v1/Usuarios/{resultado.Resultado}", resultado);
            });

            group.MapPut("{usuarioId:int}", async (
                IMediator mediator,
                int usuarioId,
                ActualizarUsuarioCuerpo cuerpo,
                CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(
                    new ActualizarUsuarioCommand(
                        usuarioId,
                        cuerpo.NombreUsuario,
                        cuerpo.Email,
                        cuerpo.Rol,
                        cuerpo.Estado,
                        cuerpo.NuevaPassword),
                    cancellationToken)));

            group.MapDelete("{usuarioId:int}", async (IMediator mediator, int usuarioId, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new EliminarUsuarioCommand(usuarioId), cancellationToken)));

            return group;
        }
    }

    public sealed record ActualizarUsuarioCuerpo(string NombreUsuario, string Email, string Rol, byte Estado, string? NuevaPassword);
}
