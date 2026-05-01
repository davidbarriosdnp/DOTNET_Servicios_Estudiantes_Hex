using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class Estudiante
    {
        public int EstudianteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ProgramaCreditoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
