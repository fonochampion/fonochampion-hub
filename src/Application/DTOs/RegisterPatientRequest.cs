using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Application.DTOs;

public sealed record RegisterPatientRequest(
    string FullName,
    DateOnly DateOfBirth,
    string Cid10Code,
    IReadOnlyList<RegisterResponsiblePersonRequest>? ResponsiblePersons = null);

public sealed record RegisterResponsiblePersonRequest(
    string FullName,
    ResponsiblePersonKinship Kinship);
