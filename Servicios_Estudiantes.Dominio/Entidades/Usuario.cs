namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Usuario del sistema (autenticación y autorización).
    /// La persistencia se gestiona vía procedimientos almacenados en infraestructura.
    /// </summary>
    public sealed class Usuario
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int UsuarioId { get; init; }

        /// <summary>
        /// Nombre de usuario para autenticación.
        /// </summary>
        public string NombreUsuario { get; init; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Rol asignado al usuario.
        /// </summary>
        public string Rol { get; init; } = "Estudiante";

        /// <summary>
        /// Estado del usuario (1 activo, 0 inactivo).
        /// </summary>
        public byte Estado { get; init; } = 1;
    }
}
