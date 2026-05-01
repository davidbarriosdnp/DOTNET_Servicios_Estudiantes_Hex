using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Consultas
{
    public sealed record ObtenerUsuarioPorIdQuery(int UsuarioId) : IRequest<Respuesta<UsuarioDetalleDto>>;

    public sealed class ObtenerUsuarioPorIdQueryHandler(IRepositorioUsuarios repositorio)
        : IRequestHandler<ObtenerUsuarioPorIdQuery, Respuesta<UsuarioDetalleDto>>
    {
        private readonly IRepositorioUsuarios _repositorio = repositorio;

        public async Task<Respuesta<UsuarioDetalleDto>> Handle(ObtenerUsuarioPorIdQuery solicitud, CancellationToken cancellationToken)
        {
            UsuarioDetalleDto? u = await _repositorio.ObtenerUsuarioPorIdAsync(solicitud.UsuarioId, cancellationToken).ConfigureAwait(false);
            if (u is null)
                throw new KeyNotFoundException($"Usuario {solicitud.UsuarioId} no encontrado.");

            return Respuesta<UsuarioDetalleDto>.Ok(u);
        }
    }
}
