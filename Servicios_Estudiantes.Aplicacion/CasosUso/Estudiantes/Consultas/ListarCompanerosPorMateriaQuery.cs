using MediatR;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas
{
    public sealed record ListarCompanerosPorMateriaQuery(int EstudianteIdSolicitante, int MateriaId)
        : IRequest<Respuesta<IReadOnlyList<string>>>;

    public sealed class ListarCompanerosPorMateriaQueryHandler(IRepositorioAcademico repositorio)
        : IRequestHandler<ListarCompanerosPorMateriaQuery, Respuesta<IReadOnlyList<string>>>
    {
        private readonly IRepositorioAcademico _repositorio = repositorio;

        public async Task<Respuesta<IReadOnlyList<string>>> Handle(
            ListarCompanerosPorMateriaQuery solicitud,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> nombres =
                await _repositorio.ListarNombresCompanerosPorMateriaAsync(
                    solicitud.EstudianteIdSolicitante,
                    solicitud.MateriaId,
                    cancellationToken).ConfigureAwait(false);

            return Respuesta<IReadOnlyList<string>>.Ok(nombres);
        }
    }
}
