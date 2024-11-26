using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;
using System.Text.Json.Serialization;
using System.Text.Json;
using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.Client.Autenticacao;
using BinaryCupcake.Client.PrivateModels;
using Blazored.LocalStorage;
using BinaryCupcake.Client.Models;
using System.Reflection.Metadata;
using System.Text;

namespace BinaryCupcake.Client.Services
{
    public class ClientServices(HttpClient httpClient, AutenticacaoService autenticacaoService, ILocalStorageService localStorageService) : IProdutoService, IUsuarioService, ICarrinho
    {
        private const string BaseUrl = "api/produto";
        private const string AutenticacaoBaseUrl = "api/usuario";

        public Action? CarrinhoAction { get; set; }
        public int CarrinhoContador { get; set; }
        
        #region Json
        private static string SerializeObj(object modelObject) => JsonSerializer.Serialize(modelObject, JsonOptions());
        private static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
        private static StringContent GenerateStringContent(string serializeObj) => new(serializeObj, System.Text.Encoding.UTF8, "application/json");
        private static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;
        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }
        #endregion Json
        #region Produto
        public async Task<ServiceResponse> AddProduto(Produto produto)
        {
            var usuario = await autenticacaoService.GetDetalhesUsuario();
            var privateHttpClient = await autenticacaoService.addHeaderHttpClient();

            if (usuario.Permissao!.Equals("Admin"))
            {

                var response = await privateHttpClient.PostAsync($"{BaseUrl}/add-produto", GenerateStringContent(SerializeObj(produto)));

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse(false, "Um erro ocorreu. Tente novamente mais tarde.");
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                return DeserializeJsonString<ServiceResponse>(apiResponse);
            }
            return new ServiceResponse(false, "Usuário sem permissão para essa ação!");
        }

        public async Task<List<Produto>> ListaTodosProdutosPorDestaque(bool produtoDestacado)
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/produtos-destaque?destacado={produtoDestacado}");

            if (!response.IsSuccessStatusCode) return null;

