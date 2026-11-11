using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Domain.Enums;

public enum ContactMethodType
{
    [Display(Name = "Telefone")]
    Telephone,

    [Display(Name = "E-mail")]
    Email
}
