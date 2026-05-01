using Microsoft.Data.SqlClient;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Data;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    /// <summary>
    /// Implementación SQL del repositorio académico.
    /// Ejecuta procedimientos almacenados para operaciones CRUD relacionadas con el ámbito académico.
    /// </summary>
    public sealed class RepositorioAcademicoSql : IRepositorioAcademico
    {
        private readonly string _cs;

        /// <summary>
        /// Crea una nueva instancia de <see cref="RepositorioAcademicoSql"/>.
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexión a la base de datos.</param>
        public RepositorioAcademicoSql(string cadenaConexion) =>
            _cs = cadenaConexion ?? throw new ArgumentNullException(nameof(cadenaConexion));

        /// <summary>
        /// Inserta un nuevo programa de crédito y devuelve su id.
        /// </summary>
        public async Task<int> InsertarProgramaCreditoAsync(string nombre, byte creditosPorMateria, byte maxMaterias, CancellationToken ct)
        {
            try
            {
                return await EjecutarConSalidaIntAsync(ct, "dbo.sp_ProgramaCredito_Insertar", "@ProgramaCreditoId",
                    ("@Nombre", nombre), ("@CreditosPorMateria", creditosPorMateria), ("@MaxMateriasPorEstudiante", maxMaterias)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Actualiza un programa de crédito existente.
        /// </summary>
        public async Task ActualizarProgramaCreditoAsync(int id, string nombre, byte creditosPorMateria, byte maxMaterias, byte estado, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_ProgramaCredito_Actualizar",
                    ("@ProgramaCreditoId", id), ("@Nombre", nombre), ("@CreditosPorMateria", creditosPorMateria), ("@MaxMateriasPorEstudiante", maxMaterias), ("@Estado", estado)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Elimina (o marca como eliminado) un programa de crédito.
        /// </summary>
        public async Task EliminarProgramaCreditoAsync(int id, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_ProgramaCredito_Eliminar", ("@ProgramaCreditoId", id)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Obtiene un programa de crédito por su identificador.
        /// </summary>
        public async Task<ProgramaCreditoDto?> ObtenerProgramaCreditoPorIdAsync(int id, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_ProgramaCredito_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProgramaCreditoId", id);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            return !await r.ReadAsync(ct).ConfigureAwait(false) ? null : LeerPrograma(r);
        }

        /// <summary>
        /// Lista los programas de crédito, opcionalmente sólo los activos.
        /// </summary>
        public async Task<IReadOnlyList<ProgramaCreditoDto>> ListarProgramasCreditoAsync(bool soloActivos, CancellationToken ct)
        {
            List<ProgramaCreditoDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_ProgramaCredito_Listar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false)) list.Add(LeerPrograma(r));
            return list;
        }

        /// <summary>
        /// Inserta un profesor y devuelve su id.
        /// </summary>
        public async Task<int> InsertarProfesorAsync(string nombre, CancellationToken ct)
        {
            try { return await EjecutarConSalidaIntAsync(ct, "dbo.sp_Profesor_Insertar", "@ProfesorId", ("@Nombre", nombre)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Actualiza los datos de un profesor.
        /// </summary>
        public async Task ActualizarProfesorAsync(int id, string nombre, byte estado, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Profesor_Actualizar", ("@ProfesorId", id), ("@Nombre", nombre), ("@Estado", estado)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Elimina (o marca como eliminado) un profesor.
        /// </summary>
        public async Task EliminarProfesorAsync(int id, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Profesor_Eliminar", ("@ProfesorId", id)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Obtiene un profesor por su identificador.
        /// </summary>
        public async Task<ProfesorDto?> ObtenerProfesorPorIdAsync(int id, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Profesor_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProfesorId", id);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await r.ReadAsync(ct).ConfigureAwait(false)) return null;
            return new ProfesorDto(r.GetInt32(0), r.GetString(1), r.GetDateTime(2), r.IsDBNull(3) ? null : r.GetDateTime(3), r.GetByte(4));
        }

        /// <summary>
        /// Lista los profesores, opcionalmente sólo los activos.
        /// </summary>
        public async Task<IReadOnlyList<ProfesorDto>> ListarProfesoresAsync(bool soloActivos, CancellationToken ct)
        {
            List<ProfesorDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Profesor_Listar", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false))
                list.Add(new ProfesorDto(r.GetInt32(0), r.GetString(1), r.GetDateTime(2), r.IsDBNull(3) ? null : r.GetDateTime(3), r.GetByte(4)));
            return list;
        }

        /// <summary>
        /// Inserta una materia y devuelve su id.
        /// </summary>
        public async Task<int> InsertarMateriaAsync(string nombre, byte creditos, int profesorId, int programaCreditoId, CancellationToken ct)
        {
            try
            {
                return await EjecutarConSalidaIntAsync(ct, "dbo.sp_Materia_Insertar", "@MateriaId",
                    ("@Nombre", nombre), ("@Creditos", creditos), ("@ProfesorId", profesorId), ("@ProgramaCreditoId", programaCreditoId)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Actualiza una materia existente.
        /// </summary>
        public async Task ActualizarMateriaAsync(int id, string nombre, byte creditos, int profesorId, int programaCreditoId, byte estado, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Materia_Actualizar",
                    ("@MateriaId", id), ("@Nombre", nombre), ("@Creditos", creditos), ("@ProfesorId", profesorId), ("@ProgramaCreditoId", programaCreditoId), ("@Estado", estado)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Elimina (o marca como eliminado) una materia.
        /// </summary>
        public async Task EliminarMateriaAsync(int id, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Materia_Eliminar", ("@MateriaId", id)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Obtiene el detalle de una materia por su id.
        /// </summary>
        public async Task<MateriaDetalleDto?> ObtenerMateriaPorIdAsync(int id, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Materia_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MateriaId", id);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await r.ReadAsync(ct).ConfigureAwait(false)) return null;
            return LeerMateriaDetalleDto(r);
        }

        /// <summary>
        /// Lista materias filtradas por programa y estado.
        /// </summary>
        public async Task<IReadOnlyList<MateriaCatalogoDto>> ListarMateriasPorProgramaAsync(int? programaCreditoId, bool soloActivos, CancellationToken ct)
        {
            List<MateriaCatalogoDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Materia_ListarPorPrograma", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProgramaCreditoId", programaCreditoId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false))
                list.Add(LeerMateriaCatalogoDto(r));

            return list;
        }

        /// <summary>
        /// Inserta un estudiante y devuelve su id.
        /// </summary>
        public async Task<int> InsertarEstudianteAsync(string nombre, string email, int? programaCreditoId, CancellationToken ct)
        {
            try
            {
                return await EjecutarConSalidaIntAsync(ct, "dbo.sp_Estudiante_Insertar", "@EstudianteId",
                    ("@Nombre", nombre), ("@Email", email), ("@ProgramaCreditoId", programaCreditoId ?? (object)DBNull.Value)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Actualiza un estudiante existente.
        /// </summary>
        public async Task ActualizarEstudianteAsync(int id, string nombre, string email, int? programaCreditoId, byte? estado, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Estudiante_Actualizar",
                    ("@EstudianteId", id), ("@Nombre", nombre), ("@Email", email),
                    ("@ProgramaCreditoId", programaCreditoId ?? (object)DBNull.Value), ("@Estado", estado ?? (object)DBNull.Value)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Elimina (o marca como eliminado) un estudiante.
        /// </summary>
        public async Task EliminarEstudianteAsync(int id, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Estudiante_Eliminar", ("@EstudianteId", id)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Obtiene el detalle de un estudiante por su id.
        /// </summary>
        public async Task<EstudianteDetalleDto?> ObtenerEstudiantePorIdAsync(int id, CancellationToken ct)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Estudiante_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@EstudianteId", id);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await r.ReadAsync(ct).ConfigureAwait(false)) return null;
            return new EstudianteDetalleDto(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetInt32(3), r.GetDateTime(4), r.IsDBNull(5) ? null : r.GetDateTime(5), r.GetByte(6));
        }

        /// <summary>
        /// Lista los registros de estudiantes, opcionalmente sólo los activos.
        /// </summary>
        public async Task<IReadOnlyList<EstudianteRegistroDto>> ListarRegistrosEstudiantesAsync(bool soloActivos, CancellationToken ct)
        {
            List<EstudianteRegistroDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Estudiante_ListarRegistros", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false))
            {
                list.Add(new EstudianteRegistroDto(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetInt32(3), r.GetDateTime(4), r.IsDBNull(5) ? null : r.GetDateTime(5), r.GetByte(6), r.GetString(7)));
            }

            return list;
        }

        /// <summary>
        /// Registra la inscripción de un estudiante en tres materias.
        /// </summary>
        public async Task RegistrarInscripcionAsync(int estudianteId, int m1, int m2, int m3, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Inscripcion_Registrar",
                    ("@EstudianteId", estudianteId), ("@MateriaId1", m1), ("@MateriaId2", m2), ("@MateriaId3", m3)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Inserta una fila de inscripción (estudiante-materia).
        /// </summary>
        public async Task InsertarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Inscripcion_InsertarFila", ("@EstudianteId", estudianteId), ("@MateriaId", materiaId)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Elimina una fila de inscripción (estudiante-materia).
        /// </summary>
        public async Task EliminarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct)
        {
            try { await EjecutarSinSalidaAsync(ct, "dbo.sp_Inscripcion_EliminarFila", ("@EstudianteId", estudianteId), ("@MateriaId", materiaId)).ConfigureAwait(false); }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Actualiza la materia inscrita por un estudiante.
        /// </summary>
        public async Task ActualizarInscripcionMateriaAsync(int estudianteId, int materiaAnterior, int materiaNueva, CancellationToken ct)
        {
            try
            {
                await EjecutarSinSalidaAsync(ct, "dbo.sp_Inscripcion_ActualizarMateria",
                    ("@EstudianteId", estudianteId), ("@MateriaIdAnterior", materiaAnterior), ("@MateriaIdNueva", materiaNueva)).ConfigureAwait(false);
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Lista las inscripciones de un estudiante.
        /// </summary>
        public async Task<IReadOnlyList<InscripcionEstudianteDto>> ListarInscripcionPorEstudianteAsync(int estudianteId, bool soloActivas, CancellationToken ct)
        {
            List<InscripcionEstudianteDto> list = [];
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new("dbo.sp_Inscripcion_ObtenerPorEstudiante", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@EstudianteId", estudianteId);
            cmd.Parameters.AddWithValue("@SoloActivas", soloActivas);
            await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await r.ReadAsync(ct).ConfigureAwait(false))
            {
                list.Add(new InscripcionEstudianteDto(r.GetInt32(0), r.GetString(1), r.GetByte(2), r.GetInt32(3), r.GetString(4), r.GetDateTime(5), r.IsDBNull(6) ? null : r.GetDateTime(6), r.GetByte(7)));
            }

            return list;
        }

        /// <summary>
        /// Lista los nombres de compañeros inscritos en una materia, excluyendo al solicitante.
        /// </summary>
        public async Task<IReadOnlyList<string>> ListarNombresCompanerosPorMateriaAsync(int estudianteIdSolicitante, int materiaId, CancellationToken ct)
        {
            try
            {
                List<string> nombres = [];
                await using SqlConnection cn = new(_cs);
                await cn.OpenAsync(ct).ConfigureAwait(false);
                await using SqlCommand cmd = new("dbo.sp_Companeros_ListarNombresPorMateria", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@EstudianteIdSolicitante", estudianteIdSolicitante);
                cmd.Parameters.AddWithValue("@MateriaId", materiaId);
                await using SqlDataReader r = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
                while (await r.ReadAsync(ct).ConfigureAwait(false)) nombres.Add(r.GetString(0));
                return nombres;
            }
            catch (SqlException ex) { LanzarSiNegocio(ex); throw; }
        }

        /// <summary>
        /// Columnas alineadas con <c>dbo.sp_Materia_ObtenerPorId</c> / <c>dbo.sp_Materia_ListarPorPrograma</c>:
        /// 0 MateriaId, 1 Nombre, 2 Creditos, 3 ProfesorId, 4 ProgramaCreditoId, 5 FechaRegistro, 6 FechaModificacion, 7 Estado, 8 NombreProfesor.
        /// </summary>
        /// <summary>
        /// Lee los campos del lector y construye un <see cref="MateriaDetalleDto"/>.
        /// </summary>
        private static MateriaDetalleDto LeerMateriaDetalleDto(SqlDataReader r) =>
            new(
                r.GetInt32(0),
                r.GetString(1),
                r.GetByte(2),
                r.GetInt32(3),
                r.GetInt32(4),
                r.GetDateTime(5),
                r.IsDBNull(6) ? null : r.GetDateTime(6),
                r.GetByte(7),
                r.GetString(8));

        /// <summary>
        /// Convierte una fila leída en un <see cref="MateriaCatalogoDto"/>.
        /// </summary>
        private static MateriaCatalogoDto LeerMateriaCatalogoDto(SqlDataReader r)
        {
            MateriaDetalleDto detalle = LeerMateriaDetalleDto(r);
            return new MateriaCatalogoDto(
                detalle.MateriaId,
                detalle.Nombre,
                detalle.Creditos,
                detalle.ProfesorId,
                detalle.ProgramaCreditoId,
                detalle.NombreProfesor,
                detalle.FechaRegistro,
                detalle.FechaModificacion,
                detalle.Estado);
        }

        /// <summary>
        /// Construye un <see cref="ProgramaCreditoDto"/> desde el lector.
        /// </summary>
        private static ProgramaCreditoDto LeerPrograma(SqlDataReader r) =>
            new(r.GetInt32(0), r.GetString(1), r.GetByte(2), r.GetByte(3), r.GetDateTime(4), r.IsDBNull(5) ? null : r.GetDateTime(5), r.GetByte(6));

        /// <summary>
        /// Ejecuta un procedimiento almacenado sin parámetros de salida.
        /// </summary>
        private async Task EjecutarSinSalidaAsync(CancellationToken ct, string sp, params (string Name, object? Value)[] pars)
        {
            await using SqlConnection cn = new(_cs);
            await cn.OpenAsync(ct).ConfigureAwait(false);
            await using SqlCommand cmd = new(sp, cn) { CommandType = CommandType.StoredProcedure };
            foreach ((string name, object? value) in pars)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y devuelve el valor de un parámetro de salida entero.
        /// </summary>
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

        /// <summary>
        /// Lanza una excepción de aplicación si el <see cref="SqlException"/> corresponde a reglas de negocio.
        /// </summary>
        private static void LanzarSiNegocio(SqlException ex)
        {
            if (ex.Number is >= 50_000 and <= 50_299)
                throw new ExcepcionAplicacion(ex.Message, ex);
            if (ex.Number is 2601 or 2627)
                throw new ExcepcionAplicacion("Violación de unicidad en base de datos.", ex);
        }
    }
}
