using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Application.DTOs;

public sealed record ProfessionalListItem(
    int Id,
    string FullName,
    string Cpf,
    bool IsActive,
    bool IsDeleted,
    int AssociatedPatientCount);

public sealed record ProfessionalDetails(
    int Id,
    string FullName,
    string Cpf,
    IReadOnlyList<ProfessionalCertificationDetails> Certifications,
    IReadOnlyList<ProfessionalContactDetails> Contacts);

public sealed record ProfessionalCertificationDetails(
    int Id,
    string Description,
    string Registration,
    DateOnly CompletionDate);

public sealed record ProfessionalContactDetails(
    int Id,
    ContactMethodType Type,
    string Value);

public sealed record UpdateProfessionalRequest(
    string? FullName,
    string? Cpf,
    IReadOnlyList<UpdateProfessionalCertificationRequest>? Certifications,
    IReadOnlyList<UpdateProfessionalContactRequest>? Contacts);

public sealed record UpdateProfessionalCertificationRequest(
    int? Id,
    string? Description,
    string? Registration,
    DateOnly? CompletionDate);

public sealed record UpdateProfessionalContactRequest(
    int? Id,
    ContactMethodType? Type,
    string? Value);

public enum ProfessionalManagementFailure
{
    ProfessionalNotFound,
    ProfessionalAlreadyDeleted,
    ProfessionalHasAssociatedPatients,
    InvalidFullName,
    InvalidCpf,
    DuplicateCpf,
    InvalidCertification,
    MissingContact,
    InvalidContact
}

public sealed record ProfessionalManagementResult(ProfessionalManagementFailure? Failure)
{
    public bool Succeeded => Failure is null;

    public static ProfessionalManagementResult Success() =>
        new((ProfessionalManagementFailure?)null);

    public static ProfessionalManagementResult Failed(ProfessionalManagementFailure failure) =>
        new(failure);
}
