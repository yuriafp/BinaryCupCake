using BinaryCupcake.Client.PrivateModels;
using Stripe.Checkout;

namespace BinaryCupcake.Server.Repositories
{
    public interface IPagamento
    {
        string CriarSessaoDePagamento(List<Pedido> carrinho);
    }
}
