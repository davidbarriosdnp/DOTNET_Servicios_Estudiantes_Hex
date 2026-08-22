using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Comandos
{
    public sealed record EliminarSedeCommand(int SedeId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarSedeHandler : IRequestHandler<EliminarSedeCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public EliminarSedeHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(EliminarSedeCommand request, CancellationToken cancellationToken)
        {
            var sede = await _repositorio.ObtenerSedePorIdAsync(request.SedeId, cancellationToken);
            if (sede == null) throw new KeyNotFoundException($"Sede con ID {request.SedeId} no existe.");

            await _repositorio.EliminarSedeAsync(request.SedeId, cancellationToken);
            return new Respuesta<bool>(true, "Sede eliminada lógicamente.");
        }
    }
}
