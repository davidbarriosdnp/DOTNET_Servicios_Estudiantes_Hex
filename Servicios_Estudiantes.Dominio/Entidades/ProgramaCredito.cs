using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa un programa académico basado en créditos.
    /// Contiene información sobre créditos por materia y restricciones de carga.
    /// </summary>
    public sealed class ProgramaCredito
    {
        /// <summary>
        /// Identificador del programa de crédito.
        /// </summary>
        public int ProgramaCreditoId { get; set; }

        /// <summary>
        /// Nombre del programa de crédito.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Créditos asignados por cada materia en el programa.
        /// </summary>
        public byte CreditosPorMateria { get; set; }

        /// <summary>
        /// Número máximo de materias que un estudiante puede matricular en el programa.
        /// </summary>
        public byte MaxMateriasPorEstudiante { get; set; }

        /// <summary>
        /// Fecha en que se registró el programa.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Fecha de la última modificación del programa, si existe.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Estado del registro (Activo/Inactivo).
        /// </summary>
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
