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

    public sealed record EstudianteDetalleDto(
        int EstudianteId,
        string Nombre,
        string Email,
        int ProgramaCreditoId,
        DateTime FechaRegistro,
        DateTime? FechaModificacion,
        byte Estado);

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
        byte Estado);
}
