using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
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
    public async Task RegisterAsync_associates_selected_professionals_with_patient()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var firstProfessional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909"
        };
        var secondProfessional = new Professional
        {
            FullName = "Bruno Terapeuta",
            Cpf = "98765432100"
        };
        dbContext.Professionals.AddRange(firstProfessional, secondProfessional);
        await dbContext.SaveChangesAsync();
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            ProfessionalIds: [firstProfessional.Id, secondProfessional.Id]));

        Assert.True(result.Succeeded);
        var associations = await dbContext.PatientProfessionals
            .OrderBy(association => association.ProfessionalId)
            .ToListAsync();
        Assert.Collection(
            associations,
            association =>
            {
                Assert.Equal(result.Patient!.Id, association.PatientId);
                Assert.Equal(firstProfessional.Id, association.ProfessionalId);
            },
            association =>
            {
                Assert.Equal(result.Patient!.Id, association.PatientId);
                Assert.Equal(secondProfessional.Id, association.ProfessionalId);
            });
    }

    [Fact]
    public async Task RegisterAsync_rejects_unknown_professional_ids_without_creating_patient()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            ProfessionalIds: [42]));

        Assert.Equal(PatientRegistrationFailure.InvalidProfessional, result.Failure);
        Assert.Empty(await dbContext.Patients.ToListAsync());
        Assert.Empty(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_rejects_inactive_professional_ids()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var professional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909",
            IsActive = false
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            ProfessionalIds: [professional.Id]));

        Assert.Equal(PatientRegistrationFailure.InvalidProfessional, result.Failure);
        Assert.Empty(await dbContext.Patients.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_persists_multiple_responsible_persons_with_the_patient()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            [
                new RegisterResponsiblePersonRequest(
                    "Ana Silva",
                    ResponsiblePersonKinship.Mother,
                    [
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Telephone,
                            " (11) 91234-5678 "),
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Telephone,
                            "+55 11 3333-4444"),
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Email,
                            " ana@example.com ")
                    ]),
                new RegisterResponsiblePersonRequest(
                    "João Silva",
                    ResponsiblePersonKinship.Grandparent,
                    [
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Email,
                            "joao@example.com")
                    ])
            ]));

        Assert.True(result.Succeeded);
        var responsiblePersons = await dbContext.ResponsiblePersons
            .Include(person => person.Contacts)
            .OrderBy(person => person.Id)
            .ToListAsync();
        Assert.Collection(
            responsiblePersons,
            person =>
            {
                Assert.Equal("Ana Silva", person.FullName);
                Assert.Equal("ANA SILVA", person.NormalizedFullName);
                Assert.Equal(ResponsiblePersonKinship.Mother, person.Kinship);
                Assert.Equal(result.Patient!.Id, person.PatientId);
                Assert.Equal(3, person.Contacts.Count);
            },
            person =>
            {
                Assert.Equal("João Silva", person.FullName);
                Assert.Equal(ResponsiblePersonKinship.Grandparent, person.Kinship);
                Assert.Equal(result.Patient!.Id, person.PatientId);
                Assert.Single(person.Contacts);
            });
        var contacts = await dbContext.ResponsiblePersonContacts
            .OrderBy(contact => contact.Id)
            .ToListAsync();
        Assert.Collection(
            contacts,
            contact =>
            {
                Assert.Equal(ContactMethodType.Telephone, contact.Type);
                Assert.Equal("(11) 91234-5678", contact.Value);
            },
            contact =>
            {
                Assert.Equal(ContactMethodType.Telephone, contact.Type);
                Assert.Equal("+55 11 3333-4444", contact.Value);
            },
            contact =>
            {
                Assert.Equal(ContactMethodType.Email, contact.Type);
                Assert.Equal("ana@example.com", contact.Value);
            },
            contact =>
            {
                Assert.Equal(ContactMethodType.Email, contact.Type);
                Assert.Equal("joao@example.com", contact.Value);
            });
    }

    [Fact]
    public async Task RegisterAsync_rejects_duplicate_responsible_names_after_normalization()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            [
                new RegisterResponsiblePersonRequest(
                    "Ána Silva",
                    ResponsiblePersonKinship.Mother,
                    [
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Telephone,
                            "11912345678")
                    ]),
                new RegisterResponsiblePersonRequest(
                    "  ána   silva ",
                    ResponsiblePersonKinship.LegalGuardian,
                    [
                        new RegisterResponsiblePersonContactRequest(
                            ContactMethodType.Email,
                            "ana@example.com")
                    ])
            ]));

        Assert.Equal(PatientRegistrationFailure.DuplicateResponsiblePerson, result.Failure);
        Assert.Empty(await dbContext.Patients.ToListAsync());
        Assert.Empty(await dbContext.ResponsiblePersons.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_requires_at_least_one_contact_for_each_responsible_person()
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            [
                new RegisterResponsiblePersonRequest(
                    "Ana Silva",
                    ResponsiblePersonKinship.Mother)
            ]));

        Assert.Equal(PatientRegistrationFailure.MissingResponsiblePersonContact, result.Failure);
        Assert.Empty(await dbContext.Patients.ToListAsync());
    }

    [Theory]
    [InlineData(ContactMethodType.Telephone, "123")]
    [InlineData(ContactMethodType.Telephone, "+1 11 91234-5678")]
    [InlineData(ContactMethodType.Email, "not-an-email")]
    public async Task RegisterAsync_rejects_invalid_contact_formats(
        ContactMethodType type,
        string value)
    {
        await using var dbContext = CreateDbContext();
        await AddCurrentCatalogEntryAsync(dbContext);
        var service = new PatientRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterPatientRequest(
            "Maria Silva",
            new DateOnly(1990, 5, 12),
            "A00",
            [
                new RegisterResponsiblePersonRequest(
                    "Ana Silva",
                    ResponsiblePersonKinship.Mother,
                    [new RegisterResponsiblePersonContactRequest(type, value)])
            ]));

        Assert.Equal(PatientRegistrationFailure.InvalidResponsiblePersonContact, result.Failure);
        Assert.Empty(await dbContext.Patients.ToListAsync());
    }

    [Theory]
    [InlineData("11 1234-5678", true)]
    [InlineData("(11) 91234-5678", true)]
    [InlineData("+55 (11) 91234-5678", true)]
    [InlineData("+1 (11) 91234-5678", false)]
    [InlineData("(11) 123-4567", false)]
    [InlineData("11 ABCD-5678", false)]
    public void IsValidBrazilianTelephone_checks_digit_count_and_allowed_separators(
        string value,
        bool expected)
    {
        Assert.Equal(expected, ContactValueValidator.IsValidBrazilianTelephone(value));
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
