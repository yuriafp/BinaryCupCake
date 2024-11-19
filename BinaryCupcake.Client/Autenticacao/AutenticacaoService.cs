using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.SharedLibrary.Responses;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Claims;
using System.Text;


namespace BinaryCupcake.Client.Autenticacao
{
    public class AutenticacaoService(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        public async Task<SessaoUsuario> GetDetalhesUsuario()
        {
            var token = await GetTokenLocalStorage();
            if (string.IsNullOrEmpty(token)) return null!;

            var httpClient = await addHeaderHttpClient();
            var detalhesUsuario = Services.JsonContentService.DeserializeJsonString<TokenProp>(token);

            if (detalhesUsuario is null || string.IsNullOrEmpty(detalhesUsuario.Token)) return null!;

            var response = await GetDetalhesUsuarioApi();

            if (response.IsSuccessStatusCode) return await GetSessaoUsuario(response);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization") && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var tokenCodificado = Encoding.UTF8.GetBytes(detalhesUsuario.Token);
                var tokenValido = WebEncoders.Base64UrlEncode(tokenCodificado);
                var modelo = new TokenRenovacaoDTO()
                {
                    TokenRenovacao = tokenValido
                };

                var resultado = await httpClient.PostAsync("api/Usuario/renova-token", Services.JsonContentService.GenerateStringContent(Services.JsonContentService.SerializeObj(modelo)));

                if (resultado.IsSuccessStatusCode && resultado.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await resultado.Content.ReadAsStringAsync();
                    var loginResponse = Services.JsonContentService.DeserializeJsonString<LoginResponse>(apiResponse);

                    await SetTokenLocalStorage(Services.JsonContentService.SerializeObj(new TokenProp()
                    {
                        Token = loginResponse.Token,
                        TokenRenovacao = loginResponse.TokenRenovacao
                    }));

                    var detalhesUsuarioAtt = await GetDetalhesUsuarioApi();
                    if (detalhesUsuarioAtt.IsSuccessStatusCode) return await GetSessaoUsuario(detalhesUsuarioAtt);
                }
            }
            return null!;
        }

        public async Task<SessaoUsuario> GetSessaoUsuario(HttpResponseMessage response)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            return Services.JsonContentService.DeserializeJsonString<SessaoUsuario>(apiResponse);
        }

        private async Task<HttpResponseMessage> GetDetalhesUsuarioApi()
        {
            var httpClient = await addHeaderHttpClient();
            return await httpClient.GetAsync("api/Usuario/usuario-info");
        }

        public async Task<HttpClient> addHeaderHttpClient()
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Services.JsonContentService.DeserializeJsonString<TokenProp>(await GetTokenLocalStorage()).Token);
            return httpClient;
        }
        private async Task<string> GetTokenLocalStorage() => await localStorageService.GetItemAsStringAsync("access_token");
        public async Task SetTokenLocalStorage(string token) => await localStorageService.SetItemAsStringAsync("access_token", token);
        public async Task RemoveTokenLocalStorage() => await localStorageService.RemoveItemAsync("access_token");
        public ClaimsPrincipal SetClaimPrincipal(SessaoUsuario sessaoUsuario)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, sessaoUsuario.Nome!),
                new(ClaimTypes.Email, sessaoUsuario.Email!),
                new(ClaimTypes.Role, sessaoUsuario.Permissao!)
            }, "AcessTokenAuth"));
        }
    }
}
