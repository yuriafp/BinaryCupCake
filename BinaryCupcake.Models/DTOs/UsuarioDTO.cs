using System.ComponentModel.DataAnnotations;

namespace BinaryCupcake.SharedLibrary.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Nome { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Senha {  get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Senha))]
        public string? ConfirmaSenha {  get; set; }
    }
}
