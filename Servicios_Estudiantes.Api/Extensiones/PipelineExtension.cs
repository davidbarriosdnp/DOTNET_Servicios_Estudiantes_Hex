using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Servicios_Estudiantes.Api.Extensiones
{
    public static class PipelineExtension
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UsarManejadorIntermediarioErrores();
            app.UsarSeguridadSolicitudLogeador();
            app.UsarRegistradorContextoIntermediario();
            app.UsarBadRequestLogeador();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseCors("PoliticaRestrictiva");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            HealthCheckOptions opcionesInformeCompleto = new()
            {
                ResponseWriter = EscribirInformeSaludJson,
                Predicate = _ => true
            };

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = comprobacion => comprobacion.Tags.Contains("live"),
                ResponseWriter = EscribirInformeSaludJson
            }).AllowAnonymous();

            app.MapHealthChecks("/health", opcionesInformeCompleto).AllowAnonymous();

            RouteGroupBuilder apiGroup = app.MapGroup("/api");
            apiGroup.MapearTodasLasVersionesControlador("Auth");
            apiGroup.MapearTodasLasVersionesControlador("Usuarios");
            apiGroup.MapearTodasLasVersionesControlador("Estudiantes");
            apiGroup.MapearTodasLasVersionesControlador("ProgramasCredito");
            apiGroup.MapearTodasLasVersionesControlador("Profesores");
            apiGroup.MapearTodasLasVersionesControlador("Materias");
            return app;
        }

        private static async Task EscribirInformeSaludJson(HttpContext contexto, HealthReport informe)
        {
            contexto.Response.ContentType = "application/json; charset=utf-8";

            await contexto.Response
                .WriteAsJsonAsync(
                    new
                    {
                        status = informe.Status.ToString(),
                        totalDurationMs = informe.TotalDuration.TotalMilliseconds,
                        checks = informe.Entries.ToDictionary(
                            par => par.Key,
                            par => new
                            {
                                status = par.Value.Status.ToString(),
                                description = par.Value.Description,
                                durationMs = par.Value.Duration.TotalMilliseconds
                            })
                    })
                .ConfigureAwait(false);
        }
    }
}
