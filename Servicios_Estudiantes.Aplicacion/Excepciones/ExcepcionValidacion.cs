using Servicios_Estudiantes.Aplicacion.Constantes;
using FluentValidation.Results;

namespace Servicios_Estudiantes.Aplicacion.Excepciones
{
    public class ExcepcionValidacion : ExcepcionAplicacion
    {
        public List<string> Errores { get; }

        public ExcepcionValidacion(IEnumerable<ValidationFailure> fallos) : base(Mensajes.ErrorGeneral)
        {
            Errores = [];
            foreach (ValidationFailure? fallo in fallos)
            {
                Errores.Add(fallo.ErrorMessage);
            }
        }
    }
}
