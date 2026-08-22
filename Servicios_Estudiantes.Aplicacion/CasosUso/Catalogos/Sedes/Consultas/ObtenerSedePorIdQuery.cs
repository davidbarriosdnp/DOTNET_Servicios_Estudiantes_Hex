using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Sedes.Consultas
{
    public sealed record ObtenerSedePorIdQuery(int SedeId) : IRequest<Respuesta<SedeDto>>;

    public sealed class ObtenerSedeHandler : IRequestHandler<ObtenerSedePorIdQuery, Respuesta<SedeDto>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ObtenerSedeHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<SedeDto>> Handle(ObtenerSedePorIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _repositorio.ObtenerSedePorIdAsync(request.SedeId, cancellationToken);
            if (dto == null) throw new KeyNotFoundException($"Sede con ID {request.SedeId} no existe.");
            return new Respuesta<SedeDto>(dto);
        }
    }
}
