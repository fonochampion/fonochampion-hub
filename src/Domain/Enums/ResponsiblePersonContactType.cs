using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Domain.Enums;

public enum ResponsiblePersonContactType
{
    [Display(Name = "Telefone")]
    Telephone,

    [Display(Name = "E-mail")]
    Email
}
