using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Domain.Entities;

public sealed class ProfessionalContact
{
    public int Id { get; set; }

    public ContactMethodType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public int ProfessionalId { get; set; }

    public Professional Professional { get; set; } = null!;
}
