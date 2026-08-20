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
            
            return servicios;
        }
    }
}
