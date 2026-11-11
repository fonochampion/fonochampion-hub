using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface IProfessionalManagementService
{
    Task<IReadOnlyList<ProfessionalListItem>> GetProfessionalsAsync(
        CancellationToken cancellationToken = default);

    Task<ProfessionalDetails?> GetProfessionalForEditAsync(
        int professionalId,
        CancellationToken cancellationToken = default);

    Task<ProfessionalManagementResult> UpdateAsync(
        int professionalId,
        UpdateProfessionalRequest request,
        CancellationToken cancellationToken = default);

    Task<ProfessionalManagementResult> SetActiveAsync(
        int professionalId,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<ProfessionalManagementResult> DeleteAsync(
        int professionalId,
        CancellationToken cancellationToken = default);
}
