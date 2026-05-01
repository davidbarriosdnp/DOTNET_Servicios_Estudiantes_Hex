using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_Estudiantes.Infraestructura.Constantes
{
    /// <summary>
    /// Define los nombres de los procedimientos almacenados utilizados en la persistencia.
    /// </summary>
    public static class ProcedimientosAlmacenados
    {
        public const string ProgramaCredito_Insertar = "dbo.sp_ProgramaCredito_Insertar";
        public const string ProgramaCredito_Actualizar = "dbo.sp_ProgramaCredito_Actualizar";
        public const string ProgramaCredito_Eliminar = "dbo.sp_ProgramaCredito_Eliminar";
        public const string ProgramaCredito_ObtenerPorId = "dbo.sp_ProgramaCredito_ObtenerPorId";
        public const string ProgramaCredito_Listar = "dbo.sp_ProgramaCredito_Listar";

        public const string Profesor_Insertar = "dbo.sp_Profesor_Insertar";
        public const string Profesor_Actualizar = "dbo.sp_Profesor_Actualizar";
        public const string Profesor_Eliminar = "dbo.sp_Profesor_Eliminar";
        public const string Profesor_ObtenerPorId = "dbo.sp_Profesor_ObtenerPorId";
        public const string Profesor_Listar = "dbo.sp_Profesor_Listar";

        public const string Materia_Insertar = "dbo.sp_Materia_Insertar";
        public const string Materia_Actualizar = "dbo.sp_Materia_Actualizar";
        public const string Materia_Eliminar = "dbo.sp_Materia_Eliminar";
        public const string Materia_ObtenerPorId = "dbo.sp_Materia_ObtenerPorId";
        public const string Materia_ListarPorPrograma = "dbo.sp_Materia_ListarPorPrograma";

        public const string Estudiante_Insertar = "dbo.sp_Estudiante_Insertar";
        public const string Estudiante_Actualizar = "dbo.sp_Estudiante_Actualizar";
        public const string Estudiante_Eliminar = "dbo.sp_Estudiante_Eliminar";
        public const string Estudiante_ObtenerPorId = "dbo.sp_Estudiante_ObtenerPorId";
        public const string Estudiante_ListarRegistros = "dbo.sp_Estudiante_ListarRegistros";

        public const string Inscripcion_Registrar = "dbo.sp_Inscripcion_Registrar";
        public const string Inscripcion_InsertarFila = "dbo.sp_Inscripcion_InsertarFila";
        public const string Inscripcion_EliminarFila = "dbo.sp_Inscripcion_EliminarFila";
        public const string Inscripcion_ActualizarMateria = "dbo.sp_Inscripcion_ActualizarMateria";
        public const string Inscripcion_ObtenerPorEstudiante = "dbo.sp_Inscripcion_ObtenerPorEstudiante";

        public const string Companeros_ListarNombresPorMateria = "dbo.sp_Companeros_ListarNombresPorMateria";

        public const string Usuario_Listar = "dbo.sp_Usuario_Listar";
        public const string Usuario_ObtenerPorId = "dbo.sp_Usuario_ObtenerPorId";
        public const string Usuario_ObtenerPorNombreUsuario = "dbo.sp_Usuario_ObtenerPorNombreUsuario";
        public const string Usuario_ObtenerPorEmail = "dbo.sp_Usuario_ObtenerPorEmail";
        public const string Usuario_Insertar = "dbo.sp_Usuario_Insertar";
        public const string Usuario_Actualizar = "dbo.sp_Usuario_Actualizar";
        public const string Usuario_ActualizarPassword = "dbo.sp_Usuario_ActualizarPassword";
        public const string Usuario_Eliminar = "dbo.sp_Usuario_Eliminar";
        public const string RefreshToken_Insertar = "dbo.sp_RefreshToken_Insertar";
        public const string RefreshToken_ObtenerPorHash = "dbo.sp_RefreshToken_ObtenerPorHash";
        public const string RefreshToken_RevocarPorHash = "dbo.sp_RefreshToken_RevocarPorHash";
        public const string RefreshToken_RevocarTodosUsuario = "dbo.sp_RefreshToken_RevocarTodosUsuario";
    }
}
