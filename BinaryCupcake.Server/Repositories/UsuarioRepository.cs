using BinaryCupcake.Server.Data;
using BinaryCupcake.SharedLibrary.DTOs;
using BinaryCupcake.SharedLibrary.Responses;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BinaryCupcake.Server.Repositories
{
    public class UsuarioRepository(AppDbContext appDbContext) : IUsuario
    {
        public async Task<LoginResponse> GetRenovacaoToken(TokenRenovacaoDTO token)
        {
            var tokenDecodificado = WebEncoders.Base64UrlDecode(token.TokenRenovacao!);
            string tokenCodificado = Encoding.UTF8.GetString(tokenDecodificado);

            var buscaToken = await appDbContext.TokenInfo.FirstOrDefaultAsync(x => x.TokenRenovacao!.Equals(tokenCodificado));
            if (buscaToken is null) return null!;

            var (novoTokenAcesso, novoTokenRenovacao) = await GerarTokens();

            await SalvarToken(buscaToken.UsuarioId, novoTokenAcesso, novoTokenRenovacao);

            return new LoginResponse(true, "tokens renovados", novoTokenAcesso, novoTokenRenovacao);
        }
        public async Task<SessaoUsuario> GetUsuarioPorToken(string token)
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(7);
            }

            var resultado = await appDbContext.TokenInfo.FirstOrDefaultAsync(x => x.TokenAcesso!.Equals(token));
            if (resultado is null) return null!;

            var buscaUsuario = await appDbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == resultado.UsuarioId);
            if (buscaUsuario is null) return null!;

            if (resultado.DataExpiracao < DateTime.Now.ToUniversalTime()) return null!;

            var buscaUsuarioPermissao = await appDbContext.PermissaoUsuario.FirstOrDefaultAsync(x => x.UsuarioId == buscaUsuario.Id);
            if (buscaUsuarioPermissao is null) return null!;

            var tipoPermissao = await appDbContext.PermissaoSistema.FirstOrDefaultAsync(x => x.Id.Equals(buscaUsuarioPermissao.PermissaoId));
            if (tipoPermissao is null) return null!;

            return new SessaoUsuario()
            {
                Email = buscaUsuario.Email,
                Nome = buscaUsuario.Nome,
                Permissao = tipoPermissao.Nome,
                Endereco = buscaUsuario.Endereco,
                Id = buscaUsuario.Id,
            };
        }
        public async Task<LoginResponse> Login(LoginDTO login)
        {
            if (login == null) new LoginResponse(false, "O login não pode ser vazio!");

            var buscaUsuario = await appDbContext.Usuarios.FirstOrDefaultAsync(x => x.Email!.ToLower().Equals(login!.Email!.ToLower()));

            if (buscaUsuario == null) return new LoginResponse(false, "Usuario não encontrado!");

            if (!BCrypt.Net.BCrypt.Verify(login!.Senha, buscaUsuario.Senha)) return new LoginResponse(false, "Login ou senha inválidos!");

            var (tokenAcesso, TokenRenovacao) = await GerarTokens();

            await SalvarToken(buscaUsuario.Id, tokenAcesso, TokenRenovacao);
            return new LoginResponse(true, null, tokenAcesso, TokenRenovacao);
        }
        public async Task<ServiceResponse> Registrar(UsuarioDTO usuario)
        {
            if (usuario == null) new ServiceResponse(false, "O usuario não pode ser vazio!");

            var buscaUsuario = await appDbContext.Usuarios.FirstOrDefaultAsync(x => x.Email!.ToLower().Equals(usuario.Email!.ToLower()));

            if (buscaUsuario is not null)
                return new ServiceResponse(false, "Esse usuário já está regitrado.");

            var resultadoUsuario = appDbContext.Usuarios.Add(new Usuario()
            {
                Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha),
                Nome = usuario.Nome,
                Email = usuario.Email
            }).Entity;

            await Commit();

            var VerificaSePermissaoAdm = await appDbContext.PermissaoSistema.FirstOrDefaultAsync(x => x.Nome!.ToLower().Equals("admin"));

            if (VerificaSePermissaoAdm is null)
            {
                var resultado = appDbContext.PermissaoSistema.Add(new PermissaoSistema()
                {
                    Nome = "Admin"
                }).Entity;
                await Commit();

                appDbContext.PermissaoUsuario.Add(new PermissaoUsuario() { PermissaoId = resultado.Id, UsuarioId = resultadoUsuario.Id });
                await Commit();
            }
            else
            {
                var VerificaSePermissaoUsuarioExiste = appDbContext.PermissaoSistema.FirstOrDefault(x => x.Nome!.ToLower().Equals("usuario"));
                int permissaoId = 0;

                if (VerificaSePermissaoUsuarioExiste is null)
                {
                    var resultado = appDbContext.PermissaoSistema.Add(new PermissaoSistema() { Nome = "Usuario" }).Entity;
                    await Commit();
                    permissaoId = resultado.Id;
                }

                appDbContext.PermissaoUsuario.Add(new PermissaoUsuario()
                {
                    PermissaoId = permissaoId == 0 ? VerificaSePermissaoUsuarioExiste!.Id : permissaoId,
                    UsuarioId = resultadoUsuario.Id
                });
                await Commit();
            }
            return new ServiceResponse(true, "Conta criada com sucesso!");
        }
        public async Task<ServiceResponse> AtualizarUsuarioPorId(UsuarioDTO usuarioDto)
        {
            if (usuarioDto == null)
                return new ServiceResponse(false, "Dados do usuário inválidos.");

            var usuario = await appDbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == usuarioDto.Id);

            if (usuario is null) return new ServiceResponse(false, "Usuário não encontrado.");

            usuario.Nome = usuarioDto.Nome ?? usuario.Nome;
            usuario.Email = usuarioDto.Email ?? usuario.Email;
            usuario.Endereco = usuarioDto.Endereco ?? usuario.Endereco;
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);
            
            await Commit();

            return new ServiceResponse(true, "Usuário atualizado com sucesso.");
        }
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        #region token
        private static string GerarToken(int Bytes) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(Bytes));
        private async Task<(string TokenAcesso, string TokenRenovacao)> GerarTokens()
        {
            string tokenAcesso = GerarToken(256);
            string tokenRenovacao = GerarToken(64);

            while (!await VerificaToken(tokenAcesso))
                tokenAcesso = GerarToken(256);

            while (!await VerificaToken(tokenRenovacao))
                tokenRenovacao = GerarToken(256);

            return (tokenAcesso, tokenRenovacao);
        }
        private async Task<bool> VerificaToken(string tokenAcesso = null!, string tokenRenovacao = null!)
        {
            TokenInfo tokenInfo = new();
            if (!string.IsNullOrEmpty(tokenAcesso))
            {
                var getTokenRenovacao = await appDbContext.TokenInfo.FirstOrDefaultAsync(x => x.TokenRenovacao!.Equals(tokenRenovacao));
                return getTokenRenovacao is null ? true : false;
            }
            else
            {
                var getTokenAcesso = await appDbContext.TokenInfo.FirstOrDefaultAsync(x => x.TokenAcesso!.Equals(tokenAcesso));
                return getTokenAcesso is null;
            }

        }
        private async Task SalvarToken(int usuarioId, string tokenAcesso, string tokenRenovacao)
        {
            var buscaUsuario = await appDbContext.TokenInfo.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);
            if (buscaUsuario is null)
            {
                appDbContext.TokenInfo.Add(new TokenInfo()
                {
                    UsuarioId = usuarioId,
                    TokenAcesso = tokenAcesso,
                    TokenRenovacao = tokenRenovacao
                });

                await Commit();
            }
            else
            {
                buscaUsuario.TokenRenovacao = tokenRenovacao;
                buscaUsuario.TokenAcesso = tokenAcesso;
                buscaUsuario.DataExpiracao = DateTime.Now.AddDays(1).ToUniversalTime();
                await Commit();
            }
        }
    }
}
#endregion token
