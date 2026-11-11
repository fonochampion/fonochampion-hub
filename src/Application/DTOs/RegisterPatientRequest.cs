namespace FonoChampionHub.Application.DTOs;

public sealed record RegisterPatientRequest(
    string FullName,
    DateOnly DateOfBirth,
    string Cid10Code);
