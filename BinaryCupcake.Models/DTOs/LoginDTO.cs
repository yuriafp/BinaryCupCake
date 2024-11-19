using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryCupcake.SharedLibrary.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O campo Email é obrigatório!"), EmailAddress(ErrorMessage = "Digite um email válido!"), DataType(DataType.EmailAddress)]
        public string? Email {  get; set; }
        [Required(ErrorMessage = "O campo Senha é obrigatória!"), DataType(DataType.Password)]
        public string? Senha { get; set; }
    }
}
