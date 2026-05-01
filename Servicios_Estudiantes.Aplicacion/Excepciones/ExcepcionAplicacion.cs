namespace Servicios_Estudiantes.Aplicacion.Excepciones
{
    /// <summary>
    /// Excepción que representa errores a nivel de aplicación o reglas de negocio.
    /// </summary>
    public class ExcepcionAplicacion : Exception
    {
        public ExcepcionAplicacion()
        {
        }

        public ExcepcionAplicacion(string? message) : base(message)
        {
        }

        public ExcepcionAplicacion(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
