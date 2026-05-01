using Servicios_Estudiantes.Aplicacion.DTOs;

namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IRepositorioUsuarios
    {
        Task<IReadOnlyList<UsuarioListaDto>> ListarUsuariosAsync(bool soloActivos, CancellationToken ct);

        Task<UsuarioDetalleDto?> ObtenerUsuarioPorIdAsync(int usuarioId, CancellationToken ct);

        Task<UsuarioCredencialDto?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct);

        Task<UsuarioCredencialDto?> ObtenerPorEmailAsync(string email, CancellationToken ct);

        Task<int> InsertarUsuarioAsync(string nombreUsuario, string email, string passwordHash, string rol, CancellationToken ct);

        Task ActualizarUsuarioAsync(int usuarioId, string nombreUsuario, string email, string rol, byte estado, CancellationToken ct);

        Task ActualizarPasswordAsync(int usuarioId, string passwordHash, CancellationToken ct);

        Task EliminarUsuarioAsync(int usuarioId, CancellationToken ct);

        Task<int> InsertarRefreshTokenAsync(int usuarioId, string tokenHash, DateTime expiresUtc, CancellationToken ct);

        Task<RefreshTokenValidoDto?> ObtenerRefreshValidoPorHashAsync(string tokenHash, CancellationToken ct);

        Task RevocarRefreshPorHashAsync(string tokenHash, CancellationToken ct);

        Task RevocarTodosRefreshUsuarioAsync(int usuarioId, CancellationToken ct);
    }
}
