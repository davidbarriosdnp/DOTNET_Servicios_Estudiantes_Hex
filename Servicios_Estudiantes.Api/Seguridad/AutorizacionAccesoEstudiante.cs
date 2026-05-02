using System.Security.Claims;
using Servicios_Estudiantes.Aplicacion.Constantes;

namespace Servicios_Estudiantes.Api.Seguridad
{
    /// <summary>Comprueba si el usuario puede actuar sobre el perfil académico indicado (admin o propio estudiante).</summary>
    public static class AutorizacionAccesoEstudiante
    {
        public static bool PuedeGestionarPerfil(ClaimsPrincipal usuario, int estudianteId)
        {
            if (usuario.IsInRole("Administrador")) return true;
            string? raw = usuario.FindFirst(JwtReclamaciones.EstudianteId)?.Value;
            return int.TryParse(raw, out int id) && id == estudianteId;
        }
    }
}
