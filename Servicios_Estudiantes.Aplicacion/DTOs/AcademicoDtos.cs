using System;

namespace Servicios_Estudiantes.Aplicacion.DTOs
{
    public sealed record ProgramaCreditoDto(
        int ProgramaCreditoId,
        string Nombre,
        byte CreditosPorMateria,
        byte MaxMateriasPorEstudiante,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record ProfesorDto(
        int ProfesorId,
        string Nombre,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record SedeDto(
        int SedeId,
        string Nombre,
        string Direccion,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record AulaDto(
        int AulaId,
        string Nombre,
        int Capacidad,
        int SedeId,
        string NombreSede,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    public sealed record MateriaDetalleDto(
        int MateriaId,
        string Nombre,
        byte Creditos,
        int ProfesorId,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        string NombreProfesor,
        int? AulaId,
        DateOnly? FechaInicio,
        DateOnly? FechaFin,
        TimeOnly? HoraInicio,
        TimeOnly? HoraFin);

    public sealed record MateriaCatalogoDto(
        int MateriaId,
        string Nombre,
        byte Creditos,
        int ProfesorId,
        int ProgramaCreditoId,
        string NombreProfesor,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        int? AulaId,
        string NombreAula,
        string NombreSede,
        DateOnly? FechaInicio,
        DateOnly? FechaFin,
        TimeOnly? HoraInicio,
        TimeOnly? HoraFin);

    public sealed record EstudianteDetalleDto(
        int EstudianteId,
        string Nombre,
        string Email,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        int? UsuarioId);

    public sealed record EstudianteRegistroDto(
        int EstudianteId,
        string Nombre,
        string Email,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        string MateriasInscritas);

    public sealed record InscripcionEstudianteDto(
        int MateriaId,
        string NombreMateria,
        byte Creditos,
        int ProfesorId,
        string NombreProfesor,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        int? AulaId,
        string NombreAula,
        string NombreSede,
        DateOnly? FechaInicio,
        DateOnly? FechaFin,
        TimeOnly? HoraInicio,
        TimeOnly? HoraFin);
}
