using FonoChampionHub.Domain.Entities;

namespace FonoChampionHub.Application.DTOs;

public enum PatientRegistrationFailure
{
    InvalidFullName,
    FutureDateOfBirth,
    DuplicatePatient,
    InvalidResponsiblePerson,
    DuplicateResponsiblePerson,
    CatalogUnavailable,
    InvalidCid10Code
}

public sealed record PatientRegistrationResult(
    Patient? Patient,
    PatientRegistrationFailure? Failure)
{
    public bool Succeeded => Patient is not null && Failure is null;

    public static PatientRegistrationResult Success(Patient patient) =>
        new(patient, null);

    public static PatientRegistrationResult Failed(PatientRegistrationFailure failure) =>
        new(null, failure);
}
