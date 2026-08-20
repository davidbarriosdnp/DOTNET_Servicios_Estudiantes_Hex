using System;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public int UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresUtc { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? RevokedUtc { get; set; }
    }
}
