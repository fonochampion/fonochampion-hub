using System.ComponentModel.DataAnnotations;
using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Web.Models;

public sealed class CreatePatientViewModel
{
    [Required(ErrorMessage = "Informe o nome completo.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Selecione um CID-10.")]
    public string Cid10Code { get; set; } = string.Empty;

    public string Cid10Description { get; set; } = string.Empty;

    public List<ResponsiblePersonInputViewModel> ResponsiblePersons { get; set; } =
        [new ResponsiblePersonInputViewModel()];
}

public sealed class ResponsiblePersonInputViewModel
{
    public string? FullName { get; set; }

    public ResponsiblePersonKinship? Kinship { get; set; }

    public List<ResponsiblePersonContactInputViewModel> Contacts { get; set; } =
        [new ResponsiblePersonContactInputViewModel()];
}

public sealed class ResponsiblePersonContactInputViewModel
{
    public ResponsiblePersonContactType? Type { get; set; }

    public string? Value { get; set; }
}

public sealed class CreatedPatientViewModel
{
    public required string FullName { get; init; }

    public required DateOnly DateOfBirth { get; init; }

    public required string AgeDisplay { get; init; }

    public required string Cid10Code { get; init; }

    public required string Cid10Description { get; init; }
}
