using FonoChampionHub.Domain.Entities;

namespace FonoChampionHub.Application.DTOs;

public enum ProfessionalRegistrationFailure
{
    InvalidFullName,
    InvalidCpf,
    DuplicateCpf,
    InvalidCertification,
    MissingContact,
    InvalidContact
}

public sealed record ProfessionalRegistrationResult(
    Professional? Professional,
    ProfessionalRegistrationFailure? Failure)
{
    public bool Succeeded => Professional is not null && Failure is null;

    public static ProfessionalRegistrationResult Success(Professional professional) =>
        new(professional, null);

    public static ProfessionalRegistrationResult Failed(ProfessionalRegistrationFailure failure) =>
        new(null, failure);
}
