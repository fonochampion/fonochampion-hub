using System.ComponentModel.DataAnnotations;
using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Web.Models;

public sealed class CreateProfessionalViewModel
{
    [Required(ErrorMessage = "Informe o nome completo.")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Informe o CPF.")]
    public string? Cpf { get; set; }

    public List<ProfessionalCertificationInputViewModel> Certifications { get; set; } =
        [new ProfessionalCertificationInputViewModel()];

    public List<ProfessionalContactInputViewModel> Contacts { get; set; } =
        [new ProfessionalContactInputViewModel()];
}

public sealed class ProfessionalCertificationInputViewModel
{
    public string? Description { get; set; }

    public string? Registration { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? CompletionDate { get; set; }
}

public sealed class ProfessionalContactInputViewModel
{
    public ContactMethodType? Type { get; set; }

    public string? Value { get; set; }
}

public sealed class CreatedProfessionalViewModel
{
    public required string FullName { get; init; }
}

public sealed class ProfessionalListItemViewModel
{
    public required int Id { get; init; }

    public required string FullName { get; init; }

    public required string Cpf { get; init; }

    public required bool IsActive { get; init; }

    public required bool IsDeleted { get; init; }

    public required int AssociatedPatientCount { get; init; }
}
