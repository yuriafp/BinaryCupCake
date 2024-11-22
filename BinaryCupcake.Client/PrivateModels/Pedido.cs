using BinaryCupcake.SharedLibrary.DTOs;

namespace BinaryCupcake.Client.PrivateModels
{
    public class Pedido
    {
        public int Id { get; set; } 
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string? Imagem { get; set; }
        public static decimal TaxaEntrega { get; set; } = 10m;
        public decimal Total => Quantidade * Preco;
        public static decimal CalcularTotalComTaxaEntrega(IEnumerable<Pedido> pedidos)
        {
            var total = pedidos.Sum(p => p.Total);
            return total + TaxaEntrega;
        }
    }
}
