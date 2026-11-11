using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Tests;

public sealed class PatientProfessionalAssociationTests
{
    [Fact]
    public async Task Scheduling_options_are_all_professionals_when_patient_has_no_associations()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var first = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var second = await AddProfessionalAsync(dbContext, "Bruno Terapeuta");
        var service = new PatientProfessionalAssociationService(dbContext);

        var options = await service.GetSchedulingOptionsAsync(patient.Id);

        Assert.Collection(
            options!,
            professional => Assert.Equal(first.Id, professional.Id),
            professional => Assert.Equal(second.Id, professional.Id));
    }

    [Fact]
    public async Task Registration_options_exclude_inactive_professionals()
    {
        await using var dbContext = CreateDbContext();
        var active = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var inactive = await AddProfessionalAsync(dbContext, "Bruno Terapeuta");
        inactive.IsActive = false;
        await dbContext.SaveChangesAsync();
        var service = new PatientProfessionalAssociationService(dbContext);

        var options = await service.GetRegistrationOptionsAsync();

        var option = Assert.Single(options);
        Assert.Equal(active.Id, option.Id);
    }

    [Fact]
    public async Task Scheduling_associates_selected_professionals_when_none_are_associated()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var first = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        await AddProfessionalAsync(dbContext, "Bruno Terapeuta");
        var service = new PatientProfessionalAssociationService(dbContext);

        var result = await service.AssociateForSchedulingAsync(patient.Id, [first.Id]);

        Assert.True(result.Succeeded);
        var association = await dbContext.PatientProfessionals.SingleAsync();
        Assert.Equal(patient.Id, association.PatientId);
        Assert.Equal(first.Id, association.ProfessionalId);
    }

    [Fact]
    public async Task Scheduling_rejects_unassociated_professional_when_associations_exist()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var associated = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var other = await AddProfessionalAsync(dbContext, "Bruno Terapeuta");
        dbContext.PatientProfessionals.Add(new PatientProfessional
        {
            PatientId = patient.Id,
            ProfessionalId = associated.Id
        });
        await dbContext.SaveChangesAsync();
        var service = new PatientProfessionalAssociationService(dbContext);

        var result = await service.AssociateForSchedulingAsync(patient.Id, [other.Id]);

        Assert.Equal(
            Application.DTOs.PatientProfessionalAssociationFailure.InvalidProfessional,
            result.Failure);
        Assert.Single(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task Scheduling_uses_active_associations_and_falls_back_when_all_are_inactive()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var inactive = await AddProfessionalAsync(dbContext, "Ana Inativa");
        inactive.IsActive = false;
        var active = await AddProfessionalAsync(dbContext, "Bruno Ativo");
        dbContext.PatientProfessionals.Add(new PatientProfessional
        {
            PatientId = patient.Id,
            ProfessionalId = inactive.Id
        });
        await dbContext.SaveChangesAsync();
        var service = new PatientProfessionalAssociationService(dbContext);

        var options = await service.GetSchedulingOptionsAsync(patient.Id);
        var result = await service.AssociateForSchedulingAsync(patient.Id, [active.Id]);

        var option = Assert.Single(options!);
        Assert.Equal(active.Id, option.Id);
        Assert.True(result.Succeeded);
        Assert.Equal(2, await dbContext.PatientProfessionals.CountAsync());
    }

    [Fact]
    public async Task Patient_management_keeps_inactive_association_but_does_not_offer_inactive_professionals()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var inactiveAssociated = await AddProfessionalAsync(dbContext, "Ana Inativa");
        inactiveAssociated.IsActive = false;
        var inactiveAvailable = await AddProfessionalAsync(dbContext, "Bruno Inativo");
        inactiveAvailable.IsActive = false;
        var activeAvailable = await AddProfessionalAsync(dbContext, "Carla Ativa");
        dbContext.PatientProfessionals.Add(new PatientProfessional
        {
            PatientId = patient.Id,
            ProfessionalId = inactiveAssociated.Id
        });
        await dbContext.SaveChangesAsync();
        var service = new PatientProfessionalAssociationService(dbContext);

        var management = await service.GetManagementAsync(patient.Id);

        var association = Assert.Single(management!.AssociatedProfessionals);
        Assert.Equal(inactiveAssociated.Id, association.Id);
        Assert.False(association.IsActive);
        var available = Assert.Single(management.AvailableProfessionals);
        Assert.Equal(activeAvailable.Id, available.Id);
    }

    [Fact]
    public async Task Patient_management_rejects_adding_an_inactive_professional()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var professional = await AddProfessionalAsync(dbContext, "Ana Inativa");
        professional.IsActive = false;
        await dbContext.SaveChangesAsync();
        var service = new PatientProfessionalAssociationService(dbContext);

        var result = await service.AddToPatientAsync(patient.Id, [professional.Id]);

        Assert.Equal(
            FonoChampionHub.Application.DTOs.PatientProfessionalAssociationFailure.InvalidProfessional,
            result.Failure);
        Assert.Empty(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task Adding_professionals_is_idempotent_and_management_lists_available_records()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var first = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var second = await AddProfessionalAsync(dbContext, "Bruno Terapeuta");
        var service = new PatientProfessionalAssociationService(dbContext);

        var firstResult = await service.AddToPatientAsync(patient.Id, [first.Id, first.Id]);
        var secondResult = await service.AddToPatientAsync(patient.Id, [first.Id]);
        var management = await service.GetManagementAsync(patient.Id);

        Assert.True(firstResult.Succeeded);
        Assert.True(secondResult.Succeeded);
        Assert.Single(await dbContext.PatientProfessionals.ToListAsync());
        Assert.Equal(patient.FullName, management!.PatientFullName);
        Assert.Single(management.AssociatedProfessionals);
        Assert.Equal(first.Id, management.AssociatedProfessionals[0].Id);
        Assert.Collection(
            management.AvailableProfessionals,
            professional => Assert.Equal(second.Id, professional.Id));
    }

    [Fact]
    public async Task One_professional_can_be_associated_with_multiple_patients()
    {
        await using var dbContext = CreateDbContext();
        var firstPatient = await AddPatientAsync(dbContext);
        var secondPatient = await AddPatientAsync(
            dbContext,
            "João Silva",
            new DateOnly(1988, 2, 20));
        var professional = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var service = new PatientProfessionalAssociationService(dbContext);

        var firstResult = await service.AddToPatientAsync(firstPatient.Id, [professional.Id]);
        var secondResult = await service.AddToPatientAsync(secondPatient.Id, [professional.Id]);

        Assert.True(firstResult.Succeeded);
        Assert.True(secondResult.Succeeded);
        Assert.Equal(
            [firstPatient.Id, secondPatient.Id],
            await dbContext.PatientProfessionals
                .OrderBy(association => association.PatientId)
                .Select(association => association.PatientId)
                .ToArrayAsync());
    }

    [Fact]
    public async Task Adding_professionals_rejects_invalid_selection_without_partial_associations()
    {
        await using var dbContext = CreateDbContext();
        var patient = await AddPatientAsync(dbContext);
        var professional = await AddProfessionalAsync(dbContext, "Ana Terapeuta");
        var service = new PatientProfessionalAssociationService(dbContext);

        var result = await service.AddToPatientAsync(patient.Id, [professional.Id, 999]);

        Assert.Equal(
            FonoChampionHub.Application.DTOs.PatientProfessionalAssociationFailure.InvalidProfessional,
            result.Failure);
        Assert.Empty(await dbContext.PatientProfessionals.ToListAsync());
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<Patient> AddPatientAsync(
        ApplicationDbContext dbContext,
        string fullName = "Maria Silva",
        DateOnly? dateOfBirth = null)
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
            FullName = fullName,
            NormalizedFullName = fullName.ToUpperInvariant(),
            DateOfBirth = dateOfBirth ?? new DateOnly(1990, 5, 12),
            Cid10CatalogEntryId = entry.Id
        };
        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync();
        return patient;
    }

    private static async Task<Professional> AddProfessionalAsync(
        ApplicationDbContext dbContext,
        string fullName)
    {
        var professional = new Professional
        {
            FullName = fullName,
            Cpf = Guid.NewGuid().ToString("N")[..11]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        return professional;
    }
}
