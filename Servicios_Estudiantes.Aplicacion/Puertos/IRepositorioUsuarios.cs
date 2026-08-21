using Servicios_Estudiantes.Aplicacion.DTOs;

namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IRepositorioUsuarios
    {
        /// <summary>
        /// Lista los usuarios, opcionalmente solo los activos.
        /// </summary>
        Task<IReadOnlyList<UsuarioListaDto>> ListarUsuariosAsync(bool soloActivos, CancellationToken ct);

        /// <summary>
        /// Obtiene el detalle de un usuario por id.
        /// </summary>
        Task<UsuarioDetalleDto?> ObtenerUsuarioPorIdAsync(int usuarioId, CancellationToken ct);

        /// <summary>
        /// Obtiene las credenciales del usuario por nombre de usuario.
        /// </summary>
        Task<UsuarioCredencialDto?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct);

        /// <summary>
        /// Obtiene las credenciales del usuario por email.
        /// </summary>
        Task<UsuarioCredencialDto?> ObtenerPorEmailAsync(string email, CancellationToken ct);

        /// <summary>
        /// Inserta un usuario y devuelve su id.
        /// </summary>
        Task<int> InsertarUsuarioAsync(string nombreUsuario, string email, string passwordHash, string rol, CancellationToken ct);

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        Task ActualizarUsuarioAsync(int usuarioId, string nombreUsuario, string email, string rol, byte estado, CancellationToken ct);

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        Task ActualizarPasswordAsync(int usuarioId, string passwordHash, CancellationToken ct);

        /// <summary>
        /// Elimina (o marca como eliminado) un usuario.
        /// </summary>
        Task EliminarUsuarioAsync(int usuarioId, CancellationToken ct);
    }
}
