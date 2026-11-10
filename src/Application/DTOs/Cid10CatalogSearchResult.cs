using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Application.DTOs;

public sealed record Cid10CatalogHierarchyItem(
    Cid10CatalogEntryLevel Level,
    string? Code,
    string? CodeRangeStart,
    string? CodeRangeEnd,
    string Description);

public sealed record Cid10CatalogSearchResult(
    Cid10CatalogEntryLevel Level,
    string? Code,
    string? CodeRangeStart,
    string? CodeRangeEnd,
    string Description,
    IReadOnlyList<Cid10CatalogHierarchyItem> Ancestors);
