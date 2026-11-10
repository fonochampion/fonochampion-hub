using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Data.Cid10;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class Cid10CatalogImporter(ApplicationDbContext dbContext) : ICid10CatalogImporter
{
    private readonly Cid10CatalogSourceReader _sourceReader = new();

    public async Task<Cid10CatalogImportResult> ImportAsync(
        string sourceDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);

        var source = _sourceReader.Read(sourceDirectory);
        var prepared = PrepareCatalog(source, source.Fingerprint);

        await using var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                cancellationToken)
            : null;

        var currentVersions = await dbContext.Cid10CatalogVersions
            .Where(version => version.IsCurrent)
            .ToListAsync(cancellationToken);

        if (currentVersions.Count > 1)
        {
            throw new InvalidOperationException("The CID-10 catalog has more than one current version.");
        }

        var existingVersion = await dbContext.Cid10CatalogVersions
            .SingleOrDefaultAsync(version => version.Fingerprint == source.Fingerprint, cancellationToken);

        if (existingVersion?.IsCurrent == true)
        {
            return prepared.ToResult(wasAlreadyCurrent: true);
        }

        foreach (var currentVersion in currentVersions)
        {
            currentVersion.IsCurrent = false;
        }

        if (currentVersions.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (existingVersion is not null)
        {
            existingVersion.IsCurrent = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            dbContext.Cid10CatalogVersions.Add(prepared.Version);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return prepared.ToResult(wasAlreadyCurrent: false);
    }

    private static PreparedCatalog PrepareCatalog(Cid10CatalogSource source, string fingerprint)
    {
        var version = new Cid10CatalogVersion
        {
            Fingerprint = fingerprint,
            ImportedAtUtc = DateTimeOffset.UtcNow,
            IsCurrent = true
        };

        var chapters = new List<Cid10CatalogEntry>();
        var chapterNumbers = new HashSet<int>();
        foreach (var chapter in source.Chapters)
        {
            ValidateRange(chapter.CodeStart, chapter.CodeEnd, "chapter");
            if (!chapterNumbers.Add(chapter.Number))
            {
                throw new InvalidDataException($"Duplicate CID-10 chapter number: {chapter.Number}.");
            }

            var entry = CreateEntry(
                version,
                Cid10CatalogEntryLevel.Chapter,
                chapter.SortOrder,
                chapter.Description,
                codeRangeStart: chapter.CodeStart,
                codeRangeEnd: chapter.CodeEnd,
                chapterNumber: chapter.Number);
            AddEntry(version, entry);
            chapters.Add(entry);
        }

        var groups = new List<(Cid10GroupSource Source, Cid10CatalogEntry Entry)>();
        foreach (var group in source.Groups)
        {
            ValidateRange(group.CodeStart, group.CodeEnd, "group");
            var parent = FindSingleParent(
                chapters,
                chapter => ContainsRange(chapter.CodeRangeStart!, chapter.CodeRangeEnd!, group.CodeStart, group.CodeEnd),
                $"group range {group.CodeStart}-{group.CodeEnd}");
            var entry = CreateEntry(
                version,
                Cid10CatalogEntryLevel.Group,
                group.SortOrder,
                group.Description,
                parent,
                codeRangeStart: group.CodeStart,
                codeRangeEnd: group.CodeEnd);
            AddEntry(version, entry);
            groups.Add((group, entry));
        }

        var categories = new Dictionary<string, Cid10CatalogEntry>(StringComparer.Ordinal);
        foreach (var category in source.Categories)
        {
            if (categories.ContainsKey(category.Code))
            {
                throw new InvalidDataException($"Duplicate CID-10 category code: {category.Code}.");
            }

            // Group ranges overlap; attach categories to the narrowest containing range.
            var parent = FindMostSpecificParent(
                groups,
                group => ContainsRange(group.Source.CodeStart, group.Source.CodeEnd, category.Code, category.Code),
                group => group.Source.CodeStart,
                group => group.Source.CodeEnd,
                $"category {category.Code}").Entry;
            var entry = CreateEntry(
                version,
                Cid10CatalogEntryLevel.Category,
                category.SortOrder,
                category.Description,
                parent,
                code: category.Code);
            AddEntry(version, entry);
            categories.Add(category.Code, entry);
        }

        var sourceSubcategoryCodes = new HashSet<string>(StringComparer.Ordinal);
        var subcategoryCount = 0;
        foreach (var subcategory in source.Subcategories)
        {
            if (!sourceSubcategoryCodes.Add(subcategory.Code))
            {
                throw new InvalidDataException($"Duplicate CID-10 subcategory source code: {subcategory.Code}.");
            }

            if (subcategory.Code.Length == 3)
            {
                if (!categories.TryGetValue(subcategory.Code, out var duplicateCategory) ||
                    Cid10CatalogTextNormalizer.NormalizeSearchText(duplicateCategory.Description) !=
                    Cid10CatalogTextNormalizer.NormalizeSearchText(subcategory.Description))
                {
                    throw new InvalidDataException(
                        $"Three-character subcategory '{subcategory.Code}' does not match its category.");
                }

                continue;
            }

            var parentCode = subcategory.Code[..3];
            if (!categories.TryGetValue(parentCode, out var categoryParent))
            {
                throw new InvalidDataException(
                    $"CID-10 subcategory '{subcategory.Code}' has no category '{parentCode}'.");
            }

            var entry = CreateEntry(
                version,
                Cid10CatalogEntryLevel.Subcategory,
                subcategory.SortOrder,
                subcategory.Description,
                categoryParent,
                code: FormatCode(subcategory.Code));
            AddEntry(version, entry);
            subcategoryCount++;
        }

        return new PreparedCatalog(
            version,
            source.Chapters.Count,
            source.Groups.Count,
            source.Categories.Count,
            subcategoryCount);
    }

    private static Cid10CatalogEntry CreateEntry(
        Cid10CatalogVersion version,
        Cid10CatalogEntryLevel level,
        int sortOrder,
        string description,
        Cid10CatalogEntry? parent = null,
        string? code = null,
        string? codeRangeStart = null,
        string? codeRangeEnd = null,
        int? chapterNumber = null)
    {
        return new Cid10CatalogEntry
        {
            Version = version,
            VersionFingerprint = version.Fingerprint,
            Parent = parent,
            Level = level,
            SortOrder = sortOrder,
            ChapterNumber = chapterNumber,
            Code = code,
            CodeSearchKey = code is null ? null : Cid10CatalogTextNormalizer.NormalizeCode(code),
            CodeRangeStart = codeRangeStart,
            CodeRangeEnd = codeRangeEnd,
            Description = description,
            SearchText = Cid10CatalogTextNormalizer.NormalizeSearchText(description)
        };
    }

    private static void AddEntry(Cid10CatalogVersion version, Cid10CatalogEntry entry) =>
        version.Entries.Add(entry);

    private static T FindSingleParent<T>(
        IEnumerable<T> candidates,
        Func<T, bool> matches,
        string childDescription)
    {
        var matchingParents = candidates.Where(matches).Take(2).ToArray();
        return matchingParents.Length switch
        {
            1 => matchingParents[0],
            0 => throw new InvalidDataException($"No parent found for {childDescription}."),
            _ => throw new InvalidDataException($"More than one parent found for {childDescription}.")
        };
    }

    private static T FindMostSpecificParent<T>(
        IEnumerable<T> candidates,
        Func<T, bool> matches,
        Func<T, string> rangeStart,
        Func<T, string> rangeEnd,
        string childDescription)
    {
        var matchingParents = candidates.Where(matches).ToArray();
        var mostSpecificParents = matchingParents
            .Where(candidate => !matchingParents.Any(other =>
                !EqualityComparer<T>.Default.Equals(candidate, other) &&
                ContainsRange(rangeStart(candidate), rangeEnd(candidate), rangeStart(other), rangeEnd(other)) &&
                (rangeStart(candidate) != rangeStart(other) || rangeEnd(candidate) != rangeEnd(other))))
            .ToArray();

        return mostSpecificParents.Length switch
        {
            1 => mostSpecificParents[0],
            0 => throw new InvalidDataException($"No parent found for {childDescription}."),
            _ => throw new InvalidDataException($"More than one most specific parent found for {childDescription}.")
        };
    }

    private static bool ContainsRange(string parentStart, string parentEnd, string childStart, string childEnd) =>
        string.CompareOrdinal(parentStart, childStart) <= 0 &&
        string.CompareOrdinal(parentEnd, childEnd) >= 0;

    private static void ValidateRange(string start, string end, string level)
    {
        if (string.CompareOrdinal(start, end) > 0)
        {
            throw new InvalidDataException($"Invalid CID-10 {level} range: {start}-{end}.");
        }
    }

    private static string FormatCode(string code) =>
        code.Length == 4 ? $"{code[..3]}.{code[3]}" : code;

    private sealed record PreparedCatalog(
        Cid10CatalogVersion Version,
        int ChapterCount,
        int GroupCount,
        int CategoryCount,
        int SubcategoryCount)
    {
        public Cid10CatalogImportResult ToResult(bool wasAlreadyCurrent) =>
            new(Version.Fingerprint, ChapterCount, GroupCount, CategoryCount, SubcategoryCount, wasAlreadyCurrent);
    }
}
