using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryCupcake.SharedLibrary.DTOs
{
    public class SessaoUsuario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Permissao { get; set; }
        public string? Endereco {  get; set; }
    }
}
