using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Api.Interceptores;
using Servicios_Estudiantes.Api.Seguridad;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Api.Extensiones
{
    public static class InyeccionDependenciasAutenticacion
    {
        public const string PoliticaSoloAdministrador = "SoloAdministrador";

        public static IServiceCollection AgregarAutenticacionJwt(this IServiceCollection servicios, IConfiguration configuracion)
        {
            servicios.Configure<JwtOpciones>(configuracion.GetSection(JwtOpciones.NombreSeccion));

            servicios.AddSingleton<FirmaSymmetricaJwt>();
            servicios.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfiguracionJwtBearerConFirma>();
            servicios.AddSingleton<IAuthorizationMiddlewareResultHandler, ManejoAutorizacionRespuestaJson>();

            servicios.AddMemoryCache();
            servicios.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();
            servicios.AddSingleton<IGeneradorTokensJwt, GeneradorTokensJwt>();
            servicios.AddSingleton<IJwtListaNegra, JwtListaNegraCachada>();

            servicios
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(_ =>
                {
                });

            servicios.AddAuthorization(opciones =>
            {
                opciones.AddPolicy(PoliticaSoloAdministrador, politica => politica.RequireRole("Administrador"));
            });

            return servicios;
        }
    }
}
