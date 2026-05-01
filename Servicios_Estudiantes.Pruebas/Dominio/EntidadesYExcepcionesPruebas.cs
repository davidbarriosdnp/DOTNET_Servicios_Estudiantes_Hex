using Servicios_Estudiantes.Dominio.Entidades;
using Servicios_Estudiantes.Dominio.Enumeraciones;
using Servicios_Estudiantes.Dominio.Excepciones;

namespace Servicios_Estudiantes.Pruebas.Dominio;

public sealed class EntidadesYExcepcionesPruebas
{
    private static readonly DateTime FechaEjemplo = new(2025, 3, 15, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Estudiante_AsignacionPropiedades()
    {
        Estudiante e = new()
        {
            EstudianteId = 7,
            Nombre = "Ana",
            Email = "ana@test.dev",
            ProgramaCreditoId = 2,
            FechaRegistro = FechaEjemplo,
            FechaModificacion = null,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(7, e.EstudianteId);
        Assert.Equal("Ana", e.Nombre);
        Assert.Equal(EstadoRegistro.Activo, e.Estado);
    }

    [Fact]
    public void ProgramaCredito_AsignacionPropiedades()
    {
        ProgramaCredito p = new()
        {
            ProgramaCreditoId = 1,
            Nombre = "Ing.",
            CreditosPorMateria = 3,
            MaxMateriasPorEstudiante = 10,
            FechaRegistro = FechaEjemplo,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(10, p.MaxMateriasPorEstudiante);
    }

    [Fact]
    public void Materia_AsignacionPropiedades()
    {
        Materia m = new()
        {
            MateriaId = 5,
            Nombre = "Algoritmos",
            Creditos = 4,
            ProfesorId = 9,
            ProgramaCreditoId = 1,
            FechaRegistro = FechaEjemplo,
            Estado = EstadoRegistro.Inactivo
        };

        Assert.Equal(9, m.ProfesorId);
        Assert.Equal(EstadoRegistro.Inactivo, m.Estado);
    }

    [Fact]
    public void Profesor_AsignacionPropiedades()
    {
        Profesor p = new()
        {
            ProfesorId = 3,
            Nombre = "Luis",
            FechaRegistro = FechaEjemplo,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal("Luis", p.Nombre);
    }

    [Fact]
    public void Usuario_AsignacionPropiedades()
    {
        Usuario u = new()
        {
            UsuarioId = 2,
            NombreUsuario = "admin",
            Email = "admin@test.dev",
            Rol = "Admin",
            Estado = (byte)EstadoRegistro.Activo
        };

        Assert.Equal("Admin", u.Rol);
        Assert.Equal((byte)1, u.Estado);
    }

    [Fact]
    public void InscripcionEstudianteMateria_AsignacionPropiedades()
    {
        InscripcionEstudianteMateria i = new()
        {
            EstudianteId = 1,
            MateriaId = 2,
            FechaRegistro = FechaEjemplo,
            Estado = EstadoRegistro.Activo
        };

        Assert.Equal(1, i.EstudianteId);
        Assert.Equal(2, i.MateriaId);
    }

    [Fact]
    public void EstadoRegistro_ValoresByte()
    {
        Assert.Equal((byte)0, (byte)EstadoRegistro.Inactivo);
        Assert.Equal((byte)1, (byte)EstadoRegistro.Activo);
    }

    [Fact]
    public void ExcepcionDominio_MensajeYInterna()
    {
        Excepcion sin = new();
        Assert.NotNull(sin);

        Excepcion con = new("regla");
        Assert.Equal("regla", con.Message);

        InvalidOperationException inner = new("inner");
        Excepcion anidada = new("outer", inner);
        Assert.Same(inner, anidada.InnerException);
    }
}
