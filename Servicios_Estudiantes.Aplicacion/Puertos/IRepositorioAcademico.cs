using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Servicios_Estudiantes.Aplicacion.DTOs;

namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IRepositorioAcademico
    {
        Task<IReadOnlyList<SedeDto>> ListarSedesAsync(CancellationToken ct);
        Task<IReadOnlyList<AulaDto>> ListarAulasAsync(CancellationToken ct);

        Task<int> InsertarProgramaCreditoAsync(string nombre, byte creditosPorMateria, byte maxMaterias, CancellationToken ct);
        Task ActualizarProgramaCreditoAsync(int id, string nombre, byte creditosPorMateria, byte maxMaterias, byte estado, CancellationToken ct);
        Task EliminarProgramaCreditoAsync(int id, CancellationToken ct);
        Task<ProgramaCreditoDto?> ObtenerProgramaCreditoPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<ProgramaCreditoDto>> ListarProgramasCreditoAsync(bool soloActivos, CancellationToken ct);

        Task<int> InsertarProfesorAsync(string nombre, CancellationToken ct);
        Task ActualizarProfesorAsync(int id, string nombre, byte estado, CancellationToken ct);
        Task EliminarProfesorAsync(int id, CancellationToken ct);
        Task<ProfesorDto?> ObtenerProfesorPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<ProfesorDto>> ListarProfesoresAsync(bool soloActivos, CancellationToken ct);

        Task<int> InsertarMateriaAsync(string nombre, byte creditos, int profesorId, int programaCreditoId, int? aulaId, DateOnly? fechaInicio, DateOnly? fechaFin, TimeOnly? horaInicio, TimeOnly? horaFin, CancellationToken ct);
        Task ActualizarMateriaAsync(int id, string nombre, byte creditos, int profesorId, int programaCreditoId, int? aulaId, DateOnly? fechaInicio, DateOnly? fechaFin, TimeOnly? horaInicio, TimeOnly? horaFin, byte estado, CancellationToken ct);
        Task EliminarMateriaAsync(int id, CancellationToken ct);
        Task<MateriaDetalleDto?> ObtenerMateriaPorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<MateriaCatalogoDto>> ListarMateriasPorProgramaAsync(int? programaCreditoId, bool soloActivos, CancellationToken ct);

        Task<int> InsertarEstudianteAsync(string nombre, string email, int? programaCreditoId, int? usuarioId, CancellationToken ct);
        Task<(int UsuarioId, int EstudianteId)> RegistroPublicoEstudianteAsync(string nombreUsuario, string email, string passwordHash, string nombreCompleto, int programaCreditoId, CancellationToken ct);
        Task<int?> ObtenerEstudianteIdPorUsuarioAsync(int usuarioId, CancellationToken ct);
        Task ActualizarEstudianteAsync(int id, string nombre, string email, int? programaCreditoId, byte? estado, CancellationToken ct);
        Task EliminarEstudianteAsync(int id, CancellationToken ct);
        Task<EstudianteDetalleDto?> ObtenerEstudiantePorIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<EstudianteRegistroDto>> ListarRegistrosEstudiantesAsync(bool soloActivos, CancellationToken ct);

        Task RegistrarInscripcionAsync(int estudianteId, int? m1, int? m2, int? m3, CancellationToken ct);
        Task InsertarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct);
        Task EliminarInscripcionFilaAsync(int estudianteId, int materiaId, CancellationToken ct);
        Task ActualizarInscripcionMateriaAsync(int estudianteId, int materiaAnterior, int materiaNueva, CancellationToken ct);
        Task<IReadOnlyList<InscripcionEstudianteDto>> ListarInscripcionPorEstudianteAsync(int estudianteId, bool soloActivas, CancellationToken ct);

        Task<IReadOnlyList<string>> ListarNombresCompanerosPorMateriaAsync(int estudianteIdSolicitante, int materiaId, CancellationToken ct);
    }
}
