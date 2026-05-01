using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Moq;
using Servicios_Estudiantes.Aplicacion.CasosUso.Autenticacion;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;
using Servicios_Estudiantes.Aplicacion.Utilidades;

namespace Servicios_Estudiantes.Pruebas.Aplicacion;

public sealed class CasosUsoAutenticacionPruebas
{
    private static readonly DateTime FechaBase = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task IniciarSesion_UsuarioInexistente_DevuelveFallo()
    {
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerPorNombreUsuarioAsync("x", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioCredencialDto?)null);

        IniciarSesionCommandHandler sut = new(repo.Object, new PasswordHasher<string>(), Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new IniciarSesionCommand("x", "pwd"), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
        Assert.Contains("Credenciales", resp.Mensaje ?? string.Empty);
    }

    [Fact]
    public async Task IniciarSesion_UsuarioInactivo_DevuelveFallo()
    {
        UsuarioCredencialDto usuario = new(1, "u", "u@test.dev", "hash", "Estudiante", FechaBase, null, 0);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerPorNombreUsuarioAsync("u", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        IniciarSesionCommandHandler sut = new(repo.Object, new PasswordHasher<string>(), Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new IniciarSesionCommand("u", "pwd"), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
    }

    [Fact]
    public async Task IniciarSesion_PasswordIncorrecto_DevuelveFallo()
    {
        PasswordHasher<string> hasher = new();
        string hash = hasher.HashPassword(string.Empty, "buena");
        UsuarioCredencialDto usuario = new(1, "u", "u@test.dev", hash, "Estudiante", FechaBase, null, 1);

        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerPorNombreUsuarioAsync("u", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        IniciarSesionCommandHandler sut = new(repo.Object, hasher, Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new IniciarSesionCommand("u", "otra"), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
    }

    [Fact]
    public async Task IniciarSesion_Exitoso_GuardaRefresh()
    {
        PasswordHasher<string> hasher = new();
        string hash = hasher.HashPassword(string.Empty, "Secret123!");
        UsuarioCredencialDto usuario = new(9, "luis", "l@test.dev", hash, "Admin", FechaBase, null, 1);

        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerPorNombreUsuarioAsync("luis", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        Mock<IGeneradorTokensJwt> gen = new();
        DateTime expAcceso = DateTime.UtcNow.AddMinutes(30);
        gen.Setup(g => g.CrearTokenAcceso(9, "luis", "Admin"))
            .Returns(new ResultadoEmisionTokenAcceso("access-token", "jid", expAcceso));
        gen.Setup(g => g.CrearTokenRenovacion()).Returns("refresh-plano");
        gen.Setup(g => g.CalcularExpiracionRenovacionUtc()).Returns(DateTime.UtcNow.AddDays(1));

        IniciarSesionCommandHandler sut = new(repo.Object, hasher, gen.Object);
        Respuesta<TokenParDto> resp = await sut.Handle(new IniciarSesionCommand("luis", "Secret123!"), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        Assert.NotNull(resp.Resultado);
        Assert.Equal("access-token", resp.Resultado!.TokenAcceso);
        Assert.Equal("refresh-plano", resp.Resultado.TokenRenovacion);

        string esperadoHash = HashTokenRenovacion.AHexMinuscula("refresh-plano");
        repo.Verify(r => r.InsertarRefreshTokenAsync(9, esperadoHash, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IniciarSesion_RehashNecesario_ActualizaPassword()
    {
        Mock<IPasswordHasher<string>> mockHasher = new();
        mockHasher.Setup(h => h.VerifyHashedPassword(string.Empty, "stored", "Secret123!"))
            .Returns(PasswordVerificationResult.SuccessRehashNeeded);
        mockHasher.Setup(h => h.HashPassword(string.Empty, "Secret123!")).Returns("nuevo-hash");

        UsuarioCredencialDto usuario = new(3, "u", "u@test.dev", "stored", "Estudiante", FechaBase, null, 1);
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerPorNombreUsuarioAsync("u", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

        Mock<IGeneradorTokensJwt> gen = new();
        gen.Setup(g => g.CrearTokenAcceso(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ResultadoEmisionTokenAcceso("t", "j", DateTime.UtcNow.AddMinutes(5)));
        gen.Setup(g => g.CrearTokenRenovacion()).Returns("r");
        gen.Setup(g => g.CalcularExpiracionRenovacionUtc()).Returns(DateTime.UtcNow.AddDays(1));

        IniciarSesionCommandHandler sut = new(repo.Object, mockHasher.Object, gen.Object);
        Respuesta<TokenParDto> resp = await sut.Handle(new IniciarSesionCommand("u", "Secret123!"), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        repo.Verify(r => r.ActualizarPasswordAsync(3, "nuevo-hash", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefrescarToken_RefreshValidoPeroUsuarioEliminado_DevuelveFallo()
    {
        string refresh = "tok";
        string hash = HashTokenRenovacion.AHexMinuscula(refresh);
        RefreshTokenValidoDto fila = new(1, 99, DateTime.UtcNow.AddHours(1));

        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerRefreshValidoPorHashAsync(hash, It.IsAny<CancellationToken>())).ReturnsAsync(fila);
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((UsuarioDetalleDto?)null);

        RefrescarTokenCommandHandler sut = new(repo.Object, Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new RefrescarTokenCommand(refresh), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
    }

    [Fact]
    public async Task RefrescarToken_HashInvalido_DevuelveFallo()
    {
        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerRefreshValidoPorHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshTokenValidoDto?)null);

        RefrescarTokenCommandHandler sut = new(repo.Object, Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new RefrescarTokenCommand("cualquiera"), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
    }

    [Fact]
    public async Task RefrescarToken_UsuarioInactivo_DevuelveFallo()
    {
        string refresh = "token-plano";
        string hash = HashTokenRenovacion.AHexMinuscula(refresh);
        RefreshTokenValidoDto fila = new(1, 5, DateTime.UtcNow.AddHours(1));

        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerRefreshValidoPorHashAsync(hash, It.IsAny<CancellationToken>())).ReturnsAsync(fila);
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioDetalleDto(5, "u", "e", "Rol", FechaBase, null, 0));

        RefrescarTokenCommandHandler sut = new(repo.Object, Mock.Of<IGeneradorTokensJwt>());
        Respuesta<TokenParDto> resp = await sut.Handle(new RefrescarTokenCommand(refresh), CancellationToken.None);

        Assert.False(resp.OperacionExitosa);
    }

    [Fact]
    public async Task RefrescarToken_Exitoso_RotaRefresh()
    {
        string viejo = "refresh-viejo";
        string hashViejo = HashTokenRenovacion.AHexMinuscula(viejo);
        RefreshTokenValidoDto fila = new(10, 2, DateTime.UtcNow.AddHours(2));

        Mock<IRepositorioUsuarios> repo = new();
        repo.Setup(r => r.ObtenerRefreshValidoPorHashAsync(hashViejo, It.IsAny<CancellationToken>())).ReturnsAsync(fila);
        repo.Setup(r => r.ObtenerUsuarioPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioDetalleDto(2, "ana", "a@test.dev", "Estudiante", FechaBase, null, 1));

        Mock<IGeneradorTokensJwt> gen = new();
        gen.Setup(g => g.CrearTokenAcceso(2, "ana", "Estudiante"))
            .Returns(new ResultadoEmisionTokenAcceso("acc", "jti", DateTime.UtcNow.AddMinutes(10)));
        gen.Setup(g => g.CrearTokenRenovacion()).Returns("refresh-nuevo");
        gen.Setup(g => g.CalcularExpiracionRenovacionUtc()).Returns(DateTime.UtcNow.AddDays(7));

        RefrescarTokenCommandHandler sut = new(repo.Object, gen.Object);
        Respuesta<TokenParDto> resp = await sut.Handle(new RefrescarTokenCommand(viejo), CancellationToken.None);

        Assert.True(resp.OperacionExitosa);
        repo.Verify(r => r.RevocarRefreshPorHashAsync(hashViejo, It.IsAny<CancellationToken>()), Times.Once);
        string hashNuevo = HashTokenRenovacion.AHexMinuscula("refresh-nuevo");
        repo.Verify(r => r.InsertarRefreshTokenAsync(2, hashNuevo, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CerrarSesion_ConJwtValido_RegistraListaNegraYRevocaRefresh()
    {
        Mock<IJwtListaNegra> lista = new();
        Mock<IRepositorioUsuarios> repo = new();

        Servicios_Estudiantes.Api.Configuraciones.JwtOpciones opciones = new()
        {
            Emisor = "test",
            Audiencia = "test",
            ClaveSecreta = new string('a', 32),
            MinutosValidezAcceso = 60
        };
        Microsoft.Extensions.Options.IOptions<Servicios_Estudiantes.Api.Configuraciones.JwtOpciones> opt =
            Microsoft.Extensions.Options.Options.Create(opciones);
        Servicios_Estudiantes.Api.Seguridad.FirmaSymmetricaJwt firma = new(opt);
        Servicios_Estudiantes.Api.Seguridad.GeneradorTokensJwt generadorReal = new(firma);
        ResultadoEmisionTokenAcceso emitido = generadorReal.CrearTokenAcceso(1, "usuario", "Estudiante");

        string refresh = "mi-refresh";
        CerrarSesionCommandHandler sut = new(repo.Object, lista.Object);
        await sut.Handle(new CerrarSesionCommand("Bearer " + emitido.Token, refresh), CancellationToken.None);

        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(emitido.Token);
        string? jti = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        lista.Verify(l => l.RegistrarRevocacionAsync(jti, jwt.ValidTo, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.RevocarRefreshPorHashAsync(HashTokenRenovacion.AHexMinuscula(refresh), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CerrarSesion_JwtIlegible_AunRevocaRefresh()
    {
        Mock<IJwtListaNegra> lista = new();
        Mock<IRepositorioUsuarios> repo = new();
        CerrarSesionCommandHandler sut = new(repo.Object, lista.Object);

        await sut.Handle(new CerrarSesionCommand("Bearer no-es-un-jwt", "otro"), CancellationToken.None);

        lista.Verify(l => l.RegistrarRevocacionAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(r => r.RevocarRefreshPorHashAsync(HashTokenRenovacion.AHexMinuscula("otro"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CerrarSesion_SinDatos_CompletaOk()
    {
        CerrarSesionCommandHandler sut = new(Mock.Of<IRepositorioUsuarios>(), Mock.Of<IJwtListaNegra>());
        Respuesta<bool> r = await sut.Handle(new CerrarSesionCommand(null, null), CancellationToken.None);
        Assert.True(r.OperacionExitosa);
    }

    [Fact]
    public void IniciarSesionCommandValidator_RechazaVacio()
    {
        IniciarSesionCommandValidator v = new();
        Assert.False(v.Validate(new IniciarSesionCommand("", "")).IsValid);
    }

    [Fact]
    public void RefrescarTokenCommandValidator_RechazaVacio()
    {
        RefrescarTokenCommandValidator v = new();
        Assert.False(v.Validate(new RefrescarTokenCommand("")).IsValid);
    }
}
