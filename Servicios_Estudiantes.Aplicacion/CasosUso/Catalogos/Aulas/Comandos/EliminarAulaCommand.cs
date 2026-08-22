using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Comandos
{
    public sealed record EliminarAulaCommand(int AulaId) : IRequest<Respuesta<bool>>;

    public sealed class EliminarAulaHandler : IRequestHandler<EliminarAulaCommand, Respuesta<bool>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public EliminarAulaHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<bool>> Handle(EliminarAulaCommand request, CancellationToken cancellationToken)
        {
            var aula = await _repositorio.ObtenerAulaPorIdAsync(request.AulaId, cancellationToken);
            if (aula == null) throw new KeyNotFoundException($"Aula con ID {request.AulaId} no existe.");

            await _repositorio.EliminarAulaAsync(request.AulaId, cancellationToken);
            return new Respuesta<bool>(true, "Aula eliminada lógicamente.");
        }
    }
}
