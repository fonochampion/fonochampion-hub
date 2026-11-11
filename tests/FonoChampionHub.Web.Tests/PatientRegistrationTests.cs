using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Tests;

public sealed class PatientRegistrationTests
{
    [Fact]
    public async Task RegisterAsync_persists_patient_with_selected_current_catalog_code()
    {
        await using var dbContext = CreateDbContext();
        var cidEntry = await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            "A00"));

        Assert.True(result.Succeeded);
        Assert.Equal("Maria Silva", result.Patient!.FullName);
        Assert.Equal(cidEntry.Id, result.Patient.Cid10CatalogEntryId);
        Assert.Equal("Maria Silva", (await dbContext.Patients.SingleAsync()).FullName);
    }

    [Fact]
    public async Task RegisterAsync_rejects_duplicate_normalized_name_and_birth_date()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);
        var dateOfBirth = new DateOnly(1990, 5, 12);

        var first = await service.RegisterAsync(
            new RegisterPatientRequest("Maria Silva", dateOfBirth, "A00"));
        var duplicate = await service.RegisterAsync(
            new RegisterPatientRequest("  maria   silva  ", dateOfBirth, "A00"));

        Assert.True(first.Succeeded);
        Assert.Equal(PatientRegistrationFailure.DuplicatePatient, duplicate.Failure);
        Assert.Single(await dbContext.Patients.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_rejects_future_birth_date_and_allows_older_birth_dates()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var futureResult = await service.RegisterAsync(new RegisterPatientRequest(
            "Pessoa Teste",
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            "A00"));
        var olderResult = await service.RegisterAsync(new RegisterPatientRequest(
            "Pessoa Antiga",
            new DateOnly(1900, 1, 1),
            "A00"));

        Assert.Equal(PatientRegistrationFailure.FutureDateOfBirth, futureResult.Failure);
        Assert.True(olderResult.Succeeded);
    }

    [Fact]
    public void NormalizeFullNameForComparison_collapses_whitespace_and_preserves_accents_and_punctuation()
    {
        var normalized = Patient.NormalizeFullNameForComparison("  Ána   D'Ávila ");

        Assert.Equal("ÁNA D'ÁVILA", normalized);
        Assert.NotEqual(normalized, Patient.NormalizeFullNameForComparison("Ana D'Ávila"));
        Assert.NotEqual(normalized, Patient.NormalizeFullNameForComparison("Ána Davila"));
    }

    [Theory]
    [InlineData(2025, 1, 15, 2025, 11, 14, "9 meses")]
    [InlineData(2025, 1, 15, 2025, 11, 15, "10 meses")]
    [InlineData(2024, 1, 15, 2025, 1, 15, "1 ano")]
    [InlineData(2000, 1, 15, 2025, 1, 14, "24 anos")]
    public void GetAgeDisplay_uses_completed_months_then_years(
        int birthYear,
        int birthMonth,
        int birthDay,
        int asOfYear,
        int asOfMonth,
        int asOfDay,
        string expected)
    {
        var patient = new Patient { DateOfBirth = new DateOnly(birthYear, birthMonth, birthDay) };

        Assert.Equal(expected, patient.GetAgeDisplay(new DateOnly(asOfYear, asOfMonth, asOfDay)));
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<Cid10CatalogEntry> AddCurrentCatalogEntryAsync(
        ApplicationDbContext dbContext)
    {
        var version = new Cid10CatalogVersion
        {
            Fingerprint = new string('a', 64),
            ImportedAtUtc = DateTimeOffset.UtcNow,
            IsCurrent = true
        };
        var entry = new Cid10CatalogEntry
        {
            VersionFingerprint = version.Fingerprint,
            Version = version,
            Level = Cid10CatalogEntryLevel.Category,
            Code = "A00",
            Description = "Cólera",
            SearchText = "COLERA"
        };
        version.Entries.Add(entry);
        dbContext.Cid10CatalogVersions.Add(version);
        await dbContext.SaveChangesAsync();
        return entry;
    }
}
