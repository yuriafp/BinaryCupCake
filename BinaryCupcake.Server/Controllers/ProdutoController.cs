using BinaryCupcake.Server.Repositories;
using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BinaryCupcake.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProduto produtoService;
        public ProdutoController(IProduto produtoService)
        {
            this.produtoService = produtoService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Produto>>> ListaTodosProdutos(bool destacado)
        {
            var produto = await produtoService.ListaTodosProdutos(destacado); return Ok(produto);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddProduto(Produto produto)
        {
            if (produto is null) return  BadRequest("O produto não pode ser nulo.");
            var response = await produtoService.AddProduto(produto);
            return Ok(response);
        }
    }
}
