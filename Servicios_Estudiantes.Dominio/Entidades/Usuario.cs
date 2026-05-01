namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Usuario del sistema (autenticación y autorización).
    /// La persistencia se gestiona vía procedimientos almacenados en infraestructura.
    /// </summary>
    public sealed class Usuario
    {
        public int UsuarioId { get; init; }
        public string NombreUsuario { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Rol { get; init; } = "Estudiante";
        public byte Estado { get; init; } = 1;
    }
}
