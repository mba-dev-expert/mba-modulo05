using System.ComponentModel.DataAnnotations;

namespace TelesEducacao.Auth.Application.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Password { get; set; } = string.Empty;
}

public class UsuarioRespostaLogin
{
    public string AccessToken { get; set; } = string.Empty;
    public Guid RefreshToken { get; set; }
    public double ExpiresIn { get; set; }
    public UsuarioToken UsuarioToken { get; set; } = new();
}

public class UsuarioToken
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<UsuarioClaim> Claims { get; set; } = [];
}

public class UsuarioClaim
{
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}