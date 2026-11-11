using FonoChampionHub.Application.DTOs;

namespace FonoChampionHub.Application.Interfaces;

public interface IPatientProfessionalAssociationService
{
    Task<IReadOnlyList<ProfessionalOption>> GetRegistrationOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientListItem>> GetPatientsAsync(
        CancellationToken cancellationToken = default);

    Task<PatientProfessionalManagement?> GetManagementAsync(
        int patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProfessionalOption>?> GetSchedulingOptionsAsync(
        int patientId,
        CancellationToken cancellationToken = default);

    Task<PatientProfessionalAssociationResult> AddToPatientAsync(
        int patientId,
        IReadOnlyCollection<int>? professionalIds,
        CancellationToken cancellationToken = default);

    Task<PatientProfessionalAssociationResult> AssociateForSchedulingAsync(
        int patientId,
        IReadOnlyCollection<int>? professionalIds,
        CancellationToken cancellationToken = default);
}
