using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Infraestructura.AccesoDatos;

namespace Servicios_Estudiantes.Infraestructura.InyeccionDependencias
{
    public static class InyeccionDependencia
    {
        public static IServiceCollection AgregarInfraestructura(this IServiceCollection servicios, IConfiguration configuracion)
        {
            string? cadena = configuracion.GetConnectionString("Estudiantes");
            if (string.IsNullOrWhiteSpace(cadena))
                throw new InvalidOperationException("Falta ConnectionStrings:Estudiantes en configuración.");

            servicios.AddSingleton<IRepositorioAcademico>(_ => new RepositorioAcademicoSql(cadena));
            servicios.AddSingleton<IRepositorioUsuarios>(_ => new RepositorioUsuariosSql(cadena));
            return servicios;
        }
    }
}
