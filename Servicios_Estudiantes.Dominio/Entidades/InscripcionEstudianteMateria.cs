using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class InscripcionEstudianteMateria
    {
        public int EstudianteId { get; set; }
        public int MateriaId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
