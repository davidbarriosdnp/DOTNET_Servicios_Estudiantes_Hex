using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Consultas
{
    public sealed record ListarAulasQuery(bool SoloActivos) : IRequest<Respuesta<IReadOnlyList<AulaDto>>>;

    public sealed class ListarAulasHandler : IRequestHandler<ListarAulasQuery, Respuesta<IReadOnlyList<AulaDto>>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ListarAulasHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<AulaDto>>> Handle(ListarAulasQuery request, CancellationToken cancellationToken)
        {
            var data = await _repositorio.ListarAulasAsync(request.SoloActivos, cancellationToken);
            return new Respuesta<IReadOnlyList<AulaDto>>(data);
        }
    }
}
