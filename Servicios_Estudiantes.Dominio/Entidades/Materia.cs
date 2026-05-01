using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Dominio.Entidades
{
    public sealed class Materia
    {
        public int MateriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public byte Creditos { get; set; }
        public int ProfesorId { get; set; }
        public int ProgramaCreditoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
    }
}
