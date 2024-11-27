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
        [HttpGet("produtos-destaque")]
        public async Task<ActionResult<List<Produto>>> ListaTodosProdutosPorDestaque(bool destacado)
        {
            var produto = await produtoService.ListaTodosProdutosPorDestaque(destacado); return Ok(produto);
        }
        [HttpPost("add-produto")]
        public async Task<ActionResult<ServiceResponse>> AddProduto(Produto produto)
        {
            if (produto is null) return BadRequest("O produto não pode ser nulo.");
            var response = await produtoService.AddProduto(produto);
            return Ok(response);
        }

        [HttpGet("todos-produtos")]
        public async Task<ActionResult<List<Produto>>> ListaTodosProdutos()
        {
            var produto = await produtoService.ListaTodosProdutos();
            return Ok(produto);
        }

        [HttpDelete("remover-produto")]
        public async Task<ActionResult> RemoverProduto(int produtoId)
        {
            if (produtoId == 0) return BadRequest("O produto não pode ser nulo.");

            var response = await produtoService.RemoverProduto(produtoId);

            return Ok(response);
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult> ObterProdutoPorId(int produtoId)
        {
            if (produtoId == 0) return BadRequest("O produto não pode ser nulo.");

            var response = await produtoService.ObterProdutoPorId(produtoId);

            return Ok(response);
        }
    }
}
