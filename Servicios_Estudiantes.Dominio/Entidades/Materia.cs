using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa una materia o asignatura dentro de un programa.
    /// </summary>
    public sealed class Materia
    {
        /// <summary>
        /// Identificador de la materia.
        /// </summary>
        public int MateriaId { get; set; }

        /// <summary>
        /// Nombre de la materia.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Créditos asociados a la materia.
        /// </summary>
        public byte Creditos { get; set; }

        /// <summary>
        /// Identificador del profesor responsable de la materia.
        /// </summary>
        public int ProfesorId { get; set; }

        /// <summary>
        /// Identificador del programa de crédito al que pertenece la materia.
        /// </summary>
        public int ProgramaCreditoId { get; set; }

        /// <summary>
        /// Fecha de registro de la materia.
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
    }
}
