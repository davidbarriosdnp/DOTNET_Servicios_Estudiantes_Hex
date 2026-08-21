using System;
using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa una materia o asignatura dentro de un programa.
    /// </summary>
    public sealed class Materia
    {
        public int MateriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public byte Creditos { get; set; }
        public int ProfesorId { get; set; }
        public int ProgramaCreditoId { get; set; }
        
        /// <summary>
        /// Aula asignada para la clase (opcional).
        /// </summary>
        public int? AulaId { get; set; }
        
        /// <summary>
        /// Fecha de inicio de las clases.
        /// </summary>
        public DateOnly? FechaInicio { get; set; }
        
        /// <summary>
        /// Fecha de finalización de las clases.
        /// </summary>
        public DateOnly? FechaFin { get; set; }
        
        /// <summary>
        /// Hora de inicio de la clase.
        /// </summary>
        public TimeOnly? HoraInicio { get; set; }
        
        /// <summary>
        /// Hora de finalización de la clase.
        /// </summary>
        public TimeOnly? HoraFin { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;

        // Propiedades de Navegación
        public Profesor Profesor { get; set; } = null!;
        public ProgramaCredito ProgramaCredito { get; set; } = null!;
        public Aula? Aula { get; set; }
        public ICollection<InscripcionEstudianteMateria> Inscripciones { get; set; } = new List<InscripcionEstudianteMateria>();
    }
}
