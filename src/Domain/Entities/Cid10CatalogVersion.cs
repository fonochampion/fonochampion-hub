namespace FonoChampionHub.Domain.Entities;

public sealed class Cid10CatalogVersion
{
    public string Fingerprint { get; set; } = string.Empty;

    public DateTimeOffset ImportedAtUtc { get; set; }

    public bool IsCurrent { get; set; }

    public ICollection<Cid10CatalogEntry> Entries { get; set; } = [];
}
