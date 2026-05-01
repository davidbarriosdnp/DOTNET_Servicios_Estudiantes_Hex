using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class ProgramaCredito
    {
        public int ProgramaCreditoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public byte CreditosPorMateria { get; set; }
        public byte MaxMateriasPorEstudiante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
