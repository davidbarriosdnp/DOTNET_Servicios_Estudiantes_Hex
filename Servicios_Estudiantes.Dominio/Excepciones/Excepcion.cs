namespace Servicios_Estudiantes.Dominio.Excepciones
{
    /// <summary>
    /// Regla de negocio o invariante de dominio incumplida.
    /// </summary>
    public sealed class Excepcion : Exception
    {
        public Excepcion()
        {
        }

        public Excepcion(string? message) : base(message)
        {
        }

        public Excepcion(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
