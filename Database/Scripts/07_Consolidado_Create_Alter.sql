-- =================================================================================
-- SCRIPT CONSOLIDADO E IDEMPOTENTE: CREATE Y ALTER
-- Diseñado para inicializar la base de datos o modificarla de forma segura
-- =================================================================================

USE [master];
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'ServiciosEstudiantes')
BEGIN
    CREATE DATABASE [ServiciosEstudiantes];
END
GO

USE [ServiciosEstudiantes];
GO

-- ==========================================
-- 1. TABLAS PRINCIPALES (CREATE SI NO EXISTEN)
-- ==========================================

IF OBJECT_ID(N'dbo.ProgramaCredito', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProgramaCredito (
        ProgramaCreditoId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre                  NVARCHAR(120) NOT NULL,
        CreditosPorMateria      TINYINT NOT NULL CONSTRAINT DF_Programa_CreditosMateria DEFAULT (3),
        MaxMateriasPorEstudiante TINYINT NOT NULL CONSTRAINT DF_Programa_MaxMaterias DEFAULT (3),
        FechaRegistro           DATETIME2 NOT NULL CONSTRAINT DF_Programa_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion       DATETIME2 NULL,
        Estado                  TINYINT NOT NULL CONSTRAINT DF_Programa_Estado DEFAULT (1),
        CONSTRAINT CK_Programa_Estado CHECK (Estado IN (0, 1))
    );
END
GO

IF OBJECT_ID(N'dbo.Profesor', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Profesor (
        ProfesorId        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre            NVARCHAR(120) NOT NULL,
        FechaRegistro     DATETIME2 NOT NULL CONSTRAINT DF_Profesor_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion DATETIME2 NULL,
        Estado            TINYINT NOT NULL CONSTRAINT DF_Profesor_Estado DEFAULT (1),
        CONSTRAINT CK_Profesor_Estado CHECK (Estado IN (0, 1))
    );
END
GO

IF OBJECT_ID(N'dbo.Materia', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Materia (
        MateriaId          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre             NVARCHAR(120) NOT NULL,
        Creditos           TINYINT NOT NULL CONSTRAINT DF_Materia_Creditos DEFAULT (3),
        ProfesorId         INT NOT NULL,
        ProgramaCreditoId  INT NOT NULL,
        FechaRegistro      DATETIME2 NOT NULL CONSTRAINT DF_Materia_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion  DATETIME2 NULL,
        Estado             TINYINT NOT NULL CONSTRAINT DF_Materia_Estado DEFAULT (1),
        CONSTRAINT CK_Materia_Estado CHECK (Estado IN (0, 1)),
        CONSTRAINT FK_Materia_Profesor FOREIGN KEY (ProfesorId) REFERENCES dbo.Profesor (ProfesorId),
        CONSTRAINT FK_Materia_Programa FOREIGN KEY (ProgramaCreditoId) REFERENCES dbo.ProgramaCredito (ProgramaCreditoId)
    );
END
GO

IF OBJECT_ID(N'dbo.Estudiante', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Estudiante (
        EstudianteId       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre             NVARCHAR(120) NOT NULL,
        Email              NVARCHAR(256) NOT NULL,
        ProgramaCreditoId  INT NOT NULL,
        FechaRegistro      DATETIME2 NOT NULL CONSTRAINT DF_Estudiante_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion DATETIME2 NULL,
        Estado             TINYINT NOT NULL CONSTRAINT DF_Estudiante_Estado DEFAULT (1),
        CONSTRAINT CK_Estudiante_Estado CHECK (Estado IN (0, 1)),
        CONSTRAINT UQ_Estudiante_Email UNIQUE (Email),
        CONSTRAINT FK_Estudiante_Programa FOREIGN KEY (ProgramaCreditoId) REFERENCES dbo.ProgramaCredito (ProgramaCreditoId)
    );
END
GO

IF OBJECT_ID(N'dbo.InscripcionEstudianteMateria', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InscripcionEstudianteMateria (
        EstudianteId       INT NOT NULL,
        MateriaId          INT NOT NULL,
        FechaRegistro      DATETIME2 NOT NULL CONSTRAINT DF_Insc_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion DATETIME2 NULL,
        Estado             TINYINT NOT NULL CONSTRAINT DF_Insc_Estado DEFAULT (1),
        CONSTRAINT PK_Inscripcion PRIMARY KEY (EstudianteId, MateriaId),
        CONSTRAINT CK_Insc_Estado CHECK (Estado IN (0, 1)),
        CONSTRAINT FK_Inscripcion_Estudiante FOREIGN KEY (EstudianteId) REFERENCES dbo.Estudiante (EstudianteId) ON DELETE CASCADE,
        CONSTRAINT FK_Inscripcion_Materia FOREIGN KEY (MateriaId) REFERENCES dbo.Materia (MateriaId)
    );
END
GO

IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario (
        UsuarioId          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NombreUsuario      NVARCHAR(120) NOT NULL,
        Email              NVARCHAR(256) NOT NULL,
        PasswordHash       NVARCHAR(500) NOT NULL,
        Rol                NVARCHAR(64) NOT NULL CONSTRAINT DF_Usuario_Rol DEFAULT (N'Estudiante'),
        FechaRegistro      DATETIME2 NOT NULL CONSTRAINT DF_Usuario_FechaReg DEFAULT (SYSUTCDATETIME()),
        FechaModificacion DATETIME2 NULL,
        Estado             TINYINT NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT (1),
        CONSTRAINT CK_Usuario_Estado CHECK (Estado IN (0, 1)),
        CONSTRAINT UQ_Usuario_Email UNIQUE (Email)
    );
END
GO

-- ==========================================
-- 2. ALTERACIONES Y EVOLUCIÓN (MIGRACIÓN / ALTER)
-- ==========================================

-- Añadir UsuarioId a Estudiante si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Estudiante') AND name = N'UsuarioId')
BEGIN
    ALTER TABLE dbo.Estudiante ADD UsuarioId INT NULL;
END
GO

-- Añadir constraint FK_Estudiante_Usuario si no existe
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Estudiante_Usuario')
BEGIN
    ALTER TABLE dbo.Estudiante
        ADD CONSTRAINT FK_Estudiante_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario (UsuarioId);
END
GO

-- Crear índice único UX_Estudiante_UsuarioId si no existe
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UX_Estudiante_UsuarioId' AND object_id = OBJECT_ID(N'dbo.Estudiante'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Estudiante_UsuarioId
        ON dbo.Estudiante (UsuarioId)
        WHERE UsuarioId IS NOT NULL;
END
GO

-- ==========================================
-- 3. PERMISOS: ROLES DE LECTURA Y ESCRITURA
-- ==========================================

-- Crear rol de lectura si no existe
IF DATABASE_PRINCIPAL_ID('Db_Lectores') IS NULL
BEGIN
    CREATE ROLE Db_Lectores;
END
GRANT SELECT TO Db_Lectores;
GO

-- Crear rol de escritura si no existe
IF DATABASE_PRINCIPAL_ID('Db_Escritores') IS NULL
BEGIN
    CREATE ROLE Db_Escritores;
END
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE TO Db_Escritores;
GO
