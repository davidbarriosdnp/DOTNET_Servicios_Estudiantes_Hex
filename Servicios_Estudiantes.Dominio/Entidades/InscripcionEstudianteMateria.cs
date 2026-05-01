using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa la inscripción de un estudiante en una materia.
    /// </summary>
    public sealed class InscripcionEstudianteMateria
    {
        /// <summary>
        /// Identificador del estudiante.
        /// </summary>
        public int EstudianteId { get; set; }

        /// <summary>
        /// Identificador de la materia.
        /// </summary>
        public int MateriaId { get; set; }

        /// <summary>
        /// Fecha en que se registró la inscripción.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Fecha de la última modificación, si existe.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del registro (Activo/Inactivo).
        /// </summary>
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
