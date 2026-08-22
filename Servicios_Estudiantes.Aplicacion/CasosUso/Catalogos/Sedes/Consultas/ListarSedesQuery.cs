using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Consultas
{
    public sealed record ListarSedesQuery(bool SoloActivos) : IRequest<Respuesta<IReadOnlyList<SedeDto>>>;

    public sealed class ListarSedesHandler : IRequestHandler<ListarSedesQuery, Respuesta<IReadOnlyList<SedeDto>>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ListarSedesHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<SedeDto>>> Handle(ListarSedesQuery request, CancellationToken cancellationToken)
        {
            var data = await _repositorio.ListarSedesAsync(request.SoloActivos, cancellationToken);
            return new Respuesta<IReadOnlyList<SedeDto>>(data);
        }
    }
}
