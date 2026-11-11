using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Domain.Enums;

public enum ResponsiblePersonKinship
{
    [Display(Name = "Mãe")]
    Mother,

    [Display(Name = "Pai")]
    Father,

    [Display(Name = "Avó/Avô")]
    Grandparent,

    [Display(Name = "Irmã/Irmão")]
    Sibling,

    [Display(Name = "Tia/Tio")]
    AuntOrUncle,

    [Display(Name = "Padrasto/Madrasta")]
    Stepparent,

    [Display(Name = "Cônjuge/Companheiro(a)")]
    SpouseOrPartner,

    [Display(Name = "Tutor/Guardião legal")]
    LegalGuardian,

    [Display(Name = "Outro")]
    Other
}
