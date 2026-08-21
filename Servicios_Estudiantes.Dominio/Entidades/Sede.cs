using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    /// <summary>
    /// Representa una sede o campus de la institucion.
    /// </summary>
    public sealed class Sede
    {
        public int SedeId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;

        // Propiedades de Navegación
        public ICollection<Aula> Aulas { get; set; } = new List<Aula>();
    }
}
