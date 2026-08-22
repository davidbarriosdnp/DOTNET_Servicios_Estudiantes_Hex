using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Comandos
{
    public sealed record ActualizarSedeCommand(int SedeId, string Nombre, string Direccion, byte Estado) : IRequest<Respuesta<bool>>;

    public sealed class ActualizarSedeValidator : AbstractValidator<ActualizarSedeCommand>
    {
        public ActualizarSedeValidator()
        {
            RuleFor(x => x.SedeId).GreaterThan(0);
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Direccion).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class ActualizarSedeHandler : IRequestHandler<ActualizarSedeCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ActualizarSedeHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(ActualizarSedeCommand request, CancellationToken cancellationToken)
        {
            var sede = await _repositorio.ObtenerSedePorIdAsync(request.SedeId, cancellationToken);
            if (sede == null) throw new KeyNotFoundException($"Sede con ID {request.SedeId} no existe.");

            await _repositorio.ActualizarSedeAsync(request.SedeId, request.Nombre, request.Direccion, request.Estado, cancellationToken);
            return new Respuesta<bool>(true, "Sede actualizada con éxito.");
        }
    }
}
