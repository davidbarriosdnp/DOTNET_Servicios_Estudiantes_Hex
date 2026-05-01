SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DROP PROCEDURE IF EXISTS dbo.sp_ProgramaCredito_Insertar, dbo.sp_ProgramaCredito_Actualizar, dbo.sp_ProgramaCredito_Eliminar, dbo.sp_ProgramaCredito_ObtenerPorId, dbo.sp_ProgramaCredito_Listar;
DROP PROCEDURE IF EXISTS dbo.sp_Profesor_Insertar, dbo.sp_Profesor_Actualizar, dbo.sp_Profesor_Eliminar, dbo.sp_Profesor_ObtenerPorId, dbo.sp_Profesor_Listar;
DROP PROCEDURE IF EXISTS dbo.sp_Materia_Insertar, dbo.sp_Materia_Actualizar, dbo.sp_Materia_Eliminar, dbo.sp_Materia_ObtenerPorId, dbo.sp_Materia_ListarPorPrograma;
DROP PROCEDURE IF EXISTS dbo.sp_Estudiante_Insertar, dbo.sp_Estudiante_Actualizar, dbo.sp_Estudiante_Eliminar, dbo.sp_Estudiante_ObtenerPorId, dbo.sp_Estudiante_ListarRegistros;
DROP PROCEDURE IF EXISTS dbo.sp_Inscripcion_Registrar, dbo.sp_Inscripcion_InsertarFila, dbo.sp_Inscripcion_EliminarFila, dbo.sp_Inscripcion_ActualizarMateria, dbo.sp_Inscripcion_ObtenerPorEstudiante;
DROP PROCEDURE IF EXISTS dbo.sp_Companeros_ListarNombresPorMateria;
GO

IF OBJECT_ID(N'dbo.fn_ProgramaCreditoIdPorDefecto', N'FN') IS NOT NULL DROP FUNCTION dbo.fn_ProgramaCreditoIdPorDefecto;
GO

CREATE FUNCTION dbo.fn_ProgramaCreditoIdPorDefecto()
RETURNS INT
AS
BEGIN
    RETURN (SELECT TOP (1) ProgramaCreditoId FROM dbo.ProgramaCredito WHERE Estado = 1 ORDER BY ProgramaCreditoId);
END;
GO

/* --- ProgramaCredito --- */
CREATE PROCEDURE dbo.sp_ProgramaCredito_Insertar
    @Nombre NVARCHAR(120),
    @CreditosPorMateria TINYINT,
    @MaxMateriasPorEstudiante TINYINT,
    @ProgramaCreditoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ProgramaCredito (Nombre, CreditosPorMateria, MaxMateriasPorEstudiante)
    VALUES (@Nombre, @CreditosPorMateria, @MaxMateriasPorEstudiante);
    SET @ProgramaCreditoId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.sp_ProgramaCredito_Actualizar
    @ProgramaCreditoId INT,
    @Nombre NVARCHAR(120),
    @CreditosPorMateria TINYINT,
    @MaxMateriasPorEstudiante TINYINT,
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId)
        THROW 50100, N'Programa no encontrado.', 1;
    UPDATE dbo.ProgramaCredito
    SET Nombre = @Nombre,
        CreditosPorMateria = @CreditosPorMateria,
        MaxMateriasPorEstudiante = @MaxMateriasPorEstudiante,
        Estado = @Estado,
        FechaModificacion = SYSUTCDATETIME()
    WHERE ProgramaCreditoId = @ProgramaCreditoId;
END;
GO

CREATE PROCEDURE dbo.sp_ProgramaCredito_Eliminar
    @ProgramaCreditoId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Estudiante WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50101, N'No se puede inactivar el programa: hay estudiantes activos.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Materia WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50102, N'No se puede inactivar el programa: hay materias activas.', 1;
    UPDATE dbo.ProgramaCredito SET Estado = 0, FechaModificacion = SYSUTCDATETIME() WHERE ProgramaCreditoId = @ProgramaCreditoId;
END;
GO

CREATE PROCEDURE dbo.sp_ProgramaCredito_ObtenerPorId
    @ProgramaCreditoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProgramaCreditoId, Nombre, CreditosPorMateria, MaxMateriasPorEstudiante, FechaRegistro, FechaModificacion, Estado
    FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId;
