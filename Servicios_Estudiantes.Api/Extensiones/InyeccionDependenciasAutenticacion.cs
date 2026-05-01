using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Servicios_Estudiantes.Api.Configuraciones;
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

            JwtOpciones jwtInicial = configuracion.GetSection(JwtOpciones.NombreSeccion).Get<JwtOpciones>() ?? new JwtOpciones();
            jwtInicial.Validar();

            servicios.AddMemoryCache();
            servicios.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();
            servicios.AddSingleton<IGeneradorTokensJwt, GeneradorTokensJwt>();
            servicios.AddSingleton<IJwtListaNegra, JwtListaNegraCachada>();

            servicios
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones =>
                {
                    opciones.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtInicial.Emisor,
                        ValidAudience = jwtInicial.Audiencia,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtInicial.ClaveSecreta)),
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };

                    opciones.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async contexto =>
                        {
                            string? jti = contexto.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                                ?? contexto.Principal?.FindFirst("jti")?.Value;
                            if (string.IsNullOrEmpty(jti)) return;

                            IJwtListaNegra lista = contexto.HttpContext.RequestServices.GetRequiredService<IJwtListaNegra>();
                            if (await lista.EstaRevocadoAsync(jti, contexto.HttpContext.RequestAborted).ConfigureAwait(false))
                                contexto.Fail("Token de acceso revocado.");
                        }
                    };
                });

            servicios.AddAuthorization(opciones =>
            {
                opciones.AddPolicy(PoliticaSoloAdministrador, politica => politica.RequireRole("Administrador"));
            });

            return servicios;
        }
    }
}
