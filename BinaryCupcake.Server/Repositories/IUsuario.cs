using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Server.Repositories
{
    public interface IUsuario
    {
        Task<ServiceResponse> Registrar(UsuarioDTO usuario);
        Task<LoginResponse> Login(LoginDTO login);
        Task<SessaoUsuario> GetUsuarioPorToken(string token);
        Task<LoginResponse>GetRenovacaoToken(TokenRenovacaoDTO token);
    }
}
