using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Domain.Entities;

public sealed class ResponsiblePersonContact
{
    public int Id { get; set; }

    public ContactMethodType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public int ResponsiblePersonId { get; set; }

    public ResponsiblePerson ResponsiblePerson { get; set; } = null!;
}
