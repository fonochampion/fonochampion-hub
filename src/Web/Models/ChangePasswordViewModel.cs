using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Web.Models;

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [MinLength(12, ErrorMessage = "A senha deve conter no mínimo 12 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NewPassword { get; set; } = string.Empty;
}
