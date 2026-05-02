SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.RefreshToken', N'U') IS NOT NULL
    DROP TABLE dbo.RefreshToken;
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NOT NULL
    DROP TABLE dbo.Usuario;
IF OBJECT_ID(N'dbo.InscripcionEstudianteMateria', N'U') IS NOT NULL
    DROP TABLE dbo.InscripcionEstudianteMateria;
IF OBJECT_ID(N'dbo.Estudiante', N'U') IS NOT NULL
    DROP TABLE dbo.Estudiante;
IF OBJECT_ID(N'dbo.Materia', N'U') IS NOT NULL
    DROP TABLE dbo.Materia;
IF OBJECT_ID(N'dbo.Profesor', N'U') IS NOT NULL
    DROP TABLE dbo.Profesor;
IF OBJECT_ID(N'dbo.ProgramaCredito', N'U') IS NOT NULL
    DROP TABLE dbo.ProgramaCredito;
GO

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
GO

CREATE TABLE dbo.Profesor (
    ProfesorId        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre            NVARCHAR(120) NOT NULL,
    FechaRegistro     DATETIME2 NOT NULL CONSTRAINT DF_Profesor_FechaReg DEFAULT (SYSUTCDATETIME()),
    FechaModificacion DATETIME2 NULL,
    Estado            TINYINT NOT NULL CONSTRAINT DF_Profesor_Estado DEFAULT (1),
    CONSTRAINT CK_Profesor_Estado CHECK (Estado IN (0, 1))
);
GO

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
GO

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
GO

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
GO

CREATE NONCLUSTERED INDEX IX_Inscripcion_Materia ON dbo.InscripcionEstudianteMateria (MateriaId) INCLUDE (EstudianteId, Estado);
CREATE NONCLUSTERED INDEX IX_Inscripcion_EstudianteActivo ON dbo.InscripcionEstudianteMateria (EstudianteId) WHERE Estado = 1;
GO

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
GO

CREATE TABLE dbo.RefreshToken (
    RefreshTokenId     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UsuarioId          INT NOT NULL,
    TokenHash          NVARCHAR(64) NOT NULL,
    ExpiresUtc         DATETIME2 NOT NULL,
    RevokedUtc         DATETIME2 NULL,
    CreadoUtc          DATETIME2 NOT NULL CONSTRAINT DF_Refresh_Creado DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Refresh_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario (UsuarioId) ON DELETE CASCADE,
    CONSTRAINT UQ_Refresh_TokenHash UNIQUE (TokenHash)
);
GO

CREATE NONCLUSTERED INDEX IX_Refresh_Usuario ON dbo.RefreshToken (UsuarioId) INCLUDE (ExpiresUtc, RevokedUtc);
GO

ALTER TABLE dbo.Estudiante ADD UsuarioId INT NULL;
GO

ALTER TABLE dbo.Estudiante
    ADD CONSTRAINT FK_Estudiante_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario (UsuarioId);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Estudiante_UsuarioId
    ON dbo.Estudiante (UsuarioId)
    WHERE UsuarioId IS NOT NULL;
GO
