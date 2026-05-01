using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Api.Excepciones;
using Servicios_Estudiantes.Api.Seguridad;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Pruebas.Api;

public sealed class ConfiguracionExtensionesPruebas
{
    private static JwtOpciones OpcionesJwtValidas() => new()
    {
        Emisor = "issuer-x",
        Audiencia = "aud-x",
        ClaveSecreta = new string('z', 32),
        MinutosValidezAcceso = 20,
        DiasValidezRefresh = 2
    };

    [Fact]
    public async Task ConfiguracionJwtBearer_OnTokenValidated_JtiEnListaNegra_FallaAutenticacion()
    {
        FirmaSymmetricaJwt firma = new(Options.Create(OpcionesJwtValidas()));
        ConfiguracionJwtBearerConFirma sut = new(firma);

        JwtBearerOptions opcionesBearer = new();
        sut.PostConfigure(JwtBearerDefaults.AuthenticationScheme, opcionesBearer);

        ServiceCollection servicios = new();
        servicios.AddMemoryCache();
        servicios.AddSingleton<IJwtListaNegra, JwtListaNegraCachada>();
        await using ServiceProvider proveedor = servicios.BuildServiceProvider();

        DefaultHttpContext http = new() { RequestServices = proveedor };

        AuthenticationScheme esquema = new(JwtBearerDefaults.AuthenticationScheme, "jwt", typeof(JwtBearerHandler));

        ClaimsPrincipal principal = new(new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Jti, "revocar-este")]));

        TokenValidatedContext contexto = new(http, esquema, opcionesBearer) { Principal = principal };

        IJwtListaNegra lista = proveedor.GetRequiredService<IJwtListaNegra>();
        await lista.RegistrarRevocacionAsync("revocar-este", DateTime.UtcNow.AddMinutes(10), CancellationToken.None);

        Assert.NotNull(opcionesBearer.Events?.OnTokenValidated);
        await opcionesBearer.Events.OnTokenValidated(contexto);

        Assert.NotNull(contexto.Result);
        Assert.False(contexto.Result.Succeeded);
    }

    [Fact]
    public void ConfiguracionJwtBearerConFirma_PostConfigure_SoloEsquemaBearer()
    {
        FirmaSymmetricaJwt firma = new(Options.Create(OpcionesJwtValidas()));
        ConfiguracionJwtBearerConFirma sut = new(firma);

        JwtBearerOptions ignorado = new();
        sut.PostConfigure("Cookie", ignorado);
        Assert.Null(ignorado.TokenValidationParameters?.ValidIssuer);

        JwtBearerOptions bearer = new();
        sut.PostConfigure(JwtBearerDefaults.AuthenticationScheme, bearer);

        Assert.Equal("issuer-x", bearer.TokenValidationParameters!.ValidIssuer);
        Assert.Equal("aud-x", bearer.TokenValidationParameters.ValidAudience);
        Assert.NotNull(bearer.Events?.OnTokenValidated);
    }

    [Fact]
    public void InstanciarConfiguracionApi_Desarrollo_LeeSeccionApi()
    {
        IConfiguration configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Api:NombreServicio"] = "ApiPrueba",
                    ["Api:OrigenesCorsPermitidos"] = "http://localhost:1111, https://other.dev"
                })
            .Build();

        ServiceCollection services = new();
        Mock<IWebHostEnvironment> entorno = new();
        entorno.Setup(e => e.EnvironmentName).Returns("Development");

        ApiConfiguracion api = services.InstanciarConfiguracionApi(configuracion, entorno.Object);

        Assert.Equal("ApiPrueba", api.NombreServicio);
        Assert.Equal(2, api.OrigenesCORSPermitidos.Length);
    }

    [Fact]
    public void InstanciarConfiguracionApi_Produccion_FaltaOtel_LanzaVariableApiNoConfigurada()
    {
        LimpiarVariablesOtel();

        try
        {
            IConfiguration configuracion = new ConfigurationBuilder().Build();
            ServiceCollection services = new();
            Mock<IWebHostEnvironment> entorno = new();
            entorno.Setup(e => e.EnvironmentName).Returns("Production");

            VariableApiNoConfigurada ex = Assert.Throws<VariableApiNoConfigurada>(() =>
                services.InstanciarConfiguracionApi(configuracion, entorno.Object));
            Assert.NotNull(ex.Message);
        }
        finally
        {
            LimpiarVariablesOtel();
        }
    }

    [Fact]
    public void InstanciarConfiguracionApi_Produccion_ConVariables_Completa()
    {
        LimpiarVariablesOtel();

        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_LOGS", "http://otel/logs");
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_TRACES", "http://otel/traces");
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "x=y");
            Environment.SetEnvironmentVariable("NOMBRE_SERVICIO", "svc-prod");
            Environment.SetEnvironmentVariable("ORIGENES_CORS_PERMITIDOS", "https://app.dev");

            IConfiguration configuracion = new ConfigurationBuilder().Build();
            ServiceCollection services = new();
            Mock<IWebHostEnvironment> entorno = new();
            entorno.Setup(e => e.EnvironmentName).Returns("Production");

            ApiConfiguracion api = services.InstanciarConfiguracionApi(configuracion, entorno.Object);

            Assert.Equal("svc-prod", api.NombreServicio);
            Assert.Single(api.OrigenesCORSPermitidos);
            Assert.Equal("http://otel/logs", api.OtelLogsEndpoint);
        }
        finally
        {
            LimpiarVariablesOtel();
        }
    }

    [Fact]
    public void VariableApiNoConfigurada_FormateaNombre()
    {
        VariableApiNoConfigurada ex = new("MiVariable");
        Assert.Contains("MiVariable", ex.Message);
    }

    private static void LimpiarVariablesOtel()
    {
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_LOGS", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_TRACES", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", null);
        Environment.SetEnvironmentVariable("NOMBRE_SERVICIO", null);
        Environment.SetEnvironmentVariable("ORIGENES_CORS_PERMITIDOS", null);
    }
}
