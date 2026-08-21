using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Infraestructura.AccesoDatos
{
    [DynamoDBTable("RefreshTokens")]
    public class RefreshTokenDynamo
    {
        [DynamoDBHashKey]
        public string TokenHash { get; set; } = string.Empty;

        [DynamoDBProperty]
        public int UsuarioId { get; set; }

        [DynamoDBProperty]
        public long ExpiraUnix { get; set; } // TTL Field

        [DynamoDBProperty]
        public bool Revocado { get; set; }
    }

    public sealed class RepositorioTokensDynamoDB : IRepositorioTokens
    {
        private readonly DynamoDBContext _context;

        public RepositorioTokensDynamoDB(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
        }

        public async Task InsertarRefreshTokenAsync(int usuarioId, string tokenHash, DateTime expiresUtc, CancellationToken ct)
        {
            var item = new RefreshTokenDynamo
            {
                TokenHash = tokenHash,
                UsuarioId = usuarioId,
                ExpiraUnix = new DateTimeOffset(expiresUtc).ToUnixTimeSeconds(),
                Revocado = false
            };

            await _context.SaveAsync(item, ct);
        }

        public async Task<RefreshTokenValidoDto?> ObtenerRefreshValidoPorHashAsync(string tokenHash, CancellationToken ct)
        {
            var item = await _context.LoadAsync<RefreshTokenDynamo>(tokenHash, ct);
            if (item == null || item.Revocado) return null;

            var expiracion = DateTimeOffset.FromUnixTimeSeconds(item.ExpiraUnix).UtcDateTime;
            if (expiracion <= DateTime.UtcNow) return null;

            // En DynamoDB no tenemos un ID autonumerico para el token, asi que retornamos 0 o el hash.
            // Para mantener compatibilidad con RefreshTokenValidoDto usamos 0
            return new RefreshTokenValidoDto(0, item.UsuarioId, expiracion);
        }

        public async Task RevocarRefreshPorHashAsync(string tokenHash, CancellationToken ct)
        {
            var item = await _context.LoadAsync<RefreshTokenDynamo>(tokenHash, ct);
            if (item != null)
            {
                item.Revocado = true;
                await _context.SaveAsync(item, ct);
            }
        }

        public async Task RevocarTodosRefreshUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            // DynamoDB Scan is generally slow, but since this is just for a single user revocation,
            // the ideal way is a Global Secondary Index (GSI) on UsuarioId.
            // For simplicity in this free tier local setup, we use Scan.
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("UsuarioId", ScanOperator.Equal, usuarioId),
                new ScanCondition("Revocado", ScanOperator.Equal, false)
            };

            var items = await _context.ScanAsync<RefreshTokenDynamo>(conditions).GetRemainingAsync(ct);
            foreach (var item in items)
            {
                item.Revocado = true;
                await _context.SaveAsync(item, ct);
            }
        }
    }
}