            var resultado = await response.Content.ReadAsStringAsync();
            return [.. DeserializeJsonStringList<Produto>(resultado)];
        }

        public async Task<List<Produto>> ListaTodosProdutos()
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/todos-produtos");
            if (!response.IsSuccessStatusCode) return null;

            var resultado = await response.Content.ReadAsStringAsync();
            return [.. DeserializeJsonStringList<Produto>(resultado)];
        }
        #endregion Produto
        #region Autenticacao
        public async Task<ServiceResponse> Registrar(UsuarioDTO usuario)
        {
            var response = await httpClient.PostAsync($"{AutenticacaoBaseUrl}/registrar", GenerateStringContent(SerializeObj(usuario)));

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadAsStringAsync();
            return DeserializeJsonString<ServiceResponse>(apiResponse);
        }
        public async Task<LoginResponse> Login(LoginDTO usuario)
        {
            var response = await httpClient.PostAsync($"{AutenticacaoBaseUrl}/login", GenerateStringContent(SerializeObj(usuario)));

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadAsStringAsync();
            return DeserializeJsonString<LoginResponse>(apiResponse);
        }

        public async Task<ServiceResponse> AtualizarUsuarioPorId(UsuarioDTO usuario)
        {
            var response = await httpClient.PutAsync($"{AutenticacaoBaseUrl}/atualizar", GenerateStringContent(SerializeObj(usuario)));
            
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadAsStringAsync();
            return DeserializeJsonString<ServiceResponse>(apiResponse);
        }
        #endregion Autenticacao
        #region Carrinho
        public async Task GetCarrinhoContador()
        {
              string carrinhoString = await GetCarrinhoLocalStorage();

                if (string.IsNullOrEmpty(carrinhoString))
                {
                    CarrinhoContador = 0;
                }
                else
                {
                    using var document = JsonDocument.Parse(carrinhoString);
                    CarrinhoContador = document.RootElement.GetArrayLength();
                }

            CarrinhoAction?.Invoke();
        }

        public async Task<ServiceResponse> AddCarrinho(Produto produto, int quantidade = 1)
        {
            quantidade = Math.Max(quantidade, 1);

            string mensagem;
            var meuCarrinho = new List<CarrinhoArmazenamento>();
            var buscaCarrinhoStorage = await GetCarrinhoLocalStorage();

            if (!string.IsNullOrEmpty(buscaCarrinhoStorage))
            {
                meuCarrinho = (List<CarrinhoArmazenamento>)JsonContentService.DeserializeJsonStringList<CarrinhoArmazenamento>(buscaCarrinhoStorage);

                var verificaSeAdicionado = meuCarrinho.FirstOrDefault(x => x.ProdutoId == produto.Id);
                if (verificaSeAdicionado is null)
                {
                    meuCarrinho.Add(new CarrinhoArmazenamento()
                    {
                        ProdutoId = produto.Id,
                        Quantidade = quantidade
                    });
                    mensagem = "Produto adicionado ao carrinho!";
                }
                else
                {
                    verificaSeAdicionado.Quantidade = quantidade;
                    mensagem = "Quantidade do produto atualizada no carrinho!";
                }
            }
            else
            {
                meuCarrinho.Add(new CarrinhoArmazenamento()
                {
                    ProdutoId = produto.Id,
                    Quantidade = quantidade
                });
                mensagem = "Produto adicionado ao carrinho!";
            }

            await RemoveCarrinhoLocalStorage();
            await SetCarrinhoLocalStorage(JsonContentService.SerializeObj(meuCarrinho));
            await GetCarrinhoContador();

            return new ServiceResponse(true, mensagem);
        }



        public async Task<List<Pedido>> MeusPedidos()
        {
            var carrinhoList = new List<Pedido>();
            string meuCarrinhoString = await GetCarrinhoLocalStorage();
            if (string.IsNullOrEmpty(meuCarrinhoString)) return null!;


            var meuCarrinhoList = JsonContentService.DeserializeJsonStringList<CarrinhoArmazenamento>(meuCarrinhoString);

            var todosProdutos = await ListaTodosProdutos();

            foreach (var itemCarrinho in meuCarrinhoList)
            {
                var produto = todosProdutos.FirstOrDefault(x => x.Id == itemCarrinho.ProdutoId);

                carrinhoList.Add(new Pedido()
                {
                    Id = produto!.Id,
                    Nome = produto.Nome,
                    Quantidade = itemCarrinho.Quantidade,
                    Preco = produto.Preco,
                    Imagem = produto.Base64Img
                });

            }
            await GetCarrinhoContador();
            return carrinhoList;
        }

        public async Task<ServiceResponse> RemoverCarrinho(Pedido carrinho)
        {
            var meuCarrinhoList = JsonContentService.DeserializeJsonStringList<CarrinhoArmazenamento>(await GetCarrinhoLocalStorage());
            if (meuCarrinhoList is null) return new ServiceResponse(false, "Produto não encontrado!");

            meuCarrinhoList.Remove(meuCarrinhoList.FirstOrDefault(x => x.ProdutoId == carrinho.Id)!);

            await RemoveCarrinhoLocalStorage();
            await SetCarrinhoLocalStorage(JsonContentService.SerializeObj(meuCarrinhoList));
            await GetCarrinhoContador();

            return new ServiceResponse(true, "Produto removido com sucesso");
        }
        public async Task<string> Pagamento(List<Pedido> carrinho)
        {
            var response = await httpClient.PostAsync("api/pagamento/pagamento", JsonContentService.GenerateStringContent(JsonContentService.SerializeObj(carrinho)));

            var resultado = await response.Content.ReadAsStringAsync();

            return resultado;
        }
        private async Task<string> GetCarrinhoLocalStorage() => await localStorageService.GetItemAsStringAsync("carrinho");
        private async Task SetCarrinhoLocalStorage(string carrinho) => await localStorageService.SetItemAsStringAsync("carrinho", carrinho);
        private async Task RemoveCarrinhoLocalStorage() => await localStorageService.RemoveItemAsync("carrinho");

      




        #endregion Carrinho
    }
}
