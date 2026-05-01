SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DROP PROCEDURE IF EXISTS dbo.sp_RefreshToken_RevocarTodosUsuario, dbo.sp_RefreshToken_RevocarPorHash, dbo.sp_RefreshToken_ObtenerPorHash, dbo.sp_RefreshToken_Insertar;
DROP PROCEDURE IF EXISTS dbo.sp_Usuario_ActualizarPassword, dbo.sp_Usuario_Eliminar, dbo.sp_Usuario_Actualizar, dbo.sp_Usuario_Insertar;
DROP PROCEDURE IF EXISTS dbo.sp_Usuario_ObtenerPorEmail, dbo.sp_Usuario_ObtenerPorNombreUsuario, dbo.sp_Usuario_ObtenerPorId, dbo.sp_Usuario_Listar;
GO

CREATE PROCEDURE dbo.sp_Usuario_Listar
    @SoloActivos BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        Rol,
        FechaRegistro,
        FechaModificacion,
        Estado
    FROM dbo.Usuario
    WHERE (@SoloActivos = 0 OR Estado = 1)
    ORDER BY UsuarioId;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_ObtenerPorId
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        Rol,
        FechaRegistro,
        FechaModificacion,
        Estado
    FROM dbo.Usuario
    WHERE UsuarioId = @UsuarioId;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_ObtenerPorNombreUsuario
    @NombreUsuario NVARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        PasswordHash,
        Rol,
        FechaRegistro,
        FechaModificacion,
        Estado
    FROM dbo.Usuario
    WHERE NombreUsuario = @NombreUsuario;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_ObtenerPorEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        PasswordHash,
        Rol,
        FechaRegistro,
        FechaModificacion,
        Estado
    FROM dbo.Usuario
    WHERE Email = @Email;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_Insertar
    @NombreUsuario NVARCHAR(120),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(500),
    @Rol NVARCHAR(64),
    @UsuarioId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Usuario (NombreUsuario, Email, PasswordHash, Rol)
    VALUES (@NombreUsuario, @Email, @PasswordHash, @Rol);
    SET @UsuarioId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_Actualizar
    @UsuarioId INT,
    @NombreUsuario NVARCHAR(120),
    @Email NVARCHAR(256),
    @Rol NVARCHAR(64),
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario
    SET
        NombreUsuario = @NombreUsuario,
        Email = @Email,
        Rol = @Rol,
        Estado = @Estado,
        FechaModificacion = SYSUTCDATETIME()
    WHERE UsuarioId = @UsuarioId;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_ActualizarPassword
    @UsuarioId INT,
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario
    SET
        PasswordHash = @PasswordHash,
        FechaModificacion = SYSUTCDATETIME()
    WHERE UsuarioId = @UsuarioId;
END;
GO

CREATE PROCEDURE dbo.sp_Usuario_Eliminar
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario
    SET Estado = 0, FechaModificacion = SYSUTCDATETIME()
    WHERE UsuarioId = @UsuarioId;
END;
GO

CREATE PROCEDURE dbo.sp_RefreshToken_Insertar
    @UsuarioId INT,
    @TokenHash NVARCHAR(64),
    @ExpiresUtc DATETIME2,
    @RefreshTokenId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.RefreshToken (UsuarioId, TokenHash, ExpiresUtc)
    VALUES (@UsuarioId, @TokenHash, @ExpiresUtc);
    SET @RefreshTokenId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE PROCEDURE dbo.sp_RefreshToken_ObtenerPorHash
    @TokenHash NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        rt.RefreshTokenId,
        rt.UsuarioId,
        rt.ExpiresUtc,
        rt.RevokedUtc
    FROM dbo.RefreshToken AS rt
    WHERE rt.TokenHash = @TokenHash
      AND rt.RevokedUtc IS NULL
      AND rt.ExpiresUtc > SYSUTCDATETIME();
END;
GO

CREATE PROCEDURE dbo.sp_RefreshToken_RevocarPorHash
    @TokenHash NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RefreshToken
    SET RevokedUtc = SYSUTCDATETIME()
    WHERE TokenHash = @TokenHash
      AND RevokedUtc IS NULL;
END;
GO

CREATE PROCEDURE dbo.sp_RefreshToken_RevocarTodosUsuario
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RefreshToken
    SET RevokedUtc = SYSUTCDATETIME()
    WHERE UsuarioId = @UsuarioId
      AND RevokedUtc IS NULL;
END;
GO
