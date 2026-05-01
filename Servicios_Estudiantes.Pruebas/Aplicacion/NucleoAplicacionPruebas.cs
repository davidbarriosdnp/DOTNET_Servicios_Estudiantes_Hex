using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Comportamientos;
using Servicios_Estudiantes.Aplicacion.Constantes;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.InyeccionDependencias;
using Servicios_Estudiantes.Aplicacion.Utilidades;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Microsoft.Extensions.DependencyInjection;

namespace Servicios_Estudiantes.Pruebas.Aplicacion;

public sealed class NucleoAplicacionPruebas
{
    private sealed record SolicitudPrueba(int Valor) : IRequest<bool>;

    private sealed class SolicitudPruebaValidador : AbstractValidator<SolicitudPrueba>
    {
        public SolicitudPruebaValidador() =>
            RuleFor(x => x.Valor).GreaterThan(0).WithMessage("Valor debe ser positivo.");
    }

    [Fact]
    public void HashTokenRenovacion_AHexMinuscula_EsDeterminista()
    {
        string hex = HashTokenRenovacion.AHexMinuscula("token-prueba");
        Assert.Equal(64, hex.Length);
        Assert.Equal(hex, HashTokenRenovacion.AHexMinuscula("token-prueba"));
        Assert.True(hex.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'));
    }

    [Fact]
    public void Respuesta_Ok_Fail_Match_YConversionImplicita()
    {
        Respuesta<int> ok = Respuesta<int>.Ok(7, "listo");
        Assert.True(ok.OperacionExitosa);
        Assert.Equal(14, ok.Match(v => v * 2, (_, _) => -1));

        Respuesta<int> fail = Respuesta<int>.Fail("error", new[] { "detalle" });
        Assert.False(fail.OperacionExitosa);
        Assert.Single(fail.Errores);
        Assert.Equal("errordetalle", fail.Match(_ => "", (m, e) => m + string.Concat(e)));

        Respuesta<string> implicita = "hola";
        Assert.True(implicita.OperacionExitosa);
        Assert.Equal("hola", implicita.Resultado);
    }

    [Fact]
    public void ExcepcionAplicacion_Constructores()
    {
        Assert.NotNull(new ExcepcionAplicacion());
        Assert.Equal("m", new ExcepcionAplicacion("m").Message);
        InvalidOperationException inner = new("i");
        ExcepcionAplicacion outer = new("o", inner);
        Assert.Same(inner, outer.InnerException);
    }

    [Fact]
    public void ExcepcionValidacion_AgregaMensajesDeFluentValidation()
    {
        ValidationFailure f1 = new("Nombre", "Nombre inválido");
        ExcepcionValidacion ex = new(new[] { f1 });
        Assert.Equal(Mensajes.ErrorGeneral, ex.Message);
        Assert.Contains("Nombre inválido", ex.Errores);
    }

    [Fact]
    public async Task ValidacionComportamiento_SinValidadores_EjecutaSiguiente()
    {
        ValidacionComportamiento<SolicitudPrueba, bool> comportamiento = new(Enumerable.Empty<IValidator<SolicitudPrueba>>());
        bool invocado = false;

        Task<bool> siguiente(CancellationToken ct)
        {
            invocado = true;
            return Task.FromResult(true);
        }

        bool resultado = await comportamiento.Handle(new SolicitudPrueba(1), siguiente, CancellationToken.None);
        Assert.True(invocado);
        Assert.True(resultado);
    }

    [Fact]
    public async Task ValidacionComportamiento_ConFallo_LanzaExcepcionValidacion()
    {
        SolicitudPruebaValidador validador = new();
        ValidacionComportamiento<SolicitudPrueba, bool> comportamiento = new(new IValidator<SolicitudPrueba>[] { validador });

        Task<bool> siguiente(CancellationToken ct) => Task.FromResult(true);

        ExcepcionValidacion ex = await Assert.ThrowsAsync<ExcepcionValidacion>(() =>
            comportamiento.Handle(new SolicitudPrueba(-1), siguiente, CancellationToken.None));

        Assert.NotEmpty(ex.Errores);
    }

    [Fact]
    public void AgregarCapaAplicacion_RegistraIMediator()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AgregarCapaAplicacion();
        using ServiceProvider sp = services.BuildServiceProvider();
        IMediator mediator = sp.GetRequiredService<IMediator>();
        Assert.NotNull(mediator);
    }

    [Fact]
    public void Mensajes_Constantes_NoVacias()
    {
        Assert.False(string.IsNullOrEmpty(Mensajes.ErrorGeneral));
        Assert.False(string.IsNullOrEmpty(Mensajes.PruebaObligatorio));
    }
}