END;
GO

CREATE PROCEDURE dbo.sp_ProgramaCredito_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProgramaCreditoId, Nombre, CreditosPorMateria, MaxMateriasPorEstudiante, FechaRegistro, FechaModificacion, Estado
    FROM dbo.ProgramaCredito
    WHERE @SoloActivos = 0 OR Estado = 1
    ORDER BY ProgramaCreditoId;
END;
GO

/* --- Profesor --- */
CREATE PROCEDURE dbo.sp_Profesor_Insertar
    @Nombre NVARCHAR(120),
    @ProfesorId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Profesor (Nombre) VALUES (@Nombre);
    SET @ProfesorId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.sp_Profesor_Actualizar
    @ProfesorId INT,
    @Nombre NVARCHAR(120),
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Profesor WHERE ProfesorId = @ProfesorId)
        THROW 50110, N'Profesor no encontrado.', 1;
    UPDATE dbo.Profesor SET Nombre = @Nombre, Estado = @Estado, FechaModificacion = SYSUTCDATETIME() WHERE ProfesorId = @ProfesorId;
END;
GO

CREATE PROCEDURE dbo.sp_Profesor_Eliminar
    @ProfesorId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Materia WHERE ProfesorId = @ProfesorId AND Estado = 1)
        THROW 50111, N'No se puede inactivar: hay materias activas asignadas.', 1;
    UPDATE dbo.Profesor SET Estado = 0, FechaModificacion = SYSUTCDATETIME() WHERE ProfesorId = @ProfesorId;
END;
GO

CREATE PROCEDURE dbo.sp_Profesor_ObtenerPorId
    @ProfesorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProfesorId, Nombre, FechaRegistro, FechaModificacion, Estado FROM dbo.Profesor WHERE ProfesorId = @ProfesorId;
END;
GO

CREATE PROCEDURE dbo.sp_Profesor_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProfesorId, Nombre, FechaRegistro, FechaModificacion, Estado FROM dbo.Profesor
    WHERE @SoloActivos = 0 OR Estado = 1 ORDER BY ProfesorId;
END;
GO

/* --- Materia --- */
CREATE PROCEDURE dbo.sp_Materia_Insertar
    @Nombre NVARCHAR(120),
    @Creditos TINYINT,
    @ProfesorId INT,
    @ProgramaCreditoId INT,
    @MateriaId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Profesor WHERE ProfesorId = @ProfesorId AND Estado = 1)
        THROW 50120, N'Profesor no válido o inactivo.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50121, N'Programa no válido o inactivo.', 1;
    INSERT INTO dbo.Materia (Nombre, Creditos, ProfesorId, ProgramaCreditoId) VALUES (@Nombre, @Creditos, @ProfesorId, @ProgramaCreditoId);
    SET @MateriaId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.sp_Materia_Actualizar
    @MateriaId INT,
    @Nombre NVARCHAR(120),
    @Creditos TINYINT,
    @ProfesorId INT,
    @ProgramaCreditoId INT,
    @Estado TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Materia WHERE MateriaId = @MateriaId)
        THROW 50122, N'Materia no encontrada.', 1;
    UPDATE dbo.Materia
    SET Nombre = @Nombre, Creditos = @Creditos, ProfesorId = @ProfesorId, ProgramaCreditoId = @ProgramaCreditoId,
        Estado = @Estado, FechaModificacion = SYSUTCDATETIME()
    WHERE MateriaId = @MateriaId;
END;
GO

CREATE PROCEDURE dbo.sp_Materia_Eliminar
    @MateriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE MateriaId = @MateriaId AND Estado = 1)
        THROW 50123, N'No se puede inactivar: hay inscripciones activas.', 1;
    UPDATE dbo.Materia SET Estado = 0, FechaModificacion = SYSUTCDATETIME() WHERE MateriaId = @MateriaId;
END;
GO

CREATE PROCEDURE dbo.sp_Materia_ObtenerPorId
    @MateriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MateriaId, m.Nombre, m.Creditos, m.ProfesorId, m.ProgramaCreditoId, m.FechaRegistro, m.FechaModificacion, m.Estado,
           p.Nombre AS NombreProfesor
    FROM dbo.Materia m INNER JOIN dbo.Profesor p ON p.ProfesorId = m.ProfesorId
    WHERE m.MateriaId = @MateriaId;
