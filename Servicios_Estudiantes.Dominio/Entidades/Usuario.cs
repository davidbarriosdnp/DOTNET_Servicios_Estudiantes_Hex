namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Usuario del sistema (autenticacin y autorizacin).
    /// La persistencia se gestiona va procedimientos almacenados en infraestructura.
    /// </summary>
    public sealed class Usuario
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nombre de usuario para autenticacin.
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrnico del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contrasea.
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
        /// Fecha de ǧltima modificacin.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del usuario (1 activo, 0 inactivo).
        /// </summary>
        public byte Estado { get; set; } = 1;

        // Propiedades de Navegación
        public Estudiante? Estudiante { get; set; }
    }
}
