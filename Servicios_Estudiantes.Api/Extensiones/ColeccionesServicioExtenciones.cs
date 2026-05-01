using Servicios_Estudiantes.Api.Configuraciones;

namespace Servicios_Estudiantes.Api.Extensiones
{
    public static class ColeccionesServicioExtenciones
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, ApiConfiguracion config)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("PoliticaRestrictiva", policy =>
                {
                    policy.WithOrigins(config.OrigenesCORSPermitidos ?? Array.Empty<string>())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });
            return services;
        }
    }
}
