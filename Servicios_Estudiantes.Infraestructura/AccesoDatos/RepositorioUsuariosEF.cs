using Microsoft.EntityFrameworkCore;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Dominio.Entidades;
using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    public sealed class RepositorioUsuariosEF : IRepositorioUsuarios
    {
        private readonly EstudiantesDbContext _context;

        public RepositorioUsuariosEF(EstudiantesDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<UsuarioListaDto>> ListarUsuariosAsync(bool soloActivos, CancellationToken ct)
        {
            var query = _context.Usuarios.AsNoTracking();
            if (soloActivos) query = query.Where(u => u.Estado == (byte)EstadoRegistro.Activo);

            return await query.Select(u => new UsuarioListaDto(
                u.UsuarioId, u.NombreUsuario, u.Email, u.Rol, u.FechaRegistro, u.FechaModificacion, u.Estado))
                .ToListAsync(ct);
        }

        public async Task<UsuarioDetalleDto?> ObtenerUsuarioPorIdAsync(int usuarioId, CancellationToken ct)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.UsuarioId == usuarioId)
                .Select(u => new UsuarioDetalleDto(u.UsuarioId, u.NombreUsuario, u.Email, u.Rol, u.FechaRegistro, u.FechaModificacion, u.Estado))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UsuarioCredencialDto?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.NombreUsuario == nombreUsuario && u.Estado == (byte)EstadoRegistro.Activo)
                .Select(u => new UsuarioCredencialDto(u.UsuarioId, u.NombreUsuario, u.Email, u.PasswordHash, u.Rol, u.FechaRegistro, u.FechaModificacion, u.Estado))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UsuarioCredencialDto?> ObtenerPorEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Email == email && u.Estado == (byte)EstadoRegistro.Activo)
                .Select(u => new UsuarioCredencialDto(u.UsuarioId, u.NombreUsuario, u.Email, u.PasswordHash, u.Rol, u.FechaRegistro, u.FechaModificacion, u.Estado))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> InsertarUsuarioAsync(string nombreUsuario, string email, string passwordHash, string rol, CancellationToken ct)
        {
            var entity = new Usuario
            {
                NombreUsuario = nombreUsuario,
                Email = email,
                PasswordHash = passwordHash,
                Rol = rol,
                FechaRegistro = DateTime.UtcNow,
                Estado = (byte)EstadoRegistro.Activo
            };
            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity.UsuarioId;
        }

        public async Task ActualizarUsuarioAsync(int usuarioId, string nombreUsuario, string email, string rol, byte estado, CancellationToken ct)
        {
            var entity = await _context.Usuarios.FindAsync([usuarioId], ct);
            if (entity != null)
            {
                entity.NombreUsuario = nombreUsuario;
                entity.Email = email;
                entity.Rol = rol;
                entity.Estado = estado;
                entity.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task ActualizarPasswordAsync(int usuarioId, string passwordHash, CancellationToken ct)
        {
            var entity = await _context.Usuarios.FindAsync([usuarioId], ct);
            if (entity != null)
            {
                entity.PasswordHash = passwordHash;
                entity.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task EliminarUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            var entity = await _context.Usuarios.FindAsync([usuarioId], ct);
            if (entity != null)
            {
                entity.Estado = (byte)EstadoRegistro.Inactivo;
                entity.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<int> InsertarRefreshTokenAsync(int usuarioId, string tokenHash, DateTime expiresUtc, CancellationToken ct)
        {
            var entity = new RefreshToken
            {
                UsuarioId = usuarioId,
                TokenHash = tokenHash,
                ExpiresUtc = expiresUtc,
                CreatedUtc = DateTime.UtcNow
            };
            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity.RefreshTokenId;
        }

        public async Task<RefreshTokenValidoDto?> ObtenerRefreshValidoPorHashAsync(string tokenHash, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            return await _context.RefreshTokens
                .AsNoTracking()
                .Where(r => r.TokenHash == tokenHash && r.RevokedUtc == null && r.ExpiresUtc > now)
                .Select(r => new RefreshTokenValidoDto(r.RefreshTokenId, r.UsuarioId, r.ExpiresUtc))
                .FirstOrDefaultAsync(ct);
        }

        public async Task RevocarRefreshPorHashAsync(string tokenHash, CancellationToken ct)
        {
            var token = await _context.RefreshTokens.Where(r => r.TokenHash == tokenHash).FirstOrDefaultAsync(ct);
            if (token != null)
            {
                token.RevokedUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task RevocarTodosRefreshUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UsuarioId == usuarioId && r.RevokedUtc == null)
                .ToListAsync(ct);
                
            var now = DateTime.UtcNow;
            foreach (var t in tokens)
            {
                t.RevokedUtc = now;
            }
            if (tokens.Count > 0)
                await _context.SaveChangesAsync(ct);
        }
    }
}
