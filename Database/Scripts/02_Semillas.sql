SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DELETE FROM dbo.InscripcionEstudianteMateria;
DELETE FROM dbo.Estudiante;
DELETE FROM dbo.Materia;
DELETE FROM dbo.Profesor;
DELETE FROM dbo.ProgramaCredito;

SET IDENTITY_INSERT dbo.ProgramaCredito ON;
INSERT INTO dbo.ProgramaCredito (ProgramaCreditoId, Nombre, CreditosPorMateria, MaxMateriasPorEstudiante, FechaRegistro, FechaModificacion, Estado)
VALUES (1, N'Programa de créditos académicos', 3, 3, SYSUTCDATETIME(), NULL, 1);
SET IDENTITY_INSERT dbo.ProgramaCredito OFF;
GO

SET IDENTITY_INSERT dbo.Profesor ON;
INSERT INTO dbo.Profesor (ProfesorId, Nombre, FechaRegistro, FechaModificacion, Estado) VALUES
 (1, N'Prof. Ana García', SYSUTCDATETIME(), NULL, 1),
 (2, N'Prof. Luis Martínez', SYSUTCDATETIME(), NULL, 1),
 (3, N'Prof. Carmen Ruiz', SYSUTCDATETIME(), NULL, 1),
 (4, N'Prof. Jorge Soto', SYSUTCDATETIME(), NULL, 1),
 (5, N'Prof. Elena Vargas', SYSUTCDATETIME(), NULL, 1);
SET IDENTITY_INSERT dbo.Profesor OFF;
GO

SET IDENTITY_INSERT dbo.Materia ON;
INSERT INTO dbo.Materia (MateriaId, Nombre, Creditos, ProfesorId, ProgramaCreditoId, FechaRegistro, FechaModificacion, Estado) VALUES
 (1,  N'Álgebra Lineal',           3, 1, 1, SYSUTCDATETIME(), NULL, 1),
 (2,  N'Cálculo I',               3, 1, 1, SYSUTCDATETIME(), NULL, 1),
 (3,  N'Programación I',          3, 2, 1, SYSUTCDATETIME(), NULL, 1),
 (4,  N'Estructuras de Datos',    3, 2, 1, SYSUTCDATETIME(), NULL, 1),
 (5,  N'Bases de Datos',          3, 3, 1, SYSUTCDATETIME(), NULL, 1),
 (6,  N'Sistemas Operativos',     3, 3, 1, SYSUTCDATETIME(), NULL, 1),
 (7,  N'Redes',                   3, 4, 1, SYSUTCDATETIME(), NULL, 1),
 (8,  N'Seguridad Informática',   3, 4, 1, SYSUTCDATETIME(), NULL, 1),
 (9,  N'Ingeniería de Software',  3, 5, 1, SYSUTCDATETIME(), NULL, 1),
 (10, N'Gestión de Proyectos',    3, 5, 1, SYSUTCDATETIME(), NULL, 1);
SET IDENTITY_INSERT dbo.Materia OFF;
GO
