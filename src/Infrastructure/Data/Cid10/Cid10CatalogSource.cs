namespace FonoChampionHub.Infrastructure.Data.Cid10;

internal sealed record Cid10ChapterSource(
    int Number,
    string CodeStart,
    string CodeEnd,
    string Description,
    int SortOrder);

internal sealed record Cid10GroupSource(
    string CodeStart,
    string CodeEnd,
    string Description,
    int SortOrder);

internal sealed record Cid10CodeSource(
    string Code,
    string Description,
    int SortOrder);

internal sealed record Cid10CatalogSource(
    string Fingerprint,
    IReadOnlyList<Cid10ChapterSource> Chapters,
    IReadOnlyList<Cid10GroupSource> Groups,
    IReadOnlyList<Cid10CodeSource> Categories,
    IReadOnlyList<Cid10CodeSource> Subcategories);
