namespace Servicios_Estudiantes.Aplicacion.DTOs
{
    public sealed record UsuarioListaDto(
        int UsuarioId,
        string NombreUsuario,
        string Email,
        string Rol,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record UsuarioDetalleDto(
        int UsuarioId,
        string NombreUsuario,
        string Email,
        string Rol,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record UsuarioCredencialDto(
        int UsuarioId,
        string NombreUsuario,
        string Email,
        string PasswordHash,
        string Rol,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record RefreshTokenValidoDto(int RefreshTokenId, int UsuarioId, DateTime ExpiresUtc);

    public sealed record TokenParDto(
        string TokenAcceso,
        string TokenRenovacion,
        int ExpiraSegundosAcceso,
        string TipoToken = "Bearer");
}
