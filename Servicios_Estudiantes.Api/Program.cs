using Servicios_Estudiantes.Api.Configuraciones;
using Servicios_Estudiantes.Api.Extensiones;
using Servicios_Estudiantes.Aplicacion.InyeccionDependencias;
using Servicios_Estudiantes.Infraestructura.InyeccionDependencias;
using DotNetEnv;

Env.Load();

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

ApiConfiguracion configuracion = constructor.Services.InstanciarConfiguracionApi(constructor.Configuration, constructor.Environment);
constructor.Services.ConfigureDynatraceTrazas(configuracion);
constructor.Logging.ConfigurarDynatraceLogs(configuracion);

constructor.Services.AddCustomCors(configuracion);
constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen();
constructor.Services.AgregarCapaAplicacion();
constructor.Services.AgregarInfraestructura(constructor.Configuration);

WebApplication app = constructor.Build();

app.ConfigurePipeline();

app.Run();
