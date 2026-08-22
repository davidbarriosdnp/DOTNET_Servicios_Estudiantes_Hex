using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Comandos
{
    public sealed record CrearSedeCommand(string Nombre, string Direccion) : IRequest<Respuesta<int>>;

    public sealed class CrearSedeValidator : AbstractValidator<CrearSedeCommand>
    {
        public CrearSedeValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Direccion).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class CrearSedeHandler : IRequestHandler<CrearSedeCommand, Respuesta<int>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public CrearSedeHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<int>> Handle(CrearSedeCommand request, CancellationToken cancellationToken)
        {
            var id = await _repositorio.InsertarSedeAsync(request.Nombre, request.Direccion, cancellationToken);
            return new Respuesta<int>(id, "Sede creada con éxito.");
        }
    }
}
