using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Api.Excepciones;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Api.Seguridad;
using Servicios_Estudiantes.Api.Utilidades;
using Servicios_Estudiantes.Aplicacion.Envoltorios;

namespace Servicios_Estudiantes.Pruebas.Api;

public sealed class SeguridadYUtilidadesApiPruebas
{
    private static JwtOpciones OpcionesValidas() => new()
    {
        Emisor = "issuer-test",
        Audiencia = "aud-test",
        ClaveSecreta = new string('k', 32),
        MinutosValidezAcceso = 15,
        DiasValidezRefresh = 3
    };

    [Fact]
    public void JwtOpciones_Validar_RechazaClaveCorta()
    {
        JwtOpciones mal = new() { Emisor = "e", Audiencia = "a", ClaveSecreta = "corta" };
        Assert.Throws<InvalidOperationException>(() => mal.Validar());
    }

    [Fact]
    public void JwtOpciones_Validar_RechazaEmisorVacio()
    {
        JwtOpciones mal = new() { Emisor = "", Audiencia = "a", ClaveSecreta = new string('x', 32) };
        Assert.Throws<InvalidOperationException>(() => mal.Validar());
    }

    [Fact]
    public void FirmaSymmetricaJwt_ConstruyeCredenciales()
    {
        FirmaSymmetricaJwt firma = new(Options.Create(OpcionesValidas()));
        Assert.NotNull(firma.Clave);
        Assert.NotNull(firma.CredencialesFirma);
        Assert.Equal("issuer-test", firma.Opciones.Emisor);
    }

    [Fact]
    public void GeneradorTokensJwt_EmiteAccesoYRenovacion()
    {
        FirmaSymmetricaJwt firma = new(Options.Create(OpcionesValidas()));
        GeneradorTokensJwt gen = new(firma);

        Servicios_Estudiantes.Aplicacion.Puertos.ResultadoEmisionTokenAcceso acceso =
            gen.CrearTokenAcceso(42, "user42", "Admin");

        Assert.False(string.IsNullOrEmpty(acceso.Token));
        Assert.False(string.IsNullOrEmpty(acceso.Jti));
        Assert.True(acceso.ExpiraUtc > DateTime.UtcNow);

        string refresh = gen.CrearTokenRenovacion();
        Assert.False(string.IsNullOrEmpty(refresh));

        DateTime expRefresh = gen.CalcularExpiracionRenovacionUtc();
        Assert.True(expRefresh > DateTime.UtcNow.AddDays(2));
    }

    [Fact]
    public async Task JwtListaNegraCachada_RegistraYConsulta()
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        JwtListaNegraCachada lista = new(cache);

        await lista.RegistrarRevocacionAsync("abc", DateTime.UtcNow.AddMinutes(5), CancellationToken.None);
        Assert.True(await lista.EstaRevocadoAsync("abc", CancellationToken.None));
    }

    [Fact]
    public async Task JwtListaNegraCachada_ExpiradoPasado_NoRegistra()
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        JwtListaNegraCachada lista = new(cache);

        await lista.RegistrarRevocacionAsync("past", DateTime.UtcNow.AddSeconds(-10), CancellationToken.None);
        Assert.False(await lista.EstaRevocadoAsync("past", CancellationToken.None));
    }

    [Fact]
    public async Task JsonRespuestaEscritorio_EscribeJsonCamelCase()
    {
        DefaultHttpContext ctx = new();
        ctx.Response.Body = new MemoryStream();

        Respuesta<string> modelo = Respuesta<string>.Ok("valor");
        await JsonRespuestaEscritorio.EscribirAsync(ctx.Response, StatusCodes.Status200OK, modelo);

        ctx.Response.Body.Position = 0;
        StreamReader reader = new(ctx.Response.Body);
        string json = await reader.ReadToEndAsync();

        Assert.Contains("operacionExitosa", json);
        Assert.Contains("valor", json);
        Assert.Equal(StatusCodes.Status200OK, ctx.Response.StatusCode);
    }

    [Theory]
    [InlineData(null, "Credenciales o token inválidos.")]
    [InlineData("", "Credenciales o token inválidos.")]
    [InlineData("IDX10517 algo falló", "No se puede validar el token")]
    [InlineData("signature key was not found in repo", "No se puede validar el token")]
    [InlineData("Otro mensaje técnico", "Otro mensaje técnico")]
    public void JsonRespuestaEscritorio_MensajeFirmaJwt_Traduce(string? entrada, string esperadoParcial)
    {
        string resultado = JsonRespuestaEscritorio.MensajeFirmaJwt(entrada);
        Assert.Contains(esperadoParcial, resultado);
    }

    [Fact]
    public void ExcepcionApi_PreservaMensaje()
    {
        ExcepcionApi ex = new("falló api");
        Assert.Equal("falló api", ex.Message);
    }

    [Fact]
    public void AddCustomCors_RegistraPolitica()
    {
        ServiceCollection services = new();
        services.AddCors();
        services.AddLogging();

        ApiConfiguracion cfg = new("", "", "", "", ["http://localhost:3000"]);
        services.AddCustomCors(cfg);

        using ServiceProvider sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<Microsoft.AspNetCore.Cors.Infrastructure.ICorsService>());
    }
}
