namespace BinaryCupcake.Server.Data
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? TokenAcesso { get; set; }
        public string? TokenRenovacao { get; set; }
        public DateTime? DataExpiracao { get; set; } = DateTime.Now.AddDays(1).ToUniversalTime();
        public DateTime? DataCriacao { get; set; } = DateTime.Now.ToUniversalTime();

    }
}
