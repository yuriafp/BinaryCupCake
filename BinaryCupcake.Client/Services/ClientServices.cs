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

namespace BinaryCupcake.Client.Services
{
    public class ClientServices(HttpClient httpClient, AutenticacaoService autenticacaoService, ILocalStorageService localStorageService) : IProdutoService, IUsuarioService, ICarrinho
    {
        private const string BaseUrl = "api/produto";
        private const string AutenticacaoBaseUrl = "api/usuario";

        public Action? CarrinhoAction { get; set; }
        public int CarrinhoContador { get; set; }
        public bool CarrinhoVisivel { get; set; }

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

                var response = await privateHttpClient.PostAsync(BaseUrl, GenerateStringContent(SerializeObj(produto)));

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse(false, "Um erro ocorreu. Tente novamente mais tarde.");
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                return DeserializeJsonString<ServiceResponse>(apiResponse);
            }
            return new ServiceResponse(false, "Usuário sem permissão para essa ação!");
        }

        public async Task<List<Produto>> ListaTodosProdutos(bool produtoDestacado)
        {
            var response = await httpClient.GetAsync($"{BaseUrl}?destacado={produtoDestacado}");
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
        #endregion Autenticacao
        #region Carrinho
        public async Task GetCarrinhoContador()
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter contador do carrinho: {ex.Message}");
                CarrinhoContador = 0;
            }

            CarrinhoAction?.Invoke();
        }

        public async Task<ServiceResponse> AddCarrinho(Produto produto, int quantidade = 1)
        {
            // Garantir que a quantidade seja pelo menos 1
            quantidade = Math.Max(quantidade, 1);

            string mensagem;
            var meuCarrinho = new List<CarrinhoArmazenamento>();
            var buscaCarrinhoStorage = await GetCarrinhoLocalStorage();

            // Verifica se já existe um carrinho no localStorage
            if (!string.IsNullOrEmpty(buscaCarrinhoStorage))
            {
                meuCarrinho = (List<CarrinhoArmazenamento>)JsonContentService.DeserializeJsonStringList<CarrinhoArmazenamento>(buscaCarrinhoStorage);

                // Verifica se o produto já foi adicionado ao carrinho
                var verificaSeAdicionado = meuCarrinho.FirstOrDefault(x => x.ProdutoId == produto.Id);
                if (verificaSeAdicionado is null)
                {
                    // Produto não encontrado no carrinho, adiciona novo item
                    meuCarrinho.Add(new CarrinhoArmazenamento()
                    {
                        ProdutoId = produto.Id,
                        Quantidade = quantidade
                    });
                    mensagem = "Produto adicionado ao carrinho!";
                }
                else
                {
                    // Produto encontrado, substitui a quantidade
                    verificaSeAdicionado.Quantidade = quantidade; // Substituindo a quantidade
                    mensagem = "Quantidade do produto atualizada no carrinho!";
                }
            }
            else
            {
                // Carrinho vazio, adiciona o primeiro produto
                meuCarrinho.Add(new CarrinhoArmazenamento()
                {
                    ProdutoId = produto.Id,
                    Quantidade = quantidade
                });
                mensagem = "Produto adicionado ao carrinho!";
            }

            // Atualiza o localStorage com o carrinho atualizado
            await RemoveCarrinhoLocalStorage();
            await SetCarrinhoLocalStorage(JsonContentService.SerializeObj(meuCarrinho));
            await GetCarrinhoContador();

            return new ServiceResponse(true, mensagem);
        }



        public async Task<List<Pedido>> MeusPedidos()
        {
            CarrinhoVisivel = true;
            var carrinhoList = new List<Pedido>();
            string meuCarrinhoString = await GetCarrinhoLocalStorage();
            if (string.IsNullOrEmpty(meuCarrinhoString)) return null!;


            var meuCarrinhoList = JsonContentService.DeserializeJsonStringList<CarrinhoArmazenamento>(meuCarrinhoString);

            var produtosDestacadosTask = ListaTodosProdutos(true);
            var produtosNaoDestacadosTask = ListaTodosProdutos(false);
            await Task.WhenAll(produtosDestacadosTask, produtosNaoDestacadosTask);

            // Combina os produtos em uma única lista
            var todosProdutos = (await produtosDestacadosTask).Concat(await produtosNaoDestacadosTask).ToList();

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
            CarrinhoVisivel = false;
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

        public async Task<string> GetCarrinhoLocalStorage() => await localStorageService.GetItemAsStringAsync("carrinho");
        public async Task SetCarrinhoLocalStorage(string carrinho) => await localStorageService.SetItemAsStringAsync("carrinho", carrinho);
        public async Task RemoveCarrinhoLocalStorage() => await localStorageService.RemoveItemAsync("carrinho");

        #endregion Carrinho
    }
}
