using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface IProfessionalRegistrationService
{
    Task<ProfessionalRegistrationResult> RegisterAsync(
        RegisterProfessionalRequest request,
        CancellationToken cancellationToken = default);
}
