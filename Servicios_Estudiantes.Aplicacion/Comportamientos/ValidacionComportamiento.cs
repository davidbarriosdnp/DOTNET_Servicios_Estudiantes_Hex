using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Servicios_Estudiantes.Aplicacion.Comportamientos
{
    /// <summary>
    /// Comportamiento de MediatR que ejecuta validaciones FluentValidation antes de llegar al handler.
    /// </summary>
    public class ValidacionComportamiento<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validador) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validador = validador;

        /// <summary>
        /// Ejecuta la validación del request y, si hay fallos, lanza <see cref="Excepciones.ExcepcionValidacion"/>.
        /// </summary>
        public async Task<TResponse> Handle(TRequest solicitud, RequestHandlerDelegate<TResponse> siguiente, CancellationToken tokenCancelacion)
        {
            if (_validador.Any())
            {
                ValidationContext<TRequest> contexto = new(solicitud);
                ValidationResult[] validacionResultado = await Task.WhenAll(_validador.Select(v => v.ValidateAsync(contexto, tokenCancelacion)));
                List<ValidationFailure> fallos = [.. validacionResultado.SelectMany(r => r.Errors).Where(f => f != null)];

                if (fallos.Count > 0)
                {
                    throw new Excepciones.ExcepcionValidacion(fallos);
                }
            }

            return await siguiente(tokenCancelacion);
        }
    }
}
