using System;
using System.Threading;
using System.Threading.Tasks;
using Servicios_Estudiantes.Aplicacion.DTOs;

namespace Servicios_Estudiantes.Aplicacion.Puertos
{
    public interface IRepositorioTokens
    {
        Task InsertarRefreshTokenAsync(int usuarioId, string tokenHash, DateTime expiresUtc, CancellationToken ct);
        Task<RefreshTokenValidoDto?> ObtenerRefreshValidoPorHashAsync(string tokenHash, CancellationToken ct);
        Task RevocarRefreshPorHashAsync(string tokenHash, CancellationToken ct);
        Task RevocarTodosRefreshUsuarioAsync(int usuarioId, CancellationToken ct);
    }
}
