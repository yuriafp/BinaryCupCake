using BinaryCupcake.Server.Repositories;
using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BinaryCupcake.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsuarioController(IUsuario usuarioService) : ControllerBase
    {
        [HttpPost("registrar")]

        public async Task<ActionResult<ServiceResponse>> CriarUsuario(UsuarioDTO usuario)
        {
            if (usuario is null) return BadRequest("Usuário não pode ser nulo.");
            var response = await usuarioService.Registrar(usuario);
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginDTO login)
        {
            if (login is null) return BadRequest("Login não pode ser nulo.");
            var response = await usuarioService.Login(login);
            return Ok(response);
        }
        //[HttpGet("usuario-info")]
        //public async Task<IActionResult> GetUsuarioInfo()
        //{
        //    var token = GetTokenDoHeader();
        //    if(string.IsNullOrEmpty(token)) return Unauthorized();

        //    var buscaUsuario = await usuarioService.GetUsuarioPorToken(token!);
        //    if (buscaUsuario is null || string.IsNullOrEmpty(buscaUsuario.Email)) return Unauthorized();

        //    return Ok(buscaUsuario);
        //}

    }
}