END;
GO

CREATE PROCEDURE dbo.sp_Materia_ListarPorPrograma
    @ProgramaCreditoId INT = NULL,
    @SoloActivos BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProgramaCreditoId IS NULL SET @ProgramaCreditoId = dbo.fn_ProgramaCreditoIdPorDefecto();
    SELECT m.MateriaId, m.Nombre, m.Creditos, m.ProfesorId, m.ProgramaCreditoId, m.FechaRegistro, m.FechaModificacion, m.Estado, p.Nombre AS NombreProfesor
    FROM dbo.Materia m INNER JOIN dbo.Profesor p ON p.ProfesorId = m.ProfesorId
    WHERE m.ProgramaCreditoId = @ProgramaCreditoId
      AND (@SoloActivos = 0 OR (m.Estado = 1 AND p.Estado = 1))
    ORDER BY m.MateriaId;
END;
GO

/* --- Estudiante --- */
CREATE PROCEDURE dbo.sp_Estudiante_Insertar
    @Nombre NVARCHAR(120),
    @Email NVARCHAR(256),
    @ProgramaCreditoId INT = NULL,
    @EstudianteId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ProgramaCreditoId IS NULL SET @ProgramaCreditoId = dbo.fn_ProgramaCreditoIdPorDefecto();
    IF NOT EXISTS (SELECT 1 FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50001, N'El programa de créditos no existe o está inactivo.', 1;
    INSERT INTO dbo.Estudiante (Nombre, Email, ProgramaCreditoId) VALUES (@Nombre, LOWER(LTRIM(RTRIM(@Email))), @ProgramaCreditoId);
    SET @EstudianteId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.sp_Estudiante_Actualizar
    @EstudianteId INT,
    @Nombre NVARCHAR(120),
    @Email NVARCHAR(256),
    @ProgramaCreditoId INT = NULL,
    @Estado TINYINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Estudiante WHERE EstudianteId = @EstudianteId)
        THROW 50002, N'Estudiante no encontrado.', 1;
    IF @ProgramaCreditoId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaCreditoId AND Estado = 1)
        THROW 50003, N'Programa no válido o inactivo.', 1;
    UPDATE dbo.Estudiante
    SET Nombre = @Nombre,
        Email = LOWER(LTRIM(RTRIM(@Email))),
        ProgramaCreditoId = COALESCE(@ProgramaCreditoId, ProgramaCreditoId),
        Estado = COALESCE(@Estado, Estado),
        FechaModificacion = SYSUTCDATETIME()
    WHERE EstudianteId = @EstudianteId;
END;
GO

CREATE PROCEDURE dbo.sp_Estudiante_Eliminar
    @EstudianteId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Estudiante SET Estado = 0, FechaModificacion = SYSUTCDATETIME() WHERE EstudianteId = @EstudianteId;
END;
GO

CREATE PROCEDURE dbo.sp_Estudiante_ObtenerPorId
    @EstudianteId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EstudianteId, Nombre, Email, ProgramaCreditoId, FechaRegistro, FechaModificacion, Estado
    FROM dbo.Estudiante WHERE EstudianteId = @EstudianteId;
END;
GO

CREATE PROCEDURE dbo.sp_Estudiante_ListarRegistros
    @SoloActivos BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.EstudianteId, e.Nombre, e.Email, e.ProgramaCreditoId, e.FechaRegistro, e.FechaModificacion, e.Estado,
        MateriasInscritas = ISNULL((
            SELECT STRING_AGG(CAST(m.Nombre AS NVARCHAR(MAX)), N', ')
            FROM dbo.InscripcionEstudianteMateria i
            INNER JOIN dbo.Materia m ON m.MateriaId = i.MateriaId AND m.Estado = 1
            WHERE i.EstudianteId = e.EstudianteId AND i.Estado = 1
        ), N'')
    FROM dbo.Estudiante e
    WHERE @SoloActivos = 0 OR e.Estado = 1
    ORDER BY e.EstudianteId;
END;
GO

