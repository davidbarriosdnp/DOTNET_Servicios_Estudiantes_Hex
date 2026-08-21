using Microsoft.EntityFrameworkCore;
using Servicios_Estudiantes.Dominio.Entidades;
using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    public static class DbContextExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            var now = DateTime.UtcNow;

            modelBuilder.Entity<ProgramaCredito>().HasData(
                new ProgramaCredito
                {
                    ProgramaCreditoId = 1,
                    Nombre = "Ingeniería de software",
                    CreditosPorMateria = 3,
                    MaxMateriasPorEstudiante = 3,
                    FechaRegistro = now,
                    Estado = EstadoRegistro.Activo
                }
            );

            modelBuilder.Entity<Sede>().HasData(
                new Sede { SedeId = 1, Nombre = "Sede Principal", Direccion = "Calle 123", FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Sede { SedeId = 2, Nombre = "Sede Norte", Direccion = "Avenida 45", FechaRegistro = now, Estado = EstadoRegistro.Activo }
            );

            modelBuilder.Entity<Aula>().HasData(
                new Aula { AulaId = 1, Nombre = "A-101", Capacidad = 30, SedeId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Aula { AulaId = 2, Nombre = "A-102", Capacidad = 40, SedeId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Aula { AulaId = 3, Nombre = "B-201", Capacidad = 25, SedeId = 2, FechaRegistro = now, Estado = EstadoRegistro.Activo }
            );

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioId = 1,
                    NombreUsuario = "admin",
                    Email = "admin@local.test",
                    PasswordHash = "AQAAAAIAAYagAAAAEBUYADfSZ2TyLmEJjXSQVXyehyd/8I0XdpR0kBnq65pRMiA1G9a+PKzL1uvr6fbcyA==",
                    Rol = "Administrador",
                    FechaRegistro = now,
                    Estado = (byte)EstadoRegistro.Activo
                }
            );

            modelBuilder.Entity<Profesor>().HasData(
                new Profesor { ProfesorId = 1, Nombre = "Prof. Ana García", FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Profesor { ProfesorId = 2, Nombre = "Prof. Luis Martínez", FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Profesor { ProfesorId = 3, Nombre = "Prof. Carmen Ruiz", FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Profesor { ProfesorId = 4, Nombre = "Prof. Jorge Soto", FechaRegistro = now, Estado = EstadoRegistro.Activo },
                new Profesor { ProfesorId = 5, Nombre = "Prof. Elena Vargas", FechaRegistro = now, Estado = EstadoRegistro.Activo }
            );

            var fecIn = new DateOnly(2026, 8, 1);
            var fecFi = new DateOnly(2026, 12, 15);
            var hr8 = new TimeOnly(8, 0);
            var hr10 = new TimeOnly(10, 0);
            var hr14 = new TimeOnly(14, 0);
            var hr16 = new TimeOnly(16, 0);

            modelBuilder.Entity<Materia>().HasData(
                new Materia { MateriaId = 1, Nombre = "Álgebra Lineal", Creditos = 3, ProfesorId = 1, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 1, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr8, HoraFin = hr10 },
                new Materia { MateriaId = 2, Nombre = "Cálculo I", Creditos = 3, ProfesorId = 1, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 2, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr10, HoraFin = new TimeOnly(12, 0) },
                new Materia { MateriaId = 3, Nombre = "Programación I", Creditos = 3, ProfesorId = 2, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 1, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr14, HoraFin = hr16 },
                new Materia { MateriaId = 4, Nombre = "Estructuras de Datos", Creditos = 3, ProfesorId = 2, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 3, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr8, HoraFin = hr10 },
                new Materia { MateriaId = 5, Nombre = "Bases de Datos", Creditos = 3, ProfesorId = 3, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 2, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr14, HoraFin = hr16 },
                new Materia { MateriaId = 6, Nombre = "Sistemas Operativos", Creditos = 3, ProfesorId = 3, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 3, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr10, HoraFin = new TimeOnly(12, 0) },
                new Materia { MateriaId = 7, Nombre = "Redes", Creditos = 3, ProfesorId = 4, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 1, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = new TimeOnly(16, 0), HoraFin = new TimeOnly(18, 0) },
                new Materia { MateriaId = 8, Nombre = "Seguridad Informática", Creditos = 3, ProfesorId = 4, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 2, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = new TimeOnly(18, 0), HoraFin = new TimeOnly(20, 0) },
                new Materia { MateriaId = 9, Nombre = "Ingeniería de Software", Creditos = 3, ProfesorId = 5, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 3, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = new TimeOnly(16, 0), HoraFin = new TimeOnly(18, 0) },
                new Materia { MateriaId = 10, Nombre = "Gestión de Proyectos", Creditos = 3, ProfesorId = 5, ProgramaCreditoId = 1, FechaRegistro = now, Estado = EstadoRegistro.Activo, AulaId = 1, FechaInicio = fecIn, FechaFin = fecFi, HoraInicio = hr8, HoraFin = hr10 }
            );
        }
    }
}
