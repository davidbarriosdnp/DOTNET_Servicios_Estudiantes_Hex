using Moq;
using Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Estudiantes.Consultas;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Pruebas.Aplicacion;

public sealed class CasosUsoEstudiantesPruebas
{
    private static readonly DateTime FechaBase = new(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task CrearEstudiante_DelegaYDevuelveId()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.InsertarEstudianteAsync("N", "n@test.dev", 3, null, It.IsAny<CancellationToken>())).ReturnsAsync(42);

        CrearEstudianteCommandHandler sut = new(repo.Object);
        Respuesta<int> resp = await sut.Handle(new CrearEstudianteCommand("N", "n@test.dev", 3), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        Assert.Equal(42, resp.Resultado);
    }

    [Fact]
    public async Task ActualizarEstudiante_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        ActualizarEstudianteCommandHandler sut = new(repo.Object);
        await sut.Handle(new ActualizarEstudianteCommand(1, "A", "a@test.dev", 2, 1), CancellationToken.None);

        repo.Verify(r => r.ActualizarEstudianteAsync(1, "A", "a@test.dev", 2, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EliminarEstudiante_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        EliminarEstudianteCommandHandler sut = new(repo.Object);
        await sut.Handle(new EliminarEstudianteCommand(9), CancellationToken.None);
        repo.Verify(r => r.EliminarEstudianteAsync(9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObtenerEstudiantePorId_NoEncontrado_Lanza()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ObtenerEstudiantePorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((EstudianteDetalleDto?)null);

        ObtenerEstudiantePorIdQueryHandler sut = new(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ObtenerEstudiantePorIdQuery(1), CancellationToken.None));
    }

    [Fact]
    public async Task ObtenerEstudiantePorId_Encontrado_DevuelveOk()
    {
        EstudianteDetalleDto dto = new(1, "x", "x@test.dev", 1, FechaBase, null, 1, null);
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ObtenerEstudiantePorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        ObtenerEstudiantePorIdQueryHandler sut = new(repo.Object);
        Respuesta<EstudianteDetalleDto> resp = await sut.Handle(new ObtenerEstudiantePorIdQuery(1), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        Assert.Equal("x", resp.Resultado!.Nombre);
    }

    [Fact]
    public async Task ListarRegistrosEstudiantes_DevuelveLista()
    {
        IReadOnlyList<EstudianteRegistroDto> datos =
            [new EstudianteRegistroDto(1, "a", "a@test.dev", 1, FechaBase, null, 1, "")];
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ListarRegistrosEstudiantesAsync(true, It.IsAny<CancellationToken>())).ReturnsAsync(datos);

        ListarRegistrosEstudiantesQueryHandler sut = new(repo.Object);
        Respuesta<IReadOnlyList<EstudianteRegistroDto>> resp =
            await sut.Handle(new ListarRegistrosEstudiantesQuery(true), CancellationToken.None);

        Assert.Single(resp.Resultado!);
    }

    [Fact]
    public async Task ListarMateriasCatalogo_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ListarMateriasPorProgramaAsync(5, false, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        ListarMateriasCatalogoQueryHandler sut = new(repo.Object);
        await sut.Handle(new ListarMateriasCatalogoQuery(5, false), CancellationToken.None);

        repo.Verify(r => r.ListarMateriasPorProgramaAsync(5, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarInscripcionEstudiante_DevuelveOk()
    {
        InscripcionEstudianteDto fila = new(3, "Mat", 3, 1, "Prof", FechaBase, null, 1);
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ListarInscripcionPorEstudianteAsync(10, true, It.IsAny<CancellationToken>())).ReturnsAsync([fila]);

        ListarInscripcionEstudianteQueryHandler sut = new(repo.Object);
        Respuesta<IReadOnlyList<InscripcionEstudianteDto>> resp =
            await sut.Handle(new ListarInscripcionEstudianteQuery(10), CancellationToken.None);

        Assert.Single(resp.Resultado!);
    }

    [Fact]
    public async Task ListarCompanerosPorMateria_DevuelveNombres()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ListarNombresCompanerosPorMateriaAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Pepe", "Lucía" });

        ListarCompanerosPorMateriaQueryHandler sut = new(repo.Object);
        Respuesta<IReadOnlyList<string>> resp =
            await sut.Handle(new ListarCompanerosPorMateriaQuery(1, 2), CancellationToken.None);

        Assert.Equal(2, resp.Resultado!.Count);
    }

    [Fact]
    public async Task RegistrarInscripcion_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        RegistrarInscripcionCommandHandler sut = new(repo.Object);
        await sut.Handle(new RegistrarInscripcionCommand(1, 2, 3, 4), CancellationToken.None);

        repo.Verify(r => r.RegistrarInscripcionAsync(1, 2, 3, 4, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AgregarInscripcionFila_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        AgregarInscripcionFilaHandler sut = new(repo.Object);
        await sut.Handle(new AgregarInscripcionFilaCommand(1, 9), CancellationToken.None);
        repo.Verify(r => r.InsertarInscripcionFilaAsync(1, 9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EliminarInscripcionFila_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        EliminarInscripcionFilaHandler sut = new(repo.Object);
        await sut.Handle(new EliminarInscripcionFilaCommand(1, 9), CancellationToken.None);
        repo.Verify(r => r.EliminarInscripcionFilaAsync(1, 9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CambiarMateriaInscripcion_Delega()
    {
        Mock<IRepositorioAcademico> repo = new();
        CambiarMateriaInscripcionHandler sut = new(repo.Object);
        await sut.Handle(new CambiarMateriaInscripcionCommand(1, 5, 6), CancellationToken.None);
        repo.Verify(r => r.ActualizarInscripcionMateriaAsync(1, 5, 6, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void RegistrarInscripcionCommandValidator_RechazaMateriasDuplicadas()
    {
        RegistrarInscripcionCommandValidator v = new();
        FluentValidation.Results.ValidationResult r =
            v.Validate(new RegistrarInscripcionCommand(1, 2, 2, 3));
        Assert.False(r.IsValid);
    }

    [Fact]
    public void CambiarMateriaInscripcionValidator_RechazaMismaMateria()
    {
        CambiarMateriaInscripcionValidator v = new();
        Assert.False(v.Validate(new CambiarMateriaInscripcionCommand(1, 4, 4)).IsValid);
    }
}
