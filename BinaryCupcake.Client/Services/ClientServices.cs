using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;
using System.Text.Json.Serialization;
using System.Text.Json;
using BinaryCupcake.SharedLibrary.DTOs;

namespace BinaryCupcake.Client.Services
{
    public class ClientServices(HttpClient httpClient) : IProdutoService, IUsuarioService
    {
        private const string BaseUrl = "api/produto";
        private const string AutenticacaoBaseUrl = "api/usuario";
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

        public async Task<ServiceResponse> AddProduto(Produto produto)
        {
            var response = await httpClient.PostAsync(BaseUrl, GenerateStringContent(SerializeObj(produto)));

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false, "Um erro ocorreu. Tente novamente mais tarde.");
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            return DeserializeJsonString<ServiceResponse>(apiResponse);
        }

        public async Task<List<Produto>> ListaTodosProdutos(bool produtoDestacado)
        {
            var response = await httpClient.GetAsync($"{BaseUrl}?destacado={produtoDestacado}");
            if (!response.IsSuccessStatusCode) return null;

            var resultado = await response.Content.ReadAsStringAsync();
            return [.. DeserializeJsonStringList<Produto>(resultado)];


        }
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
    }
}
