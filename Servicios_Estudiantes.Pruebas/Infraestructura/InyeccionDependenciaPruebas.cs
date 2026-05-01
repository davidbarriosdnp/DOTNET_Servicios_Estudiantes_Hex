using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Infraestructura.AccesoDatos;
using Servicios_Estudiantes.Infraestructura.InyeccionDependencias;

namespace Servicios_Estudiantes.Pruebas.Infraestructura;

public sealed class InyeccionDependenciaPruebas
{
    [Fact]
    public void AgregarInfraestructura_SinCadena_Lanza()
    {
        IConfiguration configuracion = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        ServiceCollection services = new();

        Assert.Throws<InvalidOperationException>(() => services.AgregarInfraestructura(configuracion));
    }

    [Fact]
    public void AgregarInfraestructura_ConCadena_RegistraRepositorios()
    {
        IConfiguration configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:Estudiantes"] = "Server=(localdb)\\MSSQLLocalDB;Database=x;" })
            .Build();

        ServiceCollection services = new();
        services.AgregarInfraestructura(configuracion);

        using ServiceProvider sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IRepositorioAcademico>());
        Assert.NotNull(sp.GetService<IRepositorioUsuarios>());
    }

    [Fact]
    public void RepositorioAcademicoSql_CadenaNull_Lanza()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositorioAcademicoSql(null!));
    }
}
