using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BinaryCupcake.Client.Autenticacao
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AutenticacaoService _autenticacaoService;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(AutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var getSessaoUsuario = await _autenticacaoService.GetDetalhesUsuario();
                if (getSessaoUsuario is null || string.IsNullOrEmpty(getSessaoUsuario.Email))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claimsPrincipal = _autenticacaoService.SetClaimPrincipal(getSessaoUsuario);
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task AtualizaAuthenticationState(TokenProp tokenProp)
        {
            ClaimsPrincipal claimsPrincipal = _anonymous;

            if (tokenProp is not null && !string.IsNullOrEmpty(tokenProp.Token))
            {
                await _autenticacaoService.SetTokenLocalStorage(Services.JsonContentService.SerializeObj(tokenProp));
                var getSessaoUsuario = await _autenticacaoService.GetDetalhesUsuario();
                if (getSessaoUsuario is not null && !string.IsNullOrEmpty(getSessaoUsuario.Email))
                {
                    claimsPrincipal = _autenticacaoService.SetClaimPrincipal(getSessaoUsuario);
                }
            }
            else
            {
                await _autenticacaoService.RemoveTokenLocalStorage();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
