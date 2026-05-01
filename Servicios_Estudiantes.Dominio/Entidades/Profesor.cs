using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class Profesor
    {
        public int ProfesorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
