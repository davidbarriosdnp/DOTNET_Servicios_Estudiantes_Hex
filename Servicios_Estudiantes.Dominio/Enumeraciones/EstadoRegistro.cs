namespace Servicios_Estudiantes.Dominio.Enumeraciones
{
    /// <summary>
    /// Indica el estado de un registro en el sistema.
    /// </summary>
    public enum EstadoRegistro : byte
    {
        /// <summary>
        /// Registro inactivo.
        /// </summary>
        Inactivo = 0,

        /// <summary>
        /// Registro activo.
        /// </summary>
        Activo = 1
    }
}
