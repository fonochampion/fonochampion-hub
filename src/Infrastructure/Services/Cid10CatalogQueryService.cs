using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Infrastructure.Data.Cid10;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class Cid10CatalogQueryService(ApplicationDbContext dbContext) : ICid10CatalogQueryService
{
    public Task<bool> HasCurrentCatalogAsync(CancellationToken cancellationToken = default) =>
        dbContext.Cid10CatalogVersions
            .AsNoTracking()
            .AnyAsync(version => version.IsCurrent, cancellationToken);

    public async Task<IReadOnlyList<Cid10CatalogSearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var codeSearchKey = Cid10CatalogTextNormalizer.NormalizeCode(query);
        var descriptionSearchText = Cid10CatalogTextNormalizer.NormalizeSearchText(query.Trim());
        if (codeSearchKey.Length == 0 && descriptionSearchText.Length == 0)
        {
            return [];
        }

        var currentFingerprint = await dbContext.Cid10CatalogVersions
            .AsNoTracking()
            .Where(version => version.IsCurrent)
            .Select(version => version.Fingerprint)
            .SingleOrDefaultAsync(cancellationToken);

        if (currentFingerprint is null)
        {
            return [];
        }

        var entries = await dbContext.Cid10CatalogEntries
            .AsNoTracking()
            .Include(entry => entry.Parent)
                .ThenInclude(parent => parent!.Parent)
                    .ThenInclude(parent => parent!.Parent)
            .Where(entry =>
                entry.VersionFingerprint == currentFingerprint &&
                ((codeSearchKey.Length > 0 && entry.CodeSearchKey == codeSearchKey) ||
                 (descriptionSearchText.Length > 0 && entry.SearchText.Contains(descriptionSearchText))))
            .OrderBy(entry => entry.Level)
            .ThenBy(entry => entry.SortOrder)
            .ToListAsync(cancellationToken);

        return entries.Select(ToSearchResult).ToArray();
    }

    private static Cid10CatalogSearchResult ToSearchResult(Cid10CatalogEntry entry)
    {
        var ancestors = new Stack<Cid10CatalogHierarchyItem>();
        for (var parent = entry.Parent; parent is not null; parent = parent.Parent)
        {
            ancestors.Push(ToHierarchyItem(parent));
        }

        return new Cid10CatalogSearchResult(
            entry.Level,
            entry.Code,
            entry.CodeRangeStart,
            entry.CodeRangeEnd,
            entry.Description,
            ancestors.ToArray());
    }

    private static Cid10CatalogHierarchyItem ToHierarchyItem(Cid10CatalogEntry entry) =>
        new(entry.Level, entry.Code, entry.CodeRangeStart, entry.CodeRangeEnd, entry.Description);
}
