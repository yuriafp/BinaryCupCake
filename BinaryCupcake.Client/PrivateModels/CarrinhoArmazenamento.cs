using System.Text.Json.Serialization;

namespace BinaryCupcake.Client.Models
{
    public class CarrinhoArmazenamento
    {
        [JsonPropertyName("produtoId")]
        public int ProdutoId { get; set; }
        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }
    }
}
