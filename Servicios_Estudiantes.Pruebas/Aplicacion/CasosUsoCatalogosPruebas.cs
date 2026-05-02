using Moq;
using Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Pruebas.Aplicacion;

public sealed class CasosUsoCatalogosPruebas
{
    private static readonly DateTime FechaBase = new(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ProgramaCredito_CrearYActualizarYEliminar_Listar()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.InsertarProgramaCreditoAsync("P", 3, 6, It.IsAny<CancellationToken>())).ReturnsAsync(11);

        CrearProgramaCreditoHandler crear = new(repo.Object);
        Respuesta<int> creado = await crear.Handle(new CrearProgramaCreditoCommand("P", 3, 6), CancellationToken.None);
        Assert.Equal(11, creado.Resultado);

        ActualizarProgramaCreditoHandler act = new(repo.Object);
        await act.Handle(new ActualizarProgramaCreditoCommand(11, "P2", 4, 8, 1), CancellationToken.None);
        repo.Verify(r => r.ActualizarProgramaCreditoAsync(11, "P2", 4, 8, 1, It.IsAny<CancellationToken>()), Times.Once);

        EliminarProgramaCreditoHandler elim = new(repo.Object);
        await elim.Handle(new EliminarProgramaCreditoCommand(11), CancellationToken.None);
        repo.Verify(r => r.EliminarProgramaCreditoAsync(11, It.IsAny<CancellationToken>()), Times.Once);

        ProgramaCreditoDto dto = new(11, "P", 3, 6, FechaBase, null, 1);
        repo.Setup(r => r.ObtenerProgramaCreditoPorIdAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        ObtenerProgramaCreditoHandler obt = new(repo.Object);
        Respuesta<ProgramaCreditoDto> uno = await obt.Handle(new ObtenerProgramaCreditoPorIdQuery(11), CancellationToken.None);
        Assert.Equal("P", uno.Resultado!.Nombre);

        repo.Setup(r => r.ListarProgramasCreditoAsync(true, It.IsAny<CancellationToken>())).ReturnsAsync([dto]);
        ListarProgramasCreditoHandler list = new(repo.Object);
        Respuesta<IReadOnlyList<ProgramaCreditoDto>> lista =
            await list.Handle(new ListarProgramasCreditoQuery(true), CancellationToken.None);
        Assert.Single(lista.Resultado!);
    }

    [Fact]
    public async Task ProgramaCredito_ObtenerPorId_NoExiste_Lanza()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ObtenerProgramaCreditoPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((ProgramaCreditoDto?)null);
        ObtenerProgramaCreditoHandler sut = new(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ObtenerProgramaCreditoPorIdQuery(99), CancellationToken.None));
    }

    [Fact]
    public async Task Profesor_CicloCrudYConsultas()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.InsertarProfesorAsync("Dr.", It.IsAny<CancellationToken>())).ReturnsAsync(5);

        CrearProfesorHandler crear = new(repo.Object);
        Assert.Equal(5, (await crear.Handle(new CrearProfesorCommand("Dr."), CancellationToken.None)).Resultado);

        ActualizarProfesorHandler act = new(repo.Object);
        await act.Handle(new ActualizarProfesorCommand(5, "Dr. X", 1), CancellationToken.None);

        EliminarProfesorHandler elim = new(repo.Object);
        await elim.Handle(new EliminarProfesorCommand(5), CancellationToken.None);

        ProfesorDto dto = new(5, "Dr.", FechaBase, null, 1);
        repo.Setup(r => r.ObtenerProfesorPorIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        ObtenerProfesorHandler obt = new(repo.Object);
        Assert.Equal("Dr.", (await obt.Handle(new ObtenerProfesorPorIdQuery(5), CancellationToken.None)).Resultado!.Nombre);

        repo.Setup(r => r.ListarProfesoresAsync(false, It.IsAny<CancellationToken>())).ReturnsAsync([dto]);
        ListarProfesoresHandler list = new(repo.Object);
        Assert.Single((await list.Handle(new ListarProfesoresQuery(false), CancellationToken.None)).Resultado!);
    }

    [Fact]
    public async Task Profesor_ObtenerPorId_NoExiste_Lanza()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ObtenerProfesorPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ProfesorDto?)null);
        ObtenerProfesorHandler sut = new(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ObtenerProfesorPorIdQuery(1), CancellationToken.None));
    }

    [Fact]
    public async Task Materia_CicloCrudYConsultas()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.InsertarMateriaAsync("Mat", 3, 2, 3, It.IsAny<CancellationToken>())).ReturnsAsync(8);

        CrearMateriaHandler crear = new(repo.Object);
        Assert.Equal(8, (await crear.Handle(new CrearMateriaCommand("Mat", 3, 2, 3), CancellationToken.None)).Resultado);

        ActualizarMateriaHandler act = new(repo.Object);
        await act.Handle(new ActualizarMateriaCommand(8, "Mat2", 3, 2, 3, 1), CancellationToken.None);

        EliminarMateriaHandler elim = new(repo.Object);
        await elim.Handle(new EliminarMateriaCommand(8), CancellationToken.None);

        MateriaDetalleDto det = new(8, "Mat", 3, 2, 3, FechaBase, null, 1, "Prof");
        repo.Setup(r => r.ObtenerMateriaPorIdAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync(det);
        ObtenerMateriaHandler obt = new(repo.Object);
        Assert.Equal("Mat", (await obt.Handle(new ObtenerMateriaPorIdQuery(8), CancellationToken.None)).Resultado!.Nombre);

        MateriaCatalogoDto cat = new(8, "Mat", 3, 2, 3, "Prof", FechaBase, null, 1);
        repo.Setup(r => r.ListarMateriasPorProgramaAsync(3, true, It.IsAny<CancellationToken>())).ReturnsAsync([cat]);
        ListarMateriasPorProgramaHandler list = new(repo.Object);
        Assert.Single((await list.Handle(new ListarMateriasPorProgramaQuery(3, true), CancellationToken.None)).Resultado!);
    }

    [Fact]
    public async Task Materia_ObtenerPorId_NoExiste_Lanza()
    {
        Mock<IRepositorioAcademico> repo = new();
        repo.Setup(r => r.ObtenerMateriaPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((MateriaDetalleDto?)null);
        ObtenerMateriaHandler sut = new(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ObtenerMateriaPorIdQuery(1), CancellationToken.None));
    }

    [Fact]
    public void CrearProgramaCreditoValidator_RechazaNombreVacio()
    {
        CrearProgramaCreditoValidator v = new();
        Assert.False(v.Validate(new CrearProgramaCreditoCommand("", 1, 1)).IsValid);
    }

    [Fact]
    public void CrearMateriaValidator_RechazaCreditosDistintosDeTres()
    {
        CrearMateriaValidator v = new();
        Assert.False(v.Validate(new CrearMateriaCommand("x", 0, 1, 1)).IsValid);
        Assert.False(v.Validate(new CrearMateriaCommand("x", 4, 1, 1)).IsValid);
        Assert.True(v.Validate(new CrearMateriaCommand("x", 3, 1, 1)).IsValid);
    }
}
