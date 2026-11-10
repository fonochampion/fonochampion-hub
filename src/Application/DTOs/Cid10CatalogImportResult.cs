namespace FonoChampionHub.Application.DTOs;

public sealed record Cid10CatalogImportResult(
    string Fingerprint,
    int ChapterCount,
    int GroupCount,
    int CategoryCount,
    int SubcategoryCount,
    bool WasAlreadyCurrent)
{
    public int TotalEntryCount =>
        ChapterCount + GroupCount + CategoryCount + SubcategoryCount;
}
