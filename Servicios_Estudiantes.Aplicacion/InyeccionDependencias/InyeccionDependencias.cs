using Servicios_Estudiantes.Aplicacion.Comportamientos;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Servicios_Estudiantes.Aplicacion.InyeccionDependencias
{
    public static class InyeccionDependencias
    {
        public static void AgregarCapaAplicacion(this IServiceCollection servicios)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            servicios.AddMediatR(mediatRConfiguracion =>
            {
                mediatRConfiguracion.RegisterServicesFromAssemblies(assembly);
                mediatRConfiguracion.AddOpenBehavior(typeof(ValidacionComportamiento<,>));
            });

        }
    }
}
