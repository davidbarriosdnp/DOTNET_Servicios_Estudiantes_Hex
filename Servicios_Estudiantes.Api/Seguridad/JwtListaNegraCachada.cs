using Microsoft.Extensions.Caching.Memory;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Api.Seguridad
{
    public sealed class JwtListaNegraCachada(IMemoryCache cachada) : IJwtListaNegra
    {
        private const string Prefijo = "jwt_revoked_";
        private readonly IMemoryCache _cachada = cachada;

        public Task RegistrarRevocacionAsync(string jti, DateTime expiraUtc, CancellationToken ct)
        {
            TimeSpan vigencia = expiraUtc - DateTime.UtcNow;
            if (vigencia <= TimeSpan.Zero) return Task.CompletedTask;

            _cachada.Set(Prefijo + jti, true, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = vigencia });
            return Task.CompletedTask;
        }

        public Task<bool> EstaRevocadoAsync(string jti, CancellationToken ct) =>
            Task.FromResult(_cachada.TryGetValue(Prefijo + jti, out _));
    }
}
