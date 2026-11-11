using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class PatientRegistrationService(ApplicationDbContext dbContext)
    : IPatientRegistrationService
{
    public async Task<PatientRegistrationResult> RegisterAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.InvalidFullName);
        }

        var normalizedFullName = Patient.NormalizeFullNameForComparison(request.FullName);
        if (normalizedFullName.Length == 0)
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.InvalidFullName);
        }

        var responsiblePersons = new List<ResponsiblePerson>();
        var responsiblePersonNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var responsiblePersonRequest in request.ResponsiblePersons ?? [])
        {
            if (string.IsNullOrWhiteSpace(responsiblePersonRequest.FullName) ||
                !Enum.IsDefined(responsiblePersonRequest.Kinship))
            {
                return PatientRegistrationResult.Failed(
                    PatientRegistrationFailure.InvalidResponsiblePerson);
            }

            var normalizedResponsiblePersonName =
                ResponsiblePerson.NormalizeFullNameForComparison(responsiblePersonRequest.FullName);
            if (normalizedResponsiblePersonName.Length == 0)
            {
                return PatientRegistrationResult.Failed(
                    PatientRegistrationFailure.InvalidResponsiblePerson);
            }

            if (!responsiblePersonNames.Add(normalizedResponsiblePersonName))
            {
                return PatientRegistrationResult.Failed(
                    PatientRegistrationFailure.DuplicateResponsiblePerson);
            }

            var contacts = new List<ResponsiblePersonContact>();
            var contactRequests = responsiblePersonRequest.Contacts ?? [];
            if (contactRequests.Count == 0)
            {
                return PatientRegistrationResult.Failed(
                    PatientRegistrationFailure.MissingResponsiblePersonContact);
            }

            foreach (var contactRequest in contactRequests)
            {
                if (contactRequest is null ||
                    !Enum.IsDefined(contactRequest.Type) ||
                    string.IsNullOrWhiteSpace(contactRequest.Value))
                {
                    return PatientRegistrationResult.Failed(
                        PatientRegistrationFailure.InvalidResponsiblePersonContact);
                }

                var contactValue = contactRequest.Value.Trim();
                var isValid = contactRequest.Type switch
                {
                    ContactMethodType.Telephone =>
                        ContactValueValidator.IsValidBrazilianTelephone(contactValue),
                    ContactMethodType.Email =>
                        ContactValueValidator.IsValidEmailAddress(contactValue),
                    _ => false
                };
                if (!isValid)
                {
                    return PatientRegistrationResult.Failed(
                        PatientRegistrationFailure.InvalidResponsiblePersonContact);
                }

                contacts.Add(new ResponsiblePersonContact
                {
                    Type = contactRequest.Type,
                    Value = contactValue
                });
            }

            responsiblePersons.Add(new ResponsiblePerson
            {
                FullName = responsiblePersonRequest.FullName.Trim(),
                NormalizedFullName = normalizedResponsiblePersonName,
                Kinship = responsiblePersonRequest.Kinship,
                Contacts = contacts
            });
        }

        if (request.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.FutureDateOfBirth);
        }

        var catalogIsAvailable = await dbContext.Cid10CatalogVersions
            .AsNoTracking()
            .AnyAsync(version => version.IsCurrent, cancellationToken);
        if (!catalogIsAvailable)
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.CatalogUnavailable);
        }

        var cidEntry = await dbContext.Cid10CatalogEntries
            .Include(entry => entry.Version)
            .SingleOrDefaultAsync(
                entry => entry.Version.IsCurrent &&
                         entry.Code == request.Cid10Code &&
                         (entry.Level == Cid10CatalogEntryLevel.Category ||
                          entry.Level == Cid10CatalogEntryLevel.Subcategory),
                cancellationToken);
        if (cidEntry is null)
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.InvalidCid10Code);
        }

        var duplicateExists = await dbContext.Patients
            .AsNoTracking()
            .AnyAsync(
                patient => patient.NormalizedFullName == normalizedFullName &&
                           patient.DateOfBirth == request.DateOfBirth,
                cancellationToken);
        if (duplicateExists)
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.DuplicatePatient);
        }

        var patient = new Patient
        {
            FullName = request.FullName.Trim(),
            NormalizedFullName = normalizedFullName,
            DateOfBirth = request.DateOfBirth,
            Cid10CatalogEntryId = cidEntry.Id,
            Cid10CatalogEntry = cidEntry,
            ResponsiblePersons = responsiblePersons
        };

        dbContext.Patients.Add(patient);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicatePatientConstraintViolation(exception))
        {
            return PatientRegistrationResult.Failed(PatientRegistrationFailure.DuplicatePatient);
        }
        catch (DbUpdateException exception) when (IsDuplicateResponsiblePersonConstraintViolation(exception))
        {
            return PatientRegistrationResult.Failed(
                PatientRegistrationFailure.DuplicateResponsiblePerson);
        }

        return PatientRegistrationResult.Success(patient);
    }

    private static bool IsDuplicatePatientConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: Patient.FullNameDateOfBirthIndexName
        };

    private static bool IsDuplicateResponsiblePersonConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: ResponsiblePerson.PatientNormalizedFullNameIndexName
        };
}
