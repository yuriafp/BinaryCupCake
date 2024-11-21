using BinaryCupcake.Client.PrivateModels;
using BinaryCupcake.Server.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BinaryCupcake.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentoController(IPagamento pagamentoServico) : ControllerBase
    {
        [HttpPost("pagamento")]

        public ActionResult CriarSessaoDePagamento(List<Pedido> carrinho)
        {
            var sessao = pagamentoServico.CriarSessaoDePagamento(carrinho);
            return Ok(sessao);
        }
    }
}
