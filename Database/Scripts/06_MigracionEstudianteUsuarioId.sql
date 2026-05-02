-- Migración incremental: columna UsuarioId y SPs de registro / lookup.
-- Tras ejecutar este script, vuelva a ejecutar 03_Procedimientos.sql completo para alinear
-- sp_Estudiante_Insertar, sp_Estudiante_ObtenerPorId y las reglas de sp_Materia_* con el código actual del API.

SET NOCOUNT ON;
GO

IF COL_LENGTH(N'dbo.Estudiante', N'UsuarioId') IS NULL
BEGIN
    ALTER TABLE dbo.Estudiante ADD UsuarioId INT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Estudiante_Usuario'
)
BEGIN
    ALTER TABLE dbo.Estudiante
        ADD CONSTRAINT FK_Estudiante_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario (UsuarioId);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = N'UX_Estudiante_UsuarioId' AND object_id = OBJECT_ID(N'dbo.Estudiante')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Estudiante_UsuarioId
        ON dbo.Estudiante (UsuarioId)
        WHERE UsuarioId IS NOT NULL;
END;
GO

DROP PROCEDURE IF EXISTS dbo.sp_Estudiante_RegistroPublico, dbo.sp_Estudiante_ObtenerIdPorUsuario;
GO

CREATE PROCEDURE dbo.sp_Estudiante_ObtenerIdPorUsuario
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EstudianteId FROM dbo.Estudiante WHERE UsuarioId = @UsuarioId AND Estado = 1;
END;
GO

CREATE PROCEDURE dbo.sp_Estudiante_RegistroPublico
    @NombreUsuario NVARCHAR(120),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(500),
    @NombreCompleto NVARCHAR(120),
    @ProgramaCreditoId INT,
    @UsuarioId INT OUTPUT,
    @EstudianteId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @EmailNorm NVARCHAR(256) = LOWER(LTRIM(RTRIM(@Email)));
    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @EmailNorm OR NombreUsuario = @NombreUsuario)
        THROW 50300, N'El correo o nombre de usuario ya está registrado.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Estudiante WHERE Email = @EmailNorm)
        THROW 50301, N'El correo ya está registrado como estudiante.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50302, N'Programa de créditos no válido o inactivo.', 1;
    BEGIN TRANSACTION;
    INSERT INTO dbo.Usuario (NombreUsuario, Email, PasswordHash, Rol)
    VALUES (@NombreUsuario, @EmailNorm, @PasswordHash, N'Estudiante');
    SET @UsuarioId = CAST(SCOPE_IDENTITY() AS INT);
    INSERT INTO dbo.Estudiante (Nombre, Email, ProgramaCreditoId, UsuarioId)
    VALUES (@NombreCompleto, @EmailNorm, @ProgramaCreditoId, @UsuarioId);
    SET @EstudianteId = CAST(SCOPE_IDENTITY() AS INT);
    COMMIT TRANSACTION;
END;
GO
