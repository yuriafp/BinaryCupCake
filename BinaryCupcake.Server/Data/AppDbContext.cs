using BinaryCupcake.SharedLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BinaryCupcake.Server.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Produto> Produtos { get; set; } = default!;
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<PermissaoSistema> PermissaoSistema { get; set; } = default!;
        public DbSet<PermissaoUsuario> PermissaoUsuario { get; set; } = default!;
        public DbSet<TokenInfo> TokenInfo { get; set; } = default!;

    }
}
