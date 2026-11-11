using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Tests;

public sealed class ProfessionalManagementTests
{
    [Fact]
    public async Task Inactivating_and_reactivating_professional_preserves_patient_associations()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var professional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909"
        };
        var association = new PatientProfessional
        {
            Patient = patient,
            Professional = professional
        };
        dbContext.PatientProfessionals.Add(association);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var inactivation = await service.SetActiveAsync(professional.Id, false);
        var associationCountAfterInactivation =
            await dbContext.PatientProfessionals.CountAsync();
        Assert.False(professional.IsActive);
        var reactivation = await service.SetActiveAsync(professional.Id, true);

        Assert.True(inactivation.Succeeded);
        Assert.Equal(1, associationCountAfterInactivation);
        Assert.True(reactivation.Succeeded);
        Assert.True(professional.IsActive);
        Assert.Single(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task GetProfessionals_returns_status_and_association_count()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var professional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909",
            IsActive = false
        };
        dbContext.PatientProfessionals.Add(new PatientProfessional
        {
            Patient = patient,
            Professional = professional
        });
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var professionals = await service.GetProfessionalsAsync();

        var item = Assert.Single(professionals);
        Assert.False(item.IsActive);
        Assert.Equal(1, item.AssociatedPatientCount);
    }

    [Fact]
    public async Task SetActive_returns_not_found_for_unknown_professional()
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.SetActiveAsync(42, false);

        Assert.Equal(
            Application.DTOs.ProfessionalManagementFailure.ProfessionalNotFound,
            result.Failure);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<Patient> AddPatientAsync(ApplicationDbContext dbContext)
    {
        var version = new Cid10CatalogVersion
        {
            Fingerprint = Guid.NewGuid().ToString("N"),
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

        var patient = new Patient
        {
            FullName = "Maria Silva",
            NormalizedFullName = "MARIA SILVA",
            DateOfBirth = new DateOnly(1990, 5, 12),
            Cid10CatalogEntryId = entry.Id,
            Cid10CatalogEntry = entry
        };
        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync();
        return patient;
    }
}
