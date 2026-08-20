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
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nombre de usuario para autenticación.
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contraseña.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rol asignado al usuario.
        /// </summary>
        public string Rol { get; set; } = "Estudiante";

        /// <summary>
        /// Fecha de registro del usuario.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Fecha de última modificación.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del usuario (1 activo, 0 inactivo).
        /// </summary>
        public byte Estado { get; set; } = 1;
    }
}
