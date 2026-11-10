using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Domain.Entities;

public sealed class Cid10CatalogEntry
{
    public int Id { get; set; }

    public string VersionFingerprint { get; set; } = string.Empty;

    public Cid10CatalogVersion Version { get; set; } = null!;

    public int? ParentId { get; set; }

    public Cid10CatalogEntry? Parent { get; set; }

    public ICollection<Cid10CatalogEntry> Children { get; set; } = [];

    public Cid10CatalogEntryLevel Level { get; set; }

    public int SortOrder { get; set; }

    public int? ChapterNumber { get; set; }

    public string? Code { get; set; }

    public string? CodeSearchKey { get; set; }

    public string? CodeRangeStart { get; set; }

    public string? CodeRangeEnd { get; set; }

    public string Description { get; set; } = string.Empty;

    public string SearchText { get; set; } = string.Empty;
}
