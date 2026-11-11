using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Web.Models;

public sealed class CancellationReasonListViewModel
{
    public IReadOnlyList<CancellationReasonListItemViewModel> Reasons { get; init; } = [];
}

public sealed class CancellationReasonListItemViewModel
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required bool IsActive { get; init; }
}

public abstract class CancellationReasonFormViewModel
{
    [Required(ErrorMessage = "Informe o nome do motivo.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public sealed class CreateCancellationReasonViewModel : CancellationReasonFormViewModel
{
}

public sealed class EditCancellationReasonViewModel : CancellationReasonFormViewModel
{
    public int Id { get; set; }
}
