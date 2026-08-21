using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Infraestructura.AccesoDatos;

namespace Servicios_Estudiantes.Infraestructura.InyeccionDependencias
{
    public static class InyeccionDependencia
    {
        /// <summary>
        /// Agrega los servicios de infraestructura al contenedor de dependencias.
        /// </summary>
        public static IServiceCollection AgregarInfraestructura(this IServiceCollection servicios, IConfiguration configuracion)
        {
            string? cadena = configuracion.GetConnectionString("Estudiantes");
            if (string.IsNullOrWhiteSpace(cadena))
                throw new InvalidOperationException("Falta ConnectionStrings:Estudiantes en configuración.");

            servicios.AddDbContext<EstudiantesDbContext>(options =>
            {
                options.UseSqlServer(cadena);
            });

            servicios.AddScoped<IRepositorioAcademico, RepositorioAcademicoEF>();
            servicios.AddScoped<IRepositorioUsuarios, RepositorioUsuariosEF>();
            
            // AWS DynamoDB Setup
            var awsOptions = configuracion.GetAWSOptions();
            // Para AWS DynamoDB Local si no hay configuración real. 
            // Si el ServiceURL está vacío usa las credenciales reales o variables de entorno.
            if (configuracion["AWS:DynamoDB:ServiceURL"] != null)
            {
                awsOptions.DefaultClientConfig.ServiceURL = configuracion["AWS:DynamoDB:ServiceURL"];
            }
            servicios.AddDefaultAWSOptions(awsOptions);
            servicios.AddAWSService<Amazon.DynamoDBv2.IAmazonDynamoDB>();
            servicios.AddScoped<IRepositorioTokens, RepositorioTokensDynamoDB>();

            return servicios;
        }
    }
}
