using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface ICid10CatalogQueryService
{
    Task<IReadOnlyList<Cid10CatalogSearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}
