using Microsoft.AspNetCore.Identity;
using Moq;
using Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Comandos;
using Servicios_Estudiantes.Aplicacion.CasosUso.Usuarios.Consultas;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Pruebas.Aplicacion;

public sealed class CasosUsoUsuariosPruebas
{
    private static readonly DateTime FechaBase = new(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task CrearUsuario_GuardaHash()
    {
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.InsertarUsuarioAsync("u", "u@test.dev", It.IsAny<string>(), "Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);

        PasswordHasher<string> hasher = new();
        CrearUsuarioCommandHandler sut = new(repo.Object, hasher);
        Respuesta<int> resp = await sut.Handle(new CrearUsuarioCommand("u", "u@test.dev", "Password12", "Admin"), CancellationToken.None);

        Assert.Equal(15, resp.Resultado);
        repo.Verify(r => r.InsertarUsuarioAsync(
            "u",
            "u@test.dev",
            It.Is<string>(h => h != "Password12" && h.Length > 20),
            "Admin",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarUsuario_SinPassword_NoLlamaActualizarPassword()
    {
        UsuarioDetalleDto actual = new(3, "old", "old@test.dev", "Estudiante", FechaBase, null, 1);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(actual);

        ActualizarUsuarioCommandHandler sut = new(repo.Object, new PasswordHasher<string>());
        await sut.Handle(new ActualizarUsuarioCommand(3, "new", "new@test.dev", "Estudiante", 1, null), CancellationToken.None);

        repo.Verify(r => r.ActualizarUsuarioAsync(3, "new", "new@test.dev", "Estudiante", 1, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.ActualizarPasswordAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ActualizarUsuario_ConPassword_ActualizaHash()
    {
        UsuarioDetalleDto actual = new(3, "u", "u@test.dev", "Estudiante", FechaBase, null, 1);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(actual);

        PasswordHasher<string> hasher = new();
        ActualizarUsuarioCommandHandler sut = new(repo.Object, hasher);
        await sut.Handle(new ActualizarUsuarioCommand(3, "u", "u@test.dev", "Estudiante", 1, "NuevaPass1"), CancellationToken.None);

        repo.Verify(r => r.ActualizarPasswordAsync(3, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarUsuario_NoExiste_Lanza()
    {
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((UsuarioDetalleDto?)null);

        ActualizarUsuarioCommandHandler sut = new(repo.Object, new PasswordHasher<string>());
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ActualizarUsuarioCommand(99, "a", "a@test.dev", "Estudiante", 1, null), CancellationToken.None));
    }

    [Fact]
    public async Task EliminarUsuario_RevocaRefresh()
    {
        Mock<IRepositorioUsuarios> repo = new();
        EliminarUsuarioCommandHandler sut = new(repo.Object);
        await sut.Handle(new EliminarUsuarioCommand(7), CancellationToken.None);

        repo.Verify(r => r.EliminarUsuarioAsync(7, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.RevocarTodosRefreshUsuarioAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObtenerUsuarioPorId_NoExiste_Lanza()
    {
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((UsuarioDetalleDto?)null);

        ObtenerUsuarioPorIdQueryHandler sut = new(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.Handle(new ObtenerUsuarioPorIdQuery(5), CancellationToken.None));
    }

    [Fact]
    public async Task ObtenerUsuarioPorId_Existe_DevuelveOk()
    {
        UsuarioDetalleDto u = new(5, "x", "x@test.dev", "Admin", FechaBase, null, 1);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(u);

        ObtenerUsuarioPorIdQueryHandler sut = new(repo.Object);
        Respuesta<UsuarioDetalleDto> resp = await sut.Handle(new ObtenerUsuarioPorIdQuery(5), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        Assert.Equal("x", resp.Resultado!.NombreUsuario);
    }

    [Fact]
    public async Task ListarUsuarios_DevuelveLista()
    {
        UsuarioListaDto fila = new(1, "a", "a@test.dev", "Estudiante", FechaBase, null, 1);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ListarUsuariosAsync(true, It.IsAny<CancellationToken>())).ReturnsAsync([fila]);

        ListarUsuariosQueryHandler sut = new(repo.Object);
        Respuesta<IReadOnlyList<UsuarioListaDto>> resp =
            await sut.Handle(new ListarUsuariosQuery(true), CancellationToken.None);

        Assert.Single(resp.Resultado!);
    }

    [Fact]
    public void CrearUsuarioCommandValidator_RechazaPasswordCorta()
    {
        CrearUsuarioCommandValidator v = new();
        Assert.False(v.Validate(new CrearUsuarioCommand("u", "u@test.dev", "short", "Rol")).IsValid);
    }

    [Fact]
    public void ActualizarUsuarioCommandValidator_PasswordOpcionalInvalida_Falla()
    {
        ActualizarUsuarioCommandValidator v = new();
        Assert.False(v.Validate(new ActualizarUsuarioCommand(1, "u", "u@test.dev", "Rol", 1, "corta")).IsValid);
    }
}
