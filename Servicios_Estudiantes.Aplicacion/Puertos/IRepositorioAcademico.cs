using Servicios_Estudiantes.Aplicacion.DTOs;

namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IRepositorioAcademico
    {
        /// <summary>
        /// Inserta un programa de crédito y devuelve su id.
        /// </summary>
        Task<int> InsertarProgramaCreditoAsync(string nombre, byte creditosPorMateria, byte maxMaterias, CancellationToken ct);
        Task ActualizarProgramaCreditoAsync(int id, string nombre, byte creditosPorMateria, byte maxMaterias, byte estado, CancellationToken ct);
        Task EliminarProgramaCreditoAsync(int id, CancellationToken ct);
        Task<ProgramaCreditoDto?> ObtenerProgramaCreditoPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<ProgramaCreditoDto>> ListarProgramasCreditoAsync(bool soloActivos, CancellationToken ct);

        /// <summary>
        /// Inserta un profesor y devuelve su id.
        /// </summary>
        Task<int> InsertarProfesorAsync(string nombre, CancellationToken ct);
        Task ActualizarProfesorAsync(int id, string nombre, byte estado, CancellationToken ct);
        Task EliminarProfesorAsync(int id, CancellationToken ct);
        Task<ProfesorDto?> ObtenerProfesorPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<ProfesorDto>> ListarProfesoresAsync(bool soloActivos, CancellationToken ct);

        /// <summary>
        /// Inserta una materia y devuelve su id.
        /// </summary>
        Task<int> InsertarMateriaAsync(string nombre, byte creditos, int profesorId, int programaCreditoId, CancellationToken ct);
        Task ActualizarMateriaAsync(int id, string nombre, byte creditos, int profesorId, int programaCreditoId, byte estado, CancellationToken ct);
        Task EliminarMateriaAsync(int id, CancellationToken ct);
        Task<MateriaDetalleDto?> ObtenerMateriaPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<MateriaCatalogoDto>> ListarMateriasPorProgramaAsync(int? programaCreditoId, bool soloActivos, CancellationToken ct);

        /// <summary>
        /// Inserta un estudiante y devuelve su id.
        /// </summary>
        Task<int> InsertarEstudianteAsync(string nombre, string email, int? programaCreditoId, int? usuarioId, CancellationToken ct);

        /// <summary>Registro público: crea usuario (rol Estudiante) y estudiante en una transacción.</summary>
        Task<(int UsuarioId, int EstudianteId)> RegistroPublicoEstudianteAsync(
            string nombreUsuario,
            string email,
            string passwordHash,
            string nombreCompleto,
            int programaCreditoId,
            CancellationToken ct);

        /// <summary>Devuelve el id de estudiante activo vinculado al usuario, o null.</summary>
        Task<int?> ObtenerEstudianteIdPorUsuarioAsync(int usuarioId, CancellationToken ct);
        Task ActualizarEstudianteAsync(int id, string nombre, string email, int? programaCreditoId, byte? estado, CancellationToken ct);
        Task EliminarEstudianteAsync(int id, CancellationToken ct);
        Task<EstudianteDetalleDto?> ObtenerEstudiantePorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<EstudianteRegistroDto>> ListarRegistrosEstudiantesAsync(bool soloActivos, CancellationToken ct);

        /// <summary>
        /// Registra la inscripción de un estudiante en tres materias.
        /// </summary>
        Task RegistrarInscripcionAsync(int estudianteId, int m1, int m2, int m3, CancellationToken ct);
        Task InsertarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct);
        Task EliminarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct);
        Task ActualizarInscripcionMateriaAsync(int estudianteId, int materiaAnterior, int materiaNueva, CancellationToken ct);
        Task<IReadOnlyList<InscripcionEstudianteDto>> ListarInscripcionPorEstudianteAsync(int estudianteId, bool soloActivas, CancellationToken ct);

        /// <summary>
        /// Lista los nombres de compañeros inscritos en una materia, excluyendo al solicitante.
        /// </summary>
        Task<IReadOnlyList<string>> ListarNombresCompanerosPorMateriaAsync(int estudianteIdSolicitante, int materiaId, CancellationToken ct);
    }
}
