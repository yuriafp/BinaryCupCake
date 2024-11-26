using BinaryCupcake.Client.PrivateModels;
using Microsoft.Exchange.WebServices.Data;
using Stripe;
using Stripe.Checkout;

namespace BinaryCupcake.Server.Repositories
{
    public class PagamentoRepository : IPagamento
    {
        public PagamentoRepository()
        {
            StripeConfiguration.ApiKey = "sk_test_51QNhtfKoMsJrtjrLvKP9KdAoYMtFFNtVtJe9qsPW95jKuOSNDrVWcvenS0TUd8eWgfIXcex1GGUbTyLfX4Oa1EKf00JB5sjO1i";
        }
        public string CriarSessaoDePagamento(List<Pedido> carrinho)
        {
            if (carrinho is null) return null!;

            var itensDeLinha = new List<SessionLineItemOptions>();

            carrinho!.ForEach(item => itensDeLinha.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = (item.Preco * 100),
                    Currency = "brl",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Nome,
                        Description = item.Id.ToString()
                    }
                },
                Quantity = item.Quantidade
            }));

            var taxaDeEntregaItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = Pedido.TaxaEntrega * 100, 
                    Currency = "brl",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Taxa de entrega",
                        Description = "Entrega do pedido"
                    }
                },
                Quantity = 1
            };

            itensDeLinha.Add(taxaDeEntregaItem);


            var opcoes = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems = itensDeLinha,
                Mode = "payment",
                SuccessUrl = "https://localhost:7119/pagamento-sucedido",
                CancelUrl = "https://localhost:7119/",
            };

            var servico = new SessionService();
            Session sessao = servico.Create(opcoes);
            return sessao.Url;
        }
    }
}
