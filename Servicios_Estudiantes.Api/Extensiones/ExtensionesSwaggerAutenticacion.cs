using Microsoft.OpenApi.Models;

namespace Servicios_Estudiantes.Api.Extensiones
{
    public static class ExtensionesSwaggerAutenticacion
    {
        public static IServiceCollection AgregarSwaggerConJwt(this IServiceCollection servicios)
        {
            servicios.ConfigureSwaggerGen(opciones =>
            {
                opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT de acceso. Ejemplo: Bearer {token}"
                });

                opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return servicios;
        }
    }
}
