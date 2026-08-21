using Microsoft.EntityFrameworkCore;
using Servicios_Estudiantes.Dominio.Entidades;
using Servicios_Estudiantes.Dominio.Enumeraciones;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    public class EstudiantesDbContext : DbContext
    {
        public EstudiantesDbContext(DbContextOptions<EstudiantesDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Estudiante> Estudiantes { get; set; } = null!;
        public DbSet<Profesor> Profesores { get; set; } = null!;
        public DbSet<Materia> Materias { get; set; } = null!;
        public DbSet<ProgramaCredito> ProgramasCredito { get; set; } = null!;
        public DbSet<InscripcionEstudianteMateria> Inscripciones { get; set; } = null!;
        public DbSet<Sede> Sedes { get; set; } = null!;
        public DbSet<Aula> Aulas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuracion de Tablas (Nombres en singular para coincidir con SQL)
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Estudiante>().ToTable("Estudiante");
            modelBuilder.Entity<Profesor>().ToTable("Profesor");
            modelBuilder.Entity<Materia>().ToTable("Materia");
            modelBuilder.Entity<ProgramaCredito>().ToTable("ProgramaCredito");
            modelBuilder.Entity<InscripcionEstudianteMateria>().ToTable("InscripcionEstudianteMateria");
            modelBuilder.Entity<Sede>().ToTable("Sede");
            modelBuilder.Entity<Aula>().ToTable("Aula");

            // Llaves Primarias (La mayoria se infieren, pero las explicitas son mejores)
            modelBuilder.Entity<Usuario>().HasKey(u => u.UsuarioId);
            modelBuilder.Entity<Estudiante>().HasKey(e => e.EstudianteId);
            modelBuilder.Entity<Profesor>().HasKey(p => p.ProfesorId);
            modelBuilder.Entity<Materia>().HasKey(m => m.MateriaId);
            modelBuilder.Entity<ProgramaCredito>().HasKey(p => p.ProgramaCreditoId);
            modelBuilder.Entity<InscripcionEstudianteMateria>().HasKey(i => new { i.EstudianteId, i.MateriaId });
            modelBuilder.Entity<Sede>().HasKey(s => s.SedeId);
            modelBuilder.Entity<Aula>().HasKey(a => a.AulaId);

            // Configuracion de conversion de enums si fuera necesario
            // Por defecto, EF Core mapea enums a enteros, lo cual coincide con tu base de datos (tinyint)

            // Relaciones de base de datos para el MER físico
            
            // Usuario (1) <-> (0..1) Estudiante
            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Usuario)
                .WithOne(u => u.Estudiante)
                .HasForeignKey<Estudiante>(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProgramaCredito (1) <-> (N) Estudiante
            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.ProgramaCredito)
                .WithMany(p => p.Estudiantes)
                .HasForeignKey(e => e.ProgramaCreditoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sede (1) <-> (N) Aula
            modelBuilder.Entity<Aula>()
                .HasOne(a => a.Sede)
                .WithMany(s => s.Aulas)
                .HasForeignKey(a => a.SedeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Profesor (1) <-> (N) Materia
            modelBuilder.Entity<Materia>()
                .HasOne(m => m.Profesor)
                .WithMany(p => p.Materias)
                .HasForeignKey(m => m.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProgramaCredito (1) <-> (N) Materia
            modelBuilder.Entity<Materia>()
                .HasOne(m => m.ProgramaCredito)
                .WithMany(p => p.Materias)
                .HasForeignKey(m => m.ProgramaCreditoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aula (1) <-> (N) Materia (Opcional)
            modelBuilder.Entity<Materia>()
                .HasOne(m => m.Aula)
                .WithMany(a => a.Materias)
                .HasForeignKey(m => m.AulaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación de muchos a muchos (InscripcionEstudianteMateria)
            modelBuilder.Entity<InscripcionEstudianteMateria>()
                .HasOne(i => i.Estudiante)
                .WithMany(e => e.Inscripciones)
                .HasForeignKey(i => i.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InscripcionEstudianteMateria>()
                .HasOne(i => i.Materia)
                .WithMany(m => m.Inscripciones)
                .HasForeignKey(i => i.MateriaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Datos Semilla (HasData) para inicializacion
            modelBuilder.SeedData();
        }
    }
}
