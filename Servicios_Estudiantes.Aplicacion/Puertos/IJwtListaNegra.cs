namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IJwtListaNegra
    {
        Task RegistrarRevocacionAsync(string jti, DateTime expiraUtc, CancellationToken ct);

        Task<bool> EstaRevocadoAsync(string jti, CancellationToken ct);
    }
}
