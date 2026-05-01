using Servicios_Estudiantes.Api.Constantes;

namespace Servicios_Estudiantes.Api.Excepciones
{
    internal class VariableApiNoConfigurada(string nombreVariable)
        : ExcepcionApi(string.Format(MensajesError.VariableNoConfigurada, nombreVariable));
    
}
