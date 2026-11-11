using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Application.DTOs;

public sealed record RegisterProfessionalRequest(
    string? FullName,
    string? Cpf,
    IReadOnlyList<RegisterProfessionalCertificationRequest>? Certifications = null,
    IReadOnlyList<RegisterProfessionalContactRequest>? Contacts = null);

public sealed record RegisterProfessionalCertificationRequest(
    string? Description,
    string? Registration,
    DateOnly? CompletionDate);

public sealed record RegisterProfessionalContactRequest(
    ContactMethodType? Type,
    string? Value);
