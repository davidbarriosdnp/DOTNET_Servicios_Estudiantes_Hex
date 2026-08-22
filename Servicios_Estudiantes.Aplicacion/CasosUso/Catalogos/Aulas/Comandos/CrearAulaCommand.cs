using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Comandos
{
    public sealed record CrearAulaCommand(string Nombre, int Capacidad, int SedeId) : IRequest<Respuesta<int>>;

    public sealed class CrearAulaValidator : AbstractValidator<CrearAulaCommand>
    {
        public CrearAulaValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Capacidad).GreaterThan(0);
            RuleFor(x => x.SedeId).GreaterThan(0);
        }
    }

    public sealed class CrearAulaHandler : IRequestHandler<CrearAulaCommand, Respuesta<int>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public CrearAulaHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<int>> Handle(CrearAulaCommand request, CancellationToken cancellationToken)
        {
            var id = await _repositorio.InsertarAulaAsync(request.Nombre, request.Capacidad, request.SedeId, cancellationToken);
            return new Respuesta<int>(id, "Aula creada con éxito.");
        }
    }
}
