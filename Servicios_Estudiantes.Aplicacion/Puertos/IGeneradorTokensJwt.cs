namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public sealed record ResultadoEmisionTokenAcceso(string Token, string Jti, DateTime ExpiraUtc);

    public interface IGeneradorTokensJwt
    {
        ResultadoEmisionTokenAcceso CrearTokenAcceso(int usuarioId, string nombreUsuario, string rol, int? estudianteId = null);

        string CrearTokenRenovacion();

        DateTime CalcularExpiracionRenovacionUtc();
    }
}
