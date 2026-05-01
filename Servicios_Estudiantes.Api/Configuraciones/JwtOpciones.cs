namespace Servicios_Estudiantes.Api.Configuraciones
{
    public sealed class JwtOpciones
    {
        public const string NombreSeccion = "Jwt";

        public string Emisor { get; set; } = string.Empty;

        public string Audiencia { get; set; } = string.Empty;

        /// <summary>Clave simétrica (mínimo 32 caracteres para HS256).</summary>
        public string ClaveSecreta { get; set; } = string.Empty;

        public int MinutosValidezAcceso { get; set; } = 60;

        public int DiasValidezRefresh { get; set; } = 7;

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Emisor)) throw new InvalidOperationException("Jwt:Emisor es obligatorio.");
            if (string.IsNullOrWhiteSpace(Audiencia)) throw new InvalidOperationException("Jwt:Audiencia es obligatorio.");
            if (string.IsNullOrWhiteSpace(ClaveSecreta) || ClaveSecreta.Length < 32)
                throw new InvalidOperationException("Jwt:ClaveSecreta debe tener al menos 32 caracteres.");
        }
    }
}
