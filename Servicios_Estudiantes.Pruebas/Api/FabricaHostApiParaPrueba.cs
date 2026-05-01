using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Servicios_Estudiantes.Pruebas.Api;

internal sealed class FabricaHostApiParaPrueba : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configuracion) =>
        {
            Dictionary<string, string?> valores =
                new(StringComparer.OrdinalIgnoreCase)
                {
                    ["ConnectionStrings:Estudiantes"] =
                        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EstudiantesHex_PruebasIntegracion;"
                        + "Integrated Security=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True",
                    ["Jwt:Emisor"] = "Servicios_Estudiantes_Test",
                    ["Jwt:Audiencia"] = "Servicios_Estudiantes_Test",
                    ["Jwt:ClaveSecreta"] = "Pruebas_Integracion_Jwt_AlMenos32_!!",
                    ["Jwt:MinutosValidezAcceso"] = "60",
                    ["Jwt:DiasValidezRefresh"] = "1"
                };

            configuracion.AddInMemoryCollection(valores);
        });
    }
}
