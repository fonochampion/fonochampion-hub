using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface IPatientRegistrationService
{
    Task<PatientRegistrationResult> RegisterAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken = default);
}
