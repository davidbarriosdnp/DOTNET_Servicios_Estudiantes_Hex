using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Servicios_Estudiantes.Pruebas;

public sealed class ApiIntegracionPruebas
{
    private static FabricaHostApiParaPrueba CrearFabrica() => new();

    [Fact]
    public async Task Health_DebeRetornar_200_ConFormatoHealthy()
    {
        using FabricaHostApiParaPrueba fabrica = CrearFabrica();
        HttpClient cliente = fabrica.CreateClient();

        HttpResponseMessage respuesta = await cliente.GetAsync(new Uri("/health", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        string cuerpo = await respuesta.Content.ReadAsStringAsync();
        using JsonDocument documento = JsonDocument.Parse(cuerpo);
        Assert.Equal("Healthy", documento.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task RutasConBearer_SinJwt_DebenRetornar_401_EnFormatoRespuesta()
    {
        using FabricaHostApiParaPrueba fabrica = CrearFabrica();
        HttpClient cliente = fabrica.CreateClient();

        HttpResponseMessage respuesta = await cliente
            .GetAsync(new Uri("/api/v1/Profesores?soloActivos=true", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        Assert.Contains("application/json", respuesta.Content.Headers.ContentType?.MediaType ?? string.Empty);

        string json = await respuesta.Content.ReadAsStringAsync();
        using JsonDocument documento = JsonDocument.Parse(json);

        JsonElement root = documento.RootElement;
        Assert.False(root.GetProperty("operacionExitosa").GetBoolean());
        Assert.False(string.IsNullOrEmpty(root.GetProperty("mensaje").GetString()));

        JsonElement errores = root.GetProperty("errores");
        Assert.True(errores.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task RutasConBearer_TokenFirmaIncorrecta_DebeRetornar_401_EnFormatoRespuesta()
    {
        using FabricaHostApiParaPrueba fabrica = CrearFabrica();
        HttpClient cliente = fabrica.CreateClient();

        string tokenEjemploFirmaAjena =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiYWRtaW4iOiJtb2NrIn0."
            + "SIGNATURE_NOT_VALID_THIS_IS_PURPOSEFULLY_TRUNCATED_fake";

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEjemploFirmaAjena);

        HttpResponseMessage respuesta = await cliente
            .GetAsync(new Uri("/api/v1/Profesores?soloActivos=true", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);

        using JsonDocument documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        Assert.False(documento.RootElement.GetProperty("operacionExitosa").GetBoolean());
    }
}
