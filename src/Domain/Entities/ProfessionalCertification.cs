namespace FonoChampionHub.Domain.Entities;

public sealed class ProfessionalCertification
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Registration { get; set; } = string.Empty;

    public DateOnly CompletionDate { get; set; }

    public int ProfessionalId { get; set; }

    public Professional Professional { get; set; } = null!;
}
