using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Excepciones;
using Servicios_Estudiantes.Aplicacion.Puertos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Aulas.Consultas
{
    public sealed record ObtenerAulaPorIdQuery(int AulaId) : IRequest<Respuesta<AulaDto>>;

    public sealed class ObtenerAulaHandler : IRequestHandler<ObtenerAulaPorIdQuery, Respuesta<AulaDto>>
    {
        private readonly IRepositorioAcademico _repositorio;
        public ObtenerAulaHandler(IRepositorioAcademico repositorio) => _repositorio = repositorio;

        public async Task<Respuesta<AulaDto>> Handle(ObtenerAulaPorIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _repositorio.ObtenerAulaPorIdAsync(request.AulaId, cancellationToken);
            if (dto == null) throw new KeyNotFoundException($"Aula con ID {request.AulaId} no existe.");
            return new Respuesta<AulaDto>(dto);
        }
    }
}
