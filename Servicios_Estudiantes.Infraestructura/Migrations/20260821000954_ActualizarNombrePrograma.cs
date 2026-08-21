using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Servicios_Estudiantes.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarNombrePrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aula",
                columns: table => new
                {
                    AulaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aula", x => x.AulaId);
                });

            migrationBuilder.CreateTable(
                name: "Estudiante",
                columns: table => new
                {
                    EstudianteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    ProgramaCreditoId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiante", x => x.EstudianteId);
                });

            migrationBuilder.CreateTable(
                name: "InscripcionEstudianteMateria",
                columns: table => new
                {
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionEstudianteMateria", x => new { x.EstudianteId, x.MateriaId });
                });

            migrationBuilder.CreateTable(
                name: "Materia",
                columns: table => new
                {
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creditos = table.Column<byte>(type: "tinyint", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    ProgramaCreditoId = table.Column<int>(type: "int", nullable: false),
                    AulaId = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: true),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materia", x => x.MateriaId);
                });

            migrationBuilder.CreateTable(
                name: "Profesor",
                columns: table => new
                {
                    ProfesorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesor", x => x.ProfesorId);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaCredito",
                columns: table => new
                {
                    ProgramaCreditoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditosPorMateria = table.Column<byte>(type: "tinyint", nullable: false),
                    MaxMateriasPorEstudiante = table.Column<byte>(type: "tinyint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaCredito", x => x.ProgramaCreditoId);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    SedeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.SedeId);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                });

            migrationBuilder.InsertData(
                table: "Aula",
                columns: new[] { "AulaId", "Capacidad", "Estado", "FechaModificacion", "FechaRegistro", "Nombre", "SedeId" },
                values: new object[,]
                {
                    { 1, 30, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "A-101", 1 },
                    { 2, 40, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "A-102", 1 },
                    { 3, 25, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "B-201", 2 }
                });

            migrationBuilder.InsertData(
                table: "Materia",
                columns: new[] { "MateriaId", "AulaId", "Creditos", "Estado", "FechaFin", "FechaInicio", "FechaModificacion", "FechaRegistro", "HoraFin", "HoraInicio", "Nombre", "ProfesorId", "ProgramaCreditoId" },
                values: new object[,]
                {
                    { 1, 1, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), "Álgebra Lineal", 1, 1 },
                    { 2, 2, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0), "Cálculo I", 1, 1 },
                    { 3, 1, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0), "Programación I", 2, 1 },
                    { 4, 3, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), "Estructuras de Datos", 2, 1 },
                    { 5, 2, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0), "Bases de Datos", 3, 1 },
                    { 6, 3, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0), "Sistemas Operativos", 3, 1 },
                    { 7, 1, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(18, 0, 0), new TimeOnly(16, 0, 0), "Redes", 4, 1 },
                    { 8, 2, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(20, 0, 0), new TimeOnly(18, 0, 0), "Seguridad Informática", 4, 1 },
                    { 9, 3, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(18, 0, 0), new TimeOnly(16, 0, 0), "Ingeniería de Software", 5, 1 },
                    { 10, 1, (byte)3, (byte)1, new DateOnly(2026, 12, 15), new DateOnly(2026, 8, 1), null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), "Gestión de Proyectos", 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Profesor",
                columns: new[] { "ProfesorId", "Estado", "FechaModificacion", "FechaRegistro", "Nombre" },
                values: new object[,]
                {
                    { 1, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Prof. Ana García" },
                    { 2, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Prof. Luis Martínez" },
                    { 3, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Prof. Carmen Ruiz" },
                    { 4, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Prof. Jorge Soto" },
                    { 5, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Prof. Elena Vargas" }
                });

            migrationBuilder.InsertData(
                table: "ProgramaCredito",
                columns: new[] { "ProgramaCreditoId", "CreditosPorMateria", "Estado", "FechaModificacion", "FechaRegistro", "MaxMateriasPorEstudiante", "Nombre" },
                values: new object[] { 1, (byte)3, (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), (byte)3, "Ingeniería de software" });

            migrationBuilder.InsertData(
                table: "Sede",
                columns: new[] { "SedeId", "Direccion", "Estado", "FechaModificacion", "FechaRegistro", "Nombre" },
                values: new object[,]
                {
                    { 1, "Calle 123", (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Sede Principal" },
                    { 2, "Avenida 45", (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "Sede Norte" }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "UsuarioId", "Email", "Estado", "FechaModificacion", "FechaRegistro", "NombreUsuario", "PasswordHash", "Rol" },
                values: new object[] { 1, "admin@local.test", (byte)1, null, new DateTime(2026, 8, 21, 0, 9, 54, 564, DateTimeKind.Utc).AddTicks(126), "admin", "AQAAAAIAAYagAAAAEBUYADfSZ2TyLmEJjXSQVXyehyd/8I0XdpR0kBnq65pRMiA1G9a+PKzL1uvr6fbcyA==", "Administrador" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aula");

            migrationBuilder.DropTable(
                name: "Estudiante");

            migrationBuilder.DropTable(
                name: "InscripcionEstudianteMateria");

            migrationBuilder.DropTable(
                name: "Materia");

            migrationBuilder.DropTable(
                name: "Profesor");

            migrationBuilder.DropTable(
                name: "ProgramaCredito");

            migrationBuilder.DropTable(
                name: "Sede");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
