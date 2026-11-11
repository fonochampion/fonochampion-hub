using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
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
            Cid10CatalogEntry = cidEntry
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

        return PatientRegistrationResult.Success(patient);
    }

    private static bool IsDuplicatePatientConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: Patient.FullNameDateOfBirthIndexName
        };
}
