using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Consultas
{
    public sealed record ListarProfesoresQuery(bool SoloActivos = false) : IRequest<Respuesta<IReadOnlyList<ProfesorDto>>>;

    public sealed class ListarProfesoresHandler(IRepositorioAcademico repo) : IRequestHandler<ListarProfesoresQuery, Respuesta<IReadOnlyList<ProfesorDto>>>
    {
        public async Task<Respuesta<IReadOnlyList<ProfesorDto>>> Handle(ListarProfesoresQuery r, CancellationToken ct) =>
            Respuesta<IReadOnlyList<ProfesorDto>>.Ok(await repo.ListarProfesoresAsync(r.SoloActivos, ct).ConfigureAwait(false));
    }
}
