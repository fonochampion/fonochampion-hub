using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class ProfessionalManagementService(ApplicationDbContext dbContext)
    : IProfessionalManagementService
{
    public async Task<IReadOnlyList<ProfessionalListItem>> GetProfessionalsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Professionals
            .AsNoTracking()
            .OrderBy(professional => professional.FullName)
            .Select(professional => new ProfessionalListItem(
                professional.Id,
                professional.FullName,
                professional.Cpf,
                professional.IsActive,
                professional.IsDeleted,
                professional.PatientProfessionals.Count))
            .ToListAsync(cancellationToken);

    public async Task<ProfessionalDetails?> GetProfessionalForEditAsync(
        int professionalId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Professionals
            .AsNoTracking()
            .Where(professional => professional.Id == professionalId &&
                                   !professional.IsDeleted)
            .Select(professional => new ProfessionalDetails(
                professional.Id,
                professional.FullName,
                professional.Cpf,
                professional.Certifications
                    .OrderBy(certification => certification.Id)
                    .Select(certification => new ProfessionalCertificationDetails(
                        certification.Id,
                        certification.Description,
                        certification.Registration,
                        certification.CompletionDate))
                    .ToList(),
                professional.Contacts
                    .OrderBy(contact => contact.Id)
                    .Select(contact => new ProfessionalContactDetails(
                        contact.Id,
                        contact.Type,
                        contact.Value))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<ProfessionalManagementResult> UpdateAsync(
        int professionalId,
        UpdateProfessionalRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var professional = await dbContext.Professionals
            .Include(item => item.Certifications)
            .Include(item => item.Contacts)
            .SingleOrDefaultAsync(
                item => item.Id == professionalId,
                cancellationToken);
        if (professional is null)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalNotFound);
        }

        if (professional.IsDeleted)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalAlreadyDeleted);
        }

        var fullName = request.FullName?.Trim();
        if (string.IsNullOrEmpty(fullName))
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.InvalidFullName);
        }

        var cpf = BrazilianCpfValidator.NormalizeAndValidate(request.Cpf);
        if (cpf is null)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.InvalidCpf);
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var certifications = new List<(
            ProfessionalCertification? Existing,
            string Description,
            string Registration,
            DateOnly CompletionDate)>();
        var certificationIds = new HashSet<int>();
        foreach (var item in request.Certifications ?? [])
        {
            if (item is null)
            {
                return ProfessionalManagementResult.Failed(
                    ProfessionalManagementFailure.InvalidCertification);
            }

            var description = item.Description?.Trim();
            var registration = item.Registration?.Trim();
            if (string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(registration) ||
                !registration.All(char.IsLetterOrDigit) ||
                item.CompletionDate is not { } completionDate ||
                completionDate > today)
            {
                return ProfessionalManagementResult.Failed(
                    ProfessionalManagementFailure.InvalidCertification);
            }

            ProfessionalCertification? existing = null;
            if (item.Id is { } certificationId)
            {
                if (!certificationIds.Add(certificationId))
                {
                    return ProfessionalManagementResult.Failed(
                        ProfessionalManagementFailure.InvalidCertification);
                }

                existing = professional.Certifications
                    .SingleOrDefault(certification => certification.Id == certificationId);
                if (existing is null)
                {
                    return ProfessionalManagementResult.Failed(
                        ProfessionalManagementFailure.InvalidCertification);
                }
            }

            certifications.Add((existing, description, registration, completionDate));
        }

        var contacts = new List<(
            ProfessionalContact? Existing,
            ContactMethodType Type,
            string Value)>();
        var contactIds = new HashSet<int>();
        foreach (var item in request.Contacts ?? [])
        {
            if (item is null ||
                item.Type is not { } contactType ||
                !Enum.IsDefined(contactType))
            {
                return ProfessionalManagementResult.Failed(
                    ProfessionalManagementFailure.InvalidContact);
            }

            var value = item.Value?.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return ProfessionalManagementResult.Failed(
                    ProfessionalManagementFailure.InvalidContact);
            }

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
                return ProfessionalManagementResult.Failed(
                    ProfessionalManagementFailure.InvalidContact);
            }

            ProfessionalContact? existing = null;
            if (item.Id is { } contactId)
            {
                if (!contactIds.Add(contactId))
                {
                    return ProfessionalManagementResult.Failed(
                        ProfessionalManagementFailure.InvalidContact);
                }

                existing = professional.Contacts
                    .SingleOrDefault(contact => contact.Id == contactId);
                if (existing is null)
                {
                    return ProfessionalManagementResult.Failed(
                        ProfessionalManagementFailure.InvalidContact);
                }
            }

            contacts.Add((existing, contactType, value));
        }

        if (contacts.Count == 0)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.MissingContact);
        }

        var duplicateCpfExists = await dbContext.Professionals
            .AsNoTracking()
            .AnyAsync(
                item => item.Id != professionalId && item.Cpf == cpf,
                cancellationToken);
        if (duplicateCpfExists)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.DuplicateCpf);
        }

        professional.FullName = fullName;
        professional.Cpf = cpf;
        foreach (var certification in professional.Certifications
                     .Where(item => !certificationIds.Contains(item.Id))
                     .ToArray())
        {
            dbContext.ProfessionalCertifications.Remove(certification);
        }

        foreach (var certification in certifications)
        {
            if (certification.Existing is { } existing)
            {
                existing.Description = certification.Description;
                existing.Registration = certification.Registration;
                existing.CompletionDate = certification.CompletionDate;
            }
            else
            {
                professional.Certifications.Add(new ProfessionalCertification
                {
                    Description = certification.Description,
                    Registration = certification.Registration,
                    CompletionDate = certification.CompletionDate,
                    Professional = professional
                });
            }
        }

        foreach (var contact in professional.Contacts
                     .Where(item => !contactIds.Contains(item.Id))
                     .ToArray())
        {
            dbContext.ProfessionalContacts.Remove(contact);
        }

        foreach (var contact in contacts)
        {
            if (contact.Existing is { } existing)
            {
                existing.Type = contact.Type;
                existing.Value = contact.Value;
            }
            else
            {
                professional.Contacts.Add(new ProfessionalContact
                {
                    Type = contact.Type,
                    Value = contact.Value,
                    Professional = professional
                });
            }
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateCpfConstraintViolation(exception))
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.DuplicateCpf);
        }

        return ProfessionalManagementResult.Success();
    }

    public async Task<ProfessionalManagementResult> SetActiveAsync(
        int professionalId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var professional = await dbContext.Professionals
            .SingleOrDefaultAsync(
                professional => professional.Id == professionalId,
                cancellationToken);
        if (professional is null)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalNotFound);
        }

        if (professional.IsDeleted)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalAlreadyDeleted);
        }

        professional.IsActive = isActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProfessionalManagementResult.Success();
    }

    public async Task<ProfessionalManagementResult> DeleteAsync(
        int professionalId,
        CancellationToken cancellationToken = default)
    {
        var professional = await dbContext.Professionals
            .SingleOrDefaultAsync(
                professional => professional.Id == professionalId,
                cancellationToken);
        if (professional is null)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalNotFound);
        }

        if (professional.IsDeleted)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalAlreadyDeleted);
        }

        var hasAssociatedPatients = await dbContext.PatientProfessionals
            .AnyAsync(
                association => association.ProfessionalId == professionalId,
                cancellationToken);
        if (hasAssociatedPatients)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalHasAssociatedPatients);
        }

        professional.IsActive = false;
        professional.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProfessionalManagementResult.Success();
    }

    private static bool IsDuplicateCpfConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: Professional.CpfUniqueIndexName
        };
}
