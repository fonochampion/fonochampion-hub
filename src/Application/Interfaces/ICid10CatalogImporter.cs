using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface ICid10CatalogImporter
{
    Task<Cid10CatalogImportResult> ImportAsync(
        string sourceDirectory,
        CancellationToken cancellationToken = default);
}