/* --- Inscripción: reglas profesor único y máximo materias --- */
CREATE PROCEDURE dbo.sp_Inscripcion_Registrar
    @EstudianteId INT,
    @MateriaId1 INT,
    @MateriaId2 INT,
    @MateriaId3 INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ProgramaId INT, @MaxM TINYINT;
    SELECT @ProgramaId = e.ProgramaCreditoId FROM dbo.Estudiante e WHERE e.EstudianteId = @EstudianteId AND e.Estado = 1;
    IF @ProgramaId IS NULL THROW 50010, N'Estudiante no encontrado o inactivo.', 1;
    SELECT @MaxM = MaxMateriasPorEstudiante FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaId AND Estado = 1;
    IF @MateriaId1 = @MateriaId2 OR @MateriaId1 = @MateriaId3 OR @MateriaId2 = @MateriaId3
        THROW 50011, N'Debe seleccionar tres materias distintas.', 1;
    IF (SELECT COUNT(*) FROM (VALUES (@MateriaId1), (@MateriaId2), (@MateriaId3)) t(Mid)
        INNER JOIN dbo.Materia m ON m.MateriaId = t.Mid AND m.ProgramaCreditoId = @ProgramaId AND m.Estado = 1) <> 3
        THROW 50012, N'Una o más materias no existen en el programa del estudiante.', 1;
    IF EXISTS (SELECT ProfesorId FROM dbo.Materia WHERE MateriaId IN (@MateriaId1,@MateriaId2,@MateriaId3) GROUP BY ProfesorId HAVING COUNT(*) > 1)
        THROW 50013, N'No puede inscribirse en más de una materia del mismo profesor.', 1;
    BEGIN TRANSACTION;
    DELETE FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId = @EstudianteId;
    INSERT INTO dbo.InscripcionEstudianteMateria (EstudianteId, MateriaId) VALUES (@EstudianteId,@MateriaId1),(@EstudianteId,@MateriaId2),(@EstudianteId,@MateriaId3);
    IF (SELECT COUNT(*) FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId=@EstudianteId AND Estado=1) <> @MaxM
        THROW 50014, N'La inscripción no cumple la cantidad de materias permitidas.', 1;
    COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE dbo.sp_Inscripcion_InsertarFila
    @EstudianteId INT,
    @MateriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ProgramaId INT, @MaxM TINYINT, @Cnt INT, @ProfNuevo INT;
    SELECT @ProgramaId = e.ProgramaCreditoId FROM dbo.Estudiante e WHERE e.EstudianteId = @EstudianteId AND e.Estado = 1;
    IF @ProgramaId IS NULL THROW 50200, N'Estudiante no encontrado o inactivo.', 1;
    SELECT @MaxM = MaxMateriasPorEstudiante FROM dbo.ProgramaCredito WHERE ProgramaCreditoId = @ProgramaId AND Estado = 1;
    SELECT @Cnt = COUNT(*) FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId = @EstudianteId AND Estado = 1;
    IF @Cnt >= @MaxM THROW 50201, N'Ya alcanzó el máximo de materias activas.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Materia m WHERE m.MateriaId = @MateriaId AND m.ProgramaCreditoId = @ProgramaId AND m.Estado = 1)
        THROW 50202, N'Materia no válida para el programa.', 1;
    SELECT @ProfNuevo = ProfesorId FROM dbo.Materia WHERE MateriaId = @MateriaId;
    IF EXISTS (
        SELECT 1 FROM dbo.InscripcionEstudianteMateria i
        INNER JOIN dbo.Materia m ON m.MateriaId = i.MateriaId AND m.Estado = 1
        WHERE i.EstudianteId = @EstudianteId AND i.Estado = 1 AND m.ProfesorId = @ProfNuevo
    ) THROW 50203, N'Ya tiene una materia activa con ese profesor.', 1;
    IF EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId = @EstudianteId AND MateriaId = @MateriaId AND Estado = 1)
        THROW 50204, N'Ya está inscrito en esa materia.', 1;
    IF EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId = @EstudianteId AND MateriaId = @MateriaId)
        UPDATE dbo.InscripcionEstudianteMateria SET Estado = 1, FechaModificacion = SYSUTCDATETIME() WHERE EstudianteId = @EstudianteId AND MateriaId = @MateriaId;
    ELSE
        INSERT INTO dbo.InscripcionEstudianteMateria (EstudianteId, MateriaId) VALUES (@EstudianteId, @MateriaId);
