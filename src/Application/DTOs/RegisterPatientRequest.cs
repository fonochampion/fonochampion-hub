using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Application.DTOs;

public sealed record RegisterPatientRequest(
    string FullName,
    DateOnly DateOfBirth,
    string Cid10Code,
    IReadOnlyList<RegisterResponsiblePersonRequest>? ResponsiblePersons = null,
    IReadOnlyList<int>? ProfessionalIds = null);

public sealed record RegisterResponsiblePersonRequest(
    string FullName,
    ResponsiblePersonKinship Kinship,
    IReadOnlyList<RegisterResponsiblePersonContactRequest>? Contacts = null);

public sealed record RegisterResponsiblePersonContactRequest(
    ContactMethodType Type,
    string? Value);
