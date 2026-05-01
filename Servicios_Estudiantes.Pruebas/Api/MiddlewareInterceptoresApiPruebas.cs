using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Servicios_Estudiantes.Api.Excepciones;
using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Api.Interceptores;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Dominio.Excepciones;
using DominioEx = Servicios_Estudiantes.Dominio.Excepciones.Excepcion;

namespace Servicios_Estudiantes.Pruebas.Api;

public sealed class MiddlewareInterceptoresApiPruebas
{
    private sealed class RequisitoMarcador : IAuthorizationRequirement { }

    private static DefaultHttpContext CrearContexto()
    {
        DefaultHttpContext ctx = new();
        ctx.Response.Body = new MemoryStream();
        return ctx;
    }

    private static async Task<string> LeerCuerpoRespuesta(HttpContext ctx)
    {
        ctx.Response.Body.Position = 0;
        StreamReader reader = new(ctx.Response.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task ManejadorErroresIntermediario_ExcepcionApi_400()
    {
        DefaultHttpContext ctx = CrearContexto();
        RequestDelegate siguiente = _ => throw new ExcepcionApi("negocio");

        ManejadorErroresIntermediario sut = new(siguiente, NullLogger<ManejadorErroresIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
        string json = await LeerCuerpoRespuesta(ctx);
        Assert.Contains("negocio", json);
    }

    [Fact]
    public async Task ManejadorErroresIntermediario_ExcepcionValidacion_IncluyeErrores()
    {
        DefaultHttpContext ctx = CrearContexto();
        ExcepcionValidacion err = new([new ValidationFailure("campo", "mensaje")]);
        RequestDelegate siguiente = _ => throw err;

        ManejadorErroresIntermediario sut = new(siguiente, NullLogger<ManejadorErroresIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
        string json = await LeerCuerpoRespuesta(ctx);
        Assert.Contains("mensaje", json);
    }

    [Fact]
    public async Task ManejadorErroresIntermediario_KeyNotFound_404()
    {
        DefaultHttpContext ctx = CrearContexto();
        RequestDelegate siguiente = _ => throw new KeyNotFoundException("no existe");

        ManejadorErroresIntermediario sut = new(siguiente, NullLogger<ManejadorErroresIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status404NotFound, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task ManejadorErroresIntermediario_Dominio_400()
    {
        DefaultHttpContext ctx = CrearContexto();
        RequestDelegate siguiente = _ => throw new DominioEx("regla");

        ManejadorErroresIntermediario sut = new(siguiente, NullLogger<ManejadorErroresIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task ManejadorErroresIntermediario_NoControlado_500()
    {
        DefaultHttpContext ctx = CrearContexto();
        RequestDelegate siguiente = _ => throw new InvalidOperationException("fallo", new Exception("inner"));

        ManejadorErroresIntermediario sut = new(siguiente, NullLogger<ManejadorErroresIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status500InternalServerError, ctx.Response.StatusCode);
        string json = await LeerCuerpoRespuesta(ctx);
        Assert.Contains("inner", json);
    }

    [Fact]
    public async Task BuferDeRespuesta_EscribeYLeeAntesDeVaciar()
    {
        DefaultHttpContext ctx = new();
        MemoryStream backend = new();
        ctx.Response.Body = backend;

        await using (await BuferDeRespuesta.BeginAsync(ctx))
        {
            await ctx.Response.WriteAsync("payload");
            string capturado = BuferDeRespuesta.GetBodyAsString(ctx);
            Assert.Contains("payload", capturado);
        }

        backend.Position = 0;
        StreamReader reader = new(backend);
        Assert.Contains("payload", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task MalaPeticionLogeadorIntermediario_Status400_Registra()
    {
        DefaultHttpContext ctx = new();
        ctx.Response.Body = new MemoryStream();
        ctx.Request.Method = "POST";
        ctx.Request.Scheme = "http";
        ctx.Request.Host = new HostString("localhost");
        ctx.Request.Path = "/x";
        ctx.Request.ContentType = "application/json";
        ctx.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("{}"));

        RequestDelegate siguiente = async c =>
        {
            c.Response.StatusCode = StatusCodes.Status400BadRequest;
            await c.Response.WriteAsync("detalle-error");
        };

        MalaPeticionLogeadorIntermediario sut = new(siguiente, NullLogger<MalaPeticionLogeadorIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status400BadRequest, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task LogeoContextoIntermediario_InvocaSiguiente()
    {
        DefaultHttpContext ctx = CrearContexto();
        bool paso = false;
        RequestDelegate siguiente = async c =>
        {
            paso = true;
            await c.Response.WriteAsync("ok");
        };

        LogeoContextoIntermediario sut = new(siguiente, NullLogger<LogeoContextoIntermediario>.Instance);
        await sut.InvokeAsync(ctx);

        Assert.True(paso);
    }

    [Fact]
    public async Task SeguridadErrorLogeadorIntermediario_401_Vacio_DisparaRutaPorDefectoEnLog()
    {
        DefaultHttpContext ctx = new();
        ctx.Response.Body = new MemoryStream();
        ctx.Request.Scheme = "http";
        ctx.Request.Host = new HostString("localhost");

        RequestDelegate siguiente = c =>
        {
            c.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        SeguridadErrorLogeadorIntermediario sut = new(siguiente, NullLogger<SeguridadErrorLogeadorIntermediario>.Instance);
        await sut.Invoke(ctx);

        Assert.Equal(StatusCodes.Status401Unauthorized, ctx.Response.StatusCode);
    }

    [Fact]
    public async Task ManejoAutorizacionRespuestaJson_ForbidAutenticado_Escribe403()
    {
        DefaultHttpContext ctx = CrearContexto();
        ClaimsIdentity identidad = new([new Claim(ClaimTypes.Name, "usr")], authenticationType: "Bearer");
        ctx.User = new ClaimsPrincipal(identidad);

        AuthorizationFailure fallo = AuthorizationFailure.Failed([new RequisitoMarcador()]);
        PolicyAuthorizationResult resultado = PolicyAuthorizationResult.Forbid(fallo);

        AuthorizationPolicy politica = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        ManejoAutorizacionRespuestaJson sut = new();
        RequestDelegate siguiente = _ => Task.CompletedTask;

        await sut.HandleAsync(siguiente, ctx, politica, resultado);

        Assert.Equal(StatusCodes.Status403Forbidden, ctx.Response.StatusCode);
        string json = await LeerCuerpoRespuesta(ctx);
        Assert.Contains("permiso", json);
    }

    [Fact]
    public async Task ManejoAutorizacionRespuestaJson_Success_DelegaEnHandlerPorDefecto()
    {
        DefaultHttpContext ctx = CrearContexto();
        bool delegado = false;
        RequestDelegate siguiente = c =>
        {
            delegado = true;
            return c.Response.WriteAsync("sig");
        };

        ManejoAutorizacionRespuestaJson sut = new();
        AuthorizationPolicy politica = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

        await sut.HandleAsync(siguiente, ctx, politica, PolicyAuthorizationResult.Success());

        Assert.True(delegado);
    }

    [Fact]
    public async Task JwtBearer_OnChallenge_Escribe401Json()
    {
        DefaultHttpContext ctx = CrearContexto();
        JwtBearerChallengeContext challenge = new(
            ctx,
            new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, "jwt", typeof(JwtBearerHandler)),
            new JwtBearerOptions(),
            new AuthenticationProperties());

        await EventosJwtBearerResponderJson.OnChallenge(challenge);

        Assert.True(challenge.Handled);
        Assert.Equal(StatusCodes.Status401Unauthorized, ctx.Response.StatusCode);
        string json = await LeerCuerpoRespuesta(ctx);
        Assert.Contains("operacionExitosa", json);
    }
}
