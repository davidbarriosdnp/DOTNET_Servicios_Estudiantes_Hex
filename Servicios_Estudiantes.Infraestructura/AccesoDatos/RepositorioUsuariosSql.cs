using Microsoft.Data.SqlClient;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Data;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    public sealed class RepositorioUsuariosSql : IRepositorioUsuarios
    {
        private readonly string _cs;

        public RepositorioUsuariosSql(string cadenaConexion) =>
            _cs = cadenaConexion ?? throw new ArgumentNullException(nameof(cadenaConexion));

        public async Task<IReadOnlyList<UsuarioListaDto>> ListarUsuariosAsync(bool soloActivos, CancellationToken ct)
        {
            List<UsuarioListaDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Usuario_Listar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false)) list.Add(LeerLista(r));
            return list;
        }

        public async Task<UsuarioDetalleDto?> ObtenerUsuarioPorIdAsync(int usuarioId, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Usuario_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            return !await r.ReadAsync(ct).ConfigureAwait(false) ? null : LeerDetalle(r);
        }

        public async Task<UsuarioCredencialDto?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Usuario_ObtenerPorNombreUsuario", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            return !await r.ReadAsync(ct).ConfigureAwait(false) ? null : LeerCredencial(r);
        }

        public async Task<UsuarioCredencialDto?> ObtenerPorEmailAsync(string email, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Usuario_ObtenerPorEmail", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Email", email);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            return !await r.ReadAsync(ct).ConfigureAwait(false) ? null : LeerCredencial(r);
        }

        public async Task<int> InsertarUsuarioAsync(string nombreUsuario, string email, string passwordHash, string rol, CancellationToken ct)
        {
            try
            {
                return await EjecutarConSalidaIntAsync(ct, "dbo.sp_Usuario_Insertar", "@UsuarioId",
                    ("@NombreUsuario", nombreUsuario), ("@Email", email), ("@PasswordHash", passwordHash), ("@Rol", rol)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task ActualizarUsuarioAsync(int usuarioId, string nombreUsuario, string email, string rol, byte estado, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Usuario_Actualizar",
                    ("@UsuarioId", usuarioId), ("@NombreUsuario", nombreUsuario), ("@Email", email), ("@Rol", rol), ("@Estado", estado)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task ActualizarPasswordAsync(int usuarioId, string passwordHash, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Usuario_ActualizarPassword",
                    ("@UsuarioId", usuarioId), ("@PasswordHash", passwordHash)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task EliminarUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Usuario_Eliminar", ("@UsuarioId", usuarioId)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task<int> InsertarRefreshTokenAsync(int usuarioId, string tokenHash, DateTime expiresUtc, CancellationToken ct)
        {
            try
            {
                return await EjecutarConSalidaIntAsync(ct, "dbo.sp_RefreshToken_Insertar", "@RefreshTokenId",
                    ("@UsuarioId", usuarioId), ("@TokenHash", tokenHash), ("@ExpiresUtc", expiresUtc)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task<RefreshTokenValidoDto?> ObtenerRefreshValidoPorHashAsync(string tokenHash, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_RefreshToken_ObtenerPorHash", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@TokenHash", tokenHash);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await r.ReadAsync(ct).ConfigureAwait(false)) return null;
            return new RefreshTokenValidoDto(r.GetInt32(0), r.GetInt32(1), r.GetDateTime(2));
        }

        public async Task RevocarRefreshPorHashAsync(string tokenHash, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_RefreshToken_RevocarPorHash", ("@TokenHash", tokenHash)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        public async Task RevocarTodosRefreshUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_RefreshToken_RevocarTodosUsuario", ("@UsuarioId", usuarioId)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        private static UsuarioListaDto LeerLista(SqlDataReader r) =>
            new(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetDateTime(4), r.IsDBNull(5) ? null : r.GetDateTime(5), r.GetByte(6));

        private static UsuarioDetalleDto LeerDetalle(SqlDataReader r) =>
            new(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetDateTime(4), r.IsDBNull(5) ? null : r.GetDateTime(5), r.GetByte(6));

        private static UsuarioCredencialDto LeerCredencial(SqlDataReader r) =>
            new(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4), r.GetDateTime(5), r.IsDBNull(6) ? null : r.GetDateTime(6), r.GetByte(7));

        private async Task EjecutarSinSalidaAsync(CancellationToken ct, string sp, params (string Name, object? Value)[] pars)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new(sp, cn) { CommandType = CommandType.StoredProcedure };
            foreach ((string name, object? value) in pars)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        private async Task<int> EjecutarConSalidaIntAsync(CancellationToken ct, string sp, string parametroSalida, params (string Name, object? Value)[] pars)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new(sp, cn) { CommandType = CommandType.StoredProcedure };
            foreach ((string name, object? value) in pars)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            SqlParameter salida = new(parametroSalida, SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(salida);
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
            return (int)salida.Value!;
        }

        private static void LanzarSiNegocio(SqlException ex)
        {
            if (ex.Number is >= 50_000 and <= 50_299)
                throw new ExcepcionAplicacion(ex.Message, ex);
            if (ex.Number is 2601 or 2627)
                throw new ExcepcionAplicacion("Violación de unicidad en base de datos (email o nombre de usuario duplicado).", ex);
        }
    }
}
