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

            RouteGroupBuilder apiGroup = app.MapGroup("/api");
            apiGroup.MapearTodasLasVersionesControlador("Plantilla");
            return app;
        }
    }
}
