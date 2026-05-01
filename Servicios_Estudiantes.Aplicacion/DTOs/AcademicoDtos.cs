namespace Servicios_Estudiantes.Aplicacion.DTOs
{
    /// <summary>
    /// DTO que representa un programa de crédito.
    /// </summary>
    public sealed record ProgramaCreditoDto(
        int ProgramaCreditoId,
        string Nombre,
        byte CreditosPorMateria,
        byte MaxMateriasPorEstudiante,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    /// <summary>
    /// DTO que representa un profesor.
    /// </summary>
    public sealed record ProfesorDto(
        int ProfesorId,
        string Nombre,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    /// <summary>
    /// DTO con detalle completo de una materia.
    /// </summary>
    public sealed record MateriaDetalleDto(
        int MateriaId,
        string Nombre,
        byte Creditos,
        int ProfesorId,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        string NombreProfesor);

    /// <summary>
    /// DTO para mostrar una materia en catálogos/listados.
    /// </summary>
    public sealed record MateriaCatalogoDto(
        int MateriaId,
        string Nombre,
        byte Creditos,
        int ProfesorId,
        int ProgramaCreditoId,
        string NombreProfesor,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    /// <summary>
    /// DTO con detalle de estudiante.
    /// </summary>
    public sealed record EstudianteDetalleDto(
        int EstudianteId,
        string Nombre,
        string Email,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

    /// <summary>
    /// DTO usado en listados de estudiantes con materias inscritas.
    /// </summary>
    public sealed record EstudianteRegistroDto(
        int EstudianteId,
        string Nombre,
        string Email,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado,
        string MateriasInscritas);

    /// <summary>
    /// DTO que representa una inscripción de estudiante en una materia.
    /// </summary>
    public sealed record InscripcionEstudianteDto(
        int MateriaId,
        string NombreMateria,
        byte Creditos,
        int ProfesorId,
        string NombreProfesor,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);
}
