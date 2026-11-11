using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class ProfessionalRegistrationService(ApplicationDbContext dbContext)
    : IProfessionalRegistrationService
{
    public async Task<ProfessionalRegistrationResult> RegisterAsync(
        RegisterProfessionalRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return ProfessionalRegistrationResult.Failed(
                ProfessionalRegistrationFailure.InvalidFullName);
        }

        var cpf = BrazilianCpfValidator.NormalizeAndValidate(request.Cpf);
        if (cpf is null)
        {
            return ProfessionalRegistrationResult.Failed(
                ProfessionalRegistrationFailure.InvalidCpf);
        }

        var contactRequests = request.Contacts ?? [];
        if (contactRequests.Count == 0)
        {
            return ProfessionalRegistrationResult.Failed(
                ProfessionalRegistrationFailure.MissingContact);
        }

        var contacts = new List<ProfessionalContact>();
        foreach (var contactRequest in contactRequests)
        {
            if (contactRequest is null ||
                contactRequest.Type is not { } contactType ||
                !Enum.IsDefined(contactType) ||
                string.IsNullOrWhiteSpace(contactRequest.Value))
            {
                return ProfessionalRegistrationResult.Failed(
                    ProfessionalRegistrationFailure.InvalidContact);
            }

            var value = contactRequest.Value.Trim();
            var isValid = contactType switch
            {
                ContactMethodType.Telephone =>
                    ContactValueValidator.IsValidBrazilianTelephone(value),
                ContactMethodType.Email =>
                    ContactValueValidator.IsValidEmailAddress(value),
                _ => false
            };
            if (!isValid)
            {
                return ProfessionalRegistrationResult.Failed(
                    ProfessionalRegistrationFailure.InvalidContact);
            }

            contacts.Add(new ProfessionalContact
            {
                Type = contactType,
                Value = value
            });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var certifications = new List<ProfessionalCertification>();
        foreach (var certificationRequest in request.Certifications ?? [])
        {
            if (certificationRequest is null)
            {
                return ProfessionalRegistrationResult.Failed(
                    ProfessionalRegistrationFailure.InvalidCertification);
            }

            var description = certificationRequest.Description?.Trim();
            var registration = certificationRequest.Registration?.Trim();
            if (string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(registration) ||
                !registration.All(char.IsLetterOrDigit) ||
                certificationRequest.CompletionDate is not { } completionDate ||
            completionDate > today)
            {
                return ProfessionalRegistrationResult.Failed(
                    ProfessionalRegistrationFailure.InvalidCertification);
            }

            certifications.Add(new ProfessionalCertification
            {
                Description = description,
                Registration = registration,
                CompletionDate = completionDate
            });
        }

        var duplicateExists = await dbContext.Professionals
            .AsNoTracking()
            .AnyAsync(professional => professional.Cpf == cpf, cancellationToken);
        if (duplicateExists)
        {
            return ProfessionalRegistrationResult.Failed(
                ProfessionalRegistrationFailure.DuplicateCpf);
        }

        var professional = new Professional
        {
            FullName = request.FullName.Trim(),
            Cpf = cpf,
            Certifications = certifications,
            Contacts = contacts
        };

        dbContext.Professionals.Add(professional);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateCpfConstraintViolation(exception))
        {
            return ProfessionalRegistrationResult.Failed(
                ProfessionalRegistrationFailure.DuplicateCpf);
        }

        return ProfessionalRegistrationResult.Success(professional);
    }

    private static bool IsDuplicateCpfConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: Professional.CpfUniqueIndexName
        };
}
