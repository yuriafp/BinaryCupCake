using BinaryCupcake.Client.PrivateModels;
using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Client.Services
{
    public interface ICarrinho
    {
        public Action? CarrinhoAction { get; set; }
        public int CarrinhoContador { get; set; }
        Task GetCarrinhoContador();
        Task<ServiceResponse> AddCarrinho(Produto produto, int quantidade = 1);
        Task<List<Pedido>> MeusPedidos();
        Task<ServiceResponse> RemoverCarrinho(Pedido carrinho);
        bool CarrinhoVisivel {  get; set; }

    }
}
