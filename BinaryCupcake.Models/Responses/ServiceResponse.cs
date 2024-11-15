namespace BinaryCupcake.SharedLibrary.Responses
{
    public record class ServiceResponse(bool Flag, string Mensagem = null!);
    public record class LoginResponse(bool Flag, string? Mensagem, string Token = null!, string TokenRenovacao = null!);
}
