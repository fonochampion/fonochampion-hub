using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Web.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Informe o nome de usuário.")]
    [Display(Name = "Nome de usuário")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool RememberMe { get; set; }
}
