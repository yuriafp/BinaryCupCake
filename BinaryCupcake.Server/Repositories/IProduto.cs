using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Server.Repositories
{
    public interface IProduto
    {
        Task<ServiceResponse> AddProduto(Produto produto);
        Task<List <Produto>> ListaTodosProdutos(bool produtoDestacado);
    }
}
