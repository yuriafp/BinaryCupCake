using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BinaryCupcake.SharedLibrary.Models
{
    public class Produto
    {
        public int Id { get; set; }
        [Required]
        public string? Nome { get; set; }
        [Required]
        public string? Descricao { get; set; }
        [Required, Range(0.1, 999999.99)]
        public decimal Preco { get; set; }
        [Required, DisplayName("Imagem do Produto")]
        public string? Base64Img { get; set; }
        [Required, Range(1, 99999)]
        public int Quantidade { get; set; }
        public bool Destaque {  get; set; } = false;
        public DateTime Data { get; set; } = DateTime.Now.ToUniversalTime();

    }
}