END;
GO

CREATE PROCEDURE dbo.sp_Inscripcion_EliminarFila
    @EstudianteId INT,
    @MateriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.InscripcionEstudianteMateria SET Estado = 0, FechaModificacion = SYSUTCDATETIME()
    WHERE EstudianteId = @EstudianteId AND MateriaId = @MateriaId AND Estado = 1;
END;
GO

CREATE PROCEDURE dbo.sp_Inscripcion_ActualizarMateria
    @EstudianteId INT,
    @MateriaIdAnterior INT,
    @MateriaIdNueva INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @MateriaIdAnterior = @MateriaIdNueva THROW 50210, N'La materia nueva debe ser distinta.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId=@EstudianteId AND MateriaId=@MateriaIdAnterior AND Estado=1)
        THROW 50211, N'No hay inscripción activa en la materia indicada.', 1;
    DECLARE @ProgramaId INT;
    SELECT @ProgramaId = ProgramaCreditoId FROM dbo.Estudiante WHERE EstudianteId = @EstudianteId AND Estado = 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Materia WHERE MateriaId=@MateriaIdNueva AND ProgramaCreditoId=@ProgramaId AND Estado=1)
        THROW 50212, N'Materia nueva no válida.', 1;
    DECLARE @ProfNuevo INT; SELECT @ProfNuevo = ProfesorId FROM dbo.Materia WHERE MateriaId = @MateriaIdNueva;
    IF EXISTS (
        SELECT 1 FROM dbo.InscripcionEstudianteMateria i
        INNER JOIN dbo.Materia m ON m.MateriaId = i.MateriaId AND m.Estado = 1
        WHERE i.EstudianteId = @EstudianteId AND i.Estado = 1 AND m.ProfesorId = @ProfNuevo AND i.MateriaId <> @MateriaIdAnterior
    ) THROW 50213, N'La materia nueva comparte profesor con otra inscripción activa.', 1;
    BEGIN TRANSACTION;
    UPDATE dbo.InscripcionEstudianteMateria SET Estado = 0, FechaModificacion = SYSUTCDATETIME() WHERE EstudianteId=@EstudianteId AND MateriaId=@MateriaIdAnterior;
    IF EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId=@EstudianteId AND MateriaId=@MateriaIdNueva)
        UPDATE dbo.InscripcionEstudianteMateria SET Estado = 1, FechaModificacion = SYSUTCDATETIME() WHERE EstudianteId=@EstudianteId AND MateriaId=@MateriaIdNueva;
    ELSE
        INSERT INTO dbo.InscripcionEstudianteMateria (EstudianteId, MateriaId) VALUES (@EstudianteId, @MateriaIdNueva);
    COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE dbo.sp_Inscripcion_ObtenerPorEstudiante
    @EstudianteId INT,
    @SoloActivas BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MateriaId, m.Nombre AS NombreMateria, m.Creditos, m.ProfesorId, p.Nombre AS NombreProfesor,
           i.FechaRegistro, i.FechaModificacion, i.Estado
    FROM dbo.InscripcionEstudianteMateria i
    INNER JOIN dbo.Materia m ON m.MateriaId = i.MateriaId
    INNER JOIN dbo.Profesor p ON p.ProfesorId = m.ProfesorId
    WHERE i.EstudianteId = @EstudianteId AND (@SoloActivas = 0 OR i.Estado = 1)
    ORDER BY m.MateriaId;
END;
GO

CREATE PROCEDURE dbo.sp_Companeros_ListarNombresPorMateria
    @EstudianteIdSolicitante INT,
    @MateriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.InscripcionEstudianteMateria WHERE EstudianteId=@EstudianteIdSolicitante AND MateriaId=@MateriaId AND Estado=1)
        THROW 50020, N'El estudiante no está inscrito en esa materia.', 1;
    SELECT e.Nombre
    FROM dbo.InscripcionEstudianteMateria i
    INNER JOIN dbo.Estudiante e ON e.EstudianteId = i.EstudianteId AND e.Estado = 1
    WHERE i.MateriaId = @MateriaId AND i.Estado = 1 AND i.EstudianteId <> @EstudianteIdSolicitante
    ORDER BY e.Nombre;
END;
GO
