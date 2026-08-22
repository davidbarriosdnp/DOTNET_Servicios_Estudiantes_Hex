using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Comandos
{
    public sealed record ActualizarAulaCommand(int AulaId, string Nombre, int Capacidad, int SedeId, byte Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarAulaValidator : AbstractValidator<ActualizarAulaCommand>
    {
        public ActualizarAulaValidator()
        {
            RuleFor(x => x.AulaId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Capacidad).GreaterThan(0);
            RuleFor(x => x.SedeId).GreaterThan(0);
        }
    }

    public sealed class ActualizarAulaHandler : IRequestHandler<ActualizarAulaCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ActualizarAulaHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(ActualizarAulaCommand request, CancellationToken cancellationToken)
        {
            var aula = await _repositorio.ObtenerAulaPorIdAsync(request.AulaId, cancellationToken);
            if (aula == null) throw new KeyNotFoundException($"Aula con ID {request.AulaId} no existe.");

            await _repositorio.ActualizarAulaAsync(request.AulaId, request.Nombre, request.Capacidad, request.SedeId, request.Estado, cancellationToken);
            return new Respuesta<bool>(true, "Aula actualizada con éxito.");
        }
    }
}
