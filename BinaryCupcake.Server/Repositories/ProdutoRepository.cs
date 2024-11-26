using BinaryCupcake.Client.Services;
using BinaryCupcake.Server.Data;
using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace BinaryCupcake.Server.Repositories
{
    public class ProdutoRepository : IProduto
    {
        private readonly AppDbContext appDbContext;

        public ProdutoRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;

        }
        public async Task<ServiceResponse> AddProduto(Produto produto)
        {
            if (produto is null) return new ServiceResponse(false, "O produto não pode ser nulo!");
            var (flag, mensagem) = await VerificaNome(produto.Nome);
            if (flag)
            {
                appDbContext.Produtos.Add(produto);
                await Commit();
                return new ServiceResponse(true, "Salvo com sucesso!");
            }
            return new ServiceResponse(flag, mensagem);
        }
        private async Task<ServiceResponse> VerificaNome(string nome)
        {
            var produto = await appDbContext.Produtos.FirstOrDefaultAsync(x => x.Nome.ToLower()!.Equals(nome.ToLower()));
            return produto is null ? new ServiceResponse(true, null!) : new ServiceResponse(false, "Esse produto já existe.");
        }

        private async Task Commit() => await appDbContext.SaveChangesAsync();

        public async Task<List<Produto>> ListaTodosProdutosPorDestaque(bool produtoDestacado)
        {
            if (produtoDestacado)
            {
                return await appDbContext.Produtos.Where(x => x.Destaque.Equals(produtoDestacado)).ToListAsync();
            }
            else
            {
                return await appDbContext.Produtos.Where(x => x.Destaque.Equals(produtoDestacado)).ToListAsync();
            }
        }

        public async Task<List<Produto>> ListaTodosProdutos()
        {
            var produtos = await appDbContext.Produtos.ToListAsync();
            return produtos;
        }
    }
}
