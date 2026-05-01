using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa un estudiante inscrito en el sistema.
    /// </summary>
    public sealed class Estudiante
    {
        /// <summary>
        /// Identificador del estudiante.
        /// </summary>
        public int EstudianteId { get; set; }

        /// <summary>
        /// Nombre completo del estudiante.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del estudiante.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del programa de crédito asociado.
        /// </summary>
        public int ProgramaCreditoId { get; set; }

        /// <summary>
        /// Fecha de registro del estudiante.
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
