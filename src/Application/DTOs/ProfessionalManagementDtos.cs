namespace FonoChampionHub.Application.DTOs;

public sealed record ProfessionalListItem(
    int Id,
    string FullName,
    string Cpf,
    bool IsActive,
    int AssociatedPatientCount);

public enum ProfessionalManagementFailure
{
    ProfessionalNotFound
}

public sealed record ProfessionalManagementResult(ProfessionalManagementFailure? Failure)
{
    public bool Succeeded => Failure is null;

    public static ProfessionalManagementResult Success() =>
        new((ProfessionalManagementFailure?)null);

    public static ProfessionalManagementResult Failed(ProfessionalManagementFailure failure) =>
        new(failure);
}
