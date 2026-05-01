using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Consultas
{
    public sealed record ListarUsuariosQuery(bool SoloActivos) : IRequest<Respuesta<IReadOnlyList<UsuarioListaDto>>>;

    public sealed class ListarUsuariosQueryHandler(IRepositorioUsuarios repositorio)
        : IRequestHandler<ListarUsuariosQuery, Respuesta<IReadOnlyList<UsuarioListaDto>>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<UsuarioListaDto>>> Handle(ListarUsuariosQuery solicitud, CancellationToken cancellationToken)
        {
            IReadOnlyList<UsuarioListaDto> lista = await _repositorio.ListarUsuariosAsync(solicitud.SoloActivos, cancellationToken).ConfigureAwait(false);
            return Respuesta<IReadOnlyList<UsuarioListaDto>>.Ok(lista);
        }
    }
}
