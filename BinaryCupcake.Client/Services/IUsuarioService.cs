using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Client.Services
{
    public interface IUsuarioService
    {
        Task<ServiceResponse> Registrar(UsuarioDTO usuario);
        Task<LoginResponse> Login(LoginDTO usuario);
    }
}
