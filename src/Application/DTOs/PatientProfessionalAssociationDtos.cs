namespace FonoChampionHub.Application.DTOs;

public sealed record ProfessionalOption(int Id, string FullName);

public sealed record PatientListItem(
    int Id,
    string FullName,
    DateOnly DateOfBirth,
    string Cid10Code,
    string Cid10Description,
    int AssociatedProfessionalCount);

public sealed record PatientProfessionalManagement(
    string PatientFullName,
    IReadOnlyList<ProfessionalOption> AssociatedProfessionals,
    IReadOnlyList<ProfessionalOption> AvailableProfessionals);

public enum PatientProfessionalAssociationFailure
{
    PatientNotFound,
    NoProfessionalsSelected,
    InvalidProfessional
}

public sealed record PatientProfessionalAssociationResult(
    PatientProfessionalAssociationFailure? Failure)
{
    public bool Succeeded => Failure is null;

    public static PatientProfessionalAssociationResult Success() =>
        new((PatientProfessionalAssociationFailure?)null);

    public static PatientProfessionalAssociationResult Failed(
        PatientProfessionalAssociationFailure failure) => new(failure);
}
