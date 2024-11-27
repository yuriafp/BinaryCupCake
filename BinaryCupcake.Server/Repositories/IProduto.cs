using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Server.Repositories
{
    public interface IProduto
    {
        Task<ServiceResponse> AddProduto(Produto produto);
        Task<List<Produto>> ListaTodosProdutosPorDestaque(bool produtoDestacado);
        Task<List<Produto>> ListaTodosProdutos();
        Task<ServiceResponse> RemoverProduto(int produtoId);
        Task<Produto> ObterProdutoPorId(int produtoId);
    }
}
