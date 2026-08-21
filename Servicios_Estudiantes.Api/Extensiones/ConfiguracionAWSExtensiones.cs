using Microsoft.Extensions.Configuration;

namespace Servicios_Estudiantes.Api.Extensiones;

public static class ConfiguracionAWSExtensiones
{
    /// <summary>
    /// Configura el proveedor de configuración para leer secretos desde AWS Systems Manager Parameter Store.
    /// </summary>
    public static void AgregarAWSParameterStore(this ConfigurationManager configuration)
    {
        // Se cargan todos los parámetros bajo la ruta base especificada
        configuration.AddSystemsManager("/ServiciosEstudiantes/");
    }
}
