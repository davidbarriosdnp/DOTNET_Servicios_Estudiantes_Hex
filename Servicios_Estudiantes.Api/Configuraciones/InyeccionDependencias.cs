using Servicios_Estudiantes.Api.Excepciones;

namespace Servicios_Estudiantes.Api.Configuraciones
{
    public static class InyeccionDependencias
    {
        public static ApiConfiguracion InstanciarConfiguracionApi(
            this IServiceCollection servicios,
            IConfiguration configuracion,
            IWebHostEnvironment entorno)
        {
            ApiConfiguracion configuracionApi = entorno.IsDevelopment()
                ? CrearDesdeConfiguracionDesarrollo(configuracion)
                : CrearDesdeVariablesEntorno(configuracion);

            servicios.AddSingleton(configuracionApi);
            return configuracionApi;
        }

        private static ApiConfiguracion CrearDesdeConfiguracionDesarrollo(IConfiguration configuracion)
        {
            string nombre = configuracion["Api:NombreServicio"]
                ?? Environment.GetEnvironmentVariable("NOMBRE_SERVICIO")
                ?? "Servicios_Estudiantes";

            string origenes = configuracion["Api:OrigenesCorsPermitidos"]
                ?? Environment.GetEnvironmentVariable("ORIGENES_CORS_PERMITIDOS")
                ?? "http://localhost:5045,https://localhost:7020";

            string logs = configuracion["Api:OtelLogsEndpoint"]
                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_LOGS")
                ?? string.Empty;

            string trazas = configuracion["Api:OtelTracesEndpoint"]
                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_TRACES")
                ?? string.Empty;

            string encabezados = configuracion["Api:OtelHeaders"]
                ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS")
                ?? string.Empty;

            return new ApiConfiguracion(
                OtelLogsEndpoint: logs,
                OtelTracesEndpoint: trazas,
                OtelHeaders: encabezados,
                NombreServicio: nombre,
                OrigenesCORSPermitidos: origenes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        private static ApiConfiguracion CrearDesdeVariablesEntorno(IConfiguration configuracion)
        {
            string? logs = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_LOGS")
                ?? configuracion["Api:OtelLogsEndpoint"];
            string? trazas = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT_TRACES")
                ?? configuracion["Api:OtelTracesEndpoint"];
            string? encabezados = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS")
                ?? configuracion["Api:OtelHeaders"];
            string? nombre = Environment.GetEnvironmentVariable("NOMBRE_SERVICIO")
                ?? configuracion["Api:NombreServicio"];
            string[]? cors = Environment.GetEnvironmentVariable("ORIGENES_CORS_PERMITIDOS")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? configuracion.GetSection("Api:OrigenesCorsPermitidos").Get<string[]>();

            if (string.IsNullOrWhiteSpace(logs))
                throw new VariableApiNoConfigurada("OTEL_EXPORTER_OTLP_ENDPOINT_LOGS o Api:OtelLogsEndpoint");
            if (string.IsNullOrWhiteSpace(trazas))
                throw new VariableApiNoConfigurada("OTEL_EXPORTER_OTLP_ENDPOINT_TRACES o Api:OtelTracesEndpoint");
            if (string.IsNullOrWhiteSpace(encabezados))
                throw new VariableApiNoConfigurada("OTEL_EXPORTER_OTLP_HEADERS o Api:OtelHeaders");
            if (string.IsNullOrWhiteSpace(nombre))
                throw new VariableApiNoConfigurada("NOMBRE_SERVICIO o Api:NombreServicio");
            if (cors is null || cors.Length == 0)
                throw new VariableApiNoConfigurada("ORIGENES_CORS_PERMITIDOS o Api:OrigenesCorsPermitidos");

            return new ApiConfiguracion(logs, trazas, encabezados, nombre, cors);
        }
    }
}
