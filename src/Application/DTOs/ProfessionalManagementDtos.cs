namespace FonoChampionHub.Application.DTOs;

public sealed record ProfessionalListItem(
    int Id,
    string FullName,
    string Cpf,
    bool IsActive,
    bool IsDeleted,
    int AssociatedPatientCount);

public enum ProfessionalManagementFailure
{
    ProfessionalNotFound,
    ProfessionalAlreadyDeleted,
    ProfessionalHasAssociatedPatients
}

public sealed record ProfessionalManagementResult(ProfessionalManagementFailure? Failure)
{
    public bool Succeeded => Failure is null;

    public static ProfessionalManagementResult Success() =>
        new((ProfessionalManagementFailure?)null);

    public static ProfessionalManagementResult Failed(ProfessionalManagementFailure failure) =>
        new(failure);
}
