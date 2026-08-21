using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa a un profesor del sistema.
    /// </summary>
    public sealed class Profesor
    {
        /// <summary>
        /// Identificador del profesor.
        /// </summary>
        public int ProfesorId { get; set; }

        /// <summary>
        /// Nombre completo del profesor.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Fecha en que se registró el profesor.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Fecha de la última modificación del registro, si existe.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del registro (Activo/Inactivo).
        /// </summary>
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;

        // Propiedades de Navegación
        public ICollection<Materia> Materias { get; set; } = new List<Materia>();
    }
}
