namespace BinaryCupcake.Client.PrivateModels
{
    public class Pedido
    {
        public int Id { get; set; } 
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string? Imagem { get; set; }
        public decimal TaxaEntrega = 10;
        public decimal Total => Quantidade * Preco + TaxaEntrega;
    }
}
