using FonoChampionHub.Application.DTOs;
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
        Assert.False(item.IsDeleted);
    }

    [Fact]
    public async Task Delete_marks_unassociated_professional_terminally_deleted_and_preserves_details()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909",
            Certifications =
            [
                new ProfessionalCertification
                {
                    Description = "Linguagem",
                    Registration = "CRFA1234",
                    CompletionDate = new DateOnly(2024, 3, 10)
                }
            ],
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.DeleteAsync(professional.Id);
        var persistedProfessional = await dbContext.Professionals
            .Include(item => item.Certifications)
            .Include(item => item.Contacts)
            .SingleAsync();

        Assert.True(result.Succeeded);
        Assert.True(persistedProfessional.IsDeleted);
        Assert.False(persistedProfessional.IsActive);
        Assert.Single(persistedProfessional.Certifications);
        Assert.Single(persistedProfessional.Contacts);
        Assert.Empty(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task Delete_rejects_professional_with_associated_patient()
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

        var result = await service.DeleteAsync(professional.Id);

        Assert.Equal(
            Application.DTOs.ProfessionalManagementFailure.ProfessionalHasAssociatedPatients,
            result.Failure);
        Assert.False(professional.IsDeleted);
        Assert.Single(await dbContext.PatientProfessionals.ToListAsync());
    }

    [Fact]
    public async Task SetActive_rejects_deleted_professional()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Terapeuta",
            Cpf = "12345678909",
            IsActive = false,
            IsDeleted = true
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.SetActiveAsync(professional.Id, true);

        Assert.Equal(
            Application.DTOs.ProfessionalManagementFailure.ProfessionalAlreadyDeleted,
            result.Failure);
        Assert.False(professional.IsActive);
        Assert.True(Assert.Single(await service.GetProfessionalsAsync()).IsDeleted);
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

    [Fact]
    public async Task Update_maintains_certifications_and_contacts_individually()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Certifications =
            [
                new ProfessionalCertification
                {
                    Description = "Linguagem",
                    Registration = "CRFA1234",
                    CompletionDate = new DateOnly(2024, 3, 10)
                },
                new ProfessionalCertification
                {
                    Description = "Voz",
                    Registration = "VOZ2",
                    CompletionDate = new DateOnly(2023, 6, 20)
                }
            ],
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                },
                new ProfessionalContact
                {
                    Type = ContactMethodType.Telephone,
                    Value = "+55 (11) 91234-5678"
                }
            ]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var firstCertification = professional.Certifications.First();
        var removedCertificationId = professional.Certifications.Last().Id;
        var emailContact = professional.Contacts.First();
        var removedContactId = professional.Contacts.Last().Id;
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.UpdateAsync(
            professional.Id,
            new UpdateProfessionalRequest(
                "  Ana Maria Silva  ",
                "529.982.247-25",
                [
                    new UpdateProfessionalCertificationRequest(
                        firstCertification.Id,
                        "Linguagem e fala",
                        "CRFA5678",
                        new DateOnly(2025, 2, 1)),
                    new UpdateProfessionalCertificationRequest(
                        null,
                        "Voz",
                        "VOZ3",
                        new DateOnly(2025, 3, 1))
                ],
                [
                    new UpdateProfessionalContactRequest(
                        emailContact.Id,
                        ContactMethodType.Email,
                        " novo@example.com "),
                    new UpdateProfessionalContactRequest(
                        null,
                        ContactMethodType.Telephone,
                        "+55 (21) 91234-5678")
                ]));

        var persisted = await dbContext.Professionals
            .Include(item => item.Certifications)
            .Include(item => item.Contacts)
            .SingleAsync();
        Assert.True(result.Succeeded);
        Assert.Equal("Ana Maria Silva", persisted.FullName);
        Assert.Equal("52998224725", persisted.Cpf);
        Assert.Equal(2, persisted.Certifications.Count);
        var updatedCertification = Assert.Single(
            persisted.Certifications,
            item => item.Id == firstCertification.Id);
        Assert.Equal("Linguagem e fala", updatedCertification.Description);
        Assert.DoesNotContain(
            persisted.Certifications,
            item => item.Id == removedCertificationId);
        Assert.Equal(2, persisted.Contacts.Count);
        var updatedEmail = Assert.Single(
            persisted.Contacts,
            item => item.Id == emailContact.Id);
        Assert.Equal("novo@example.com", updatedEmail.Value);
        Assert.DoesNotContain(persisted.Contacts, item => item.Id == removedContactId);
        Assert.Contains(
            persisted.Contacts,
            item => item.Type == ContactMethodType.Telephone &&
                    item.Value == "+55 (21) 91234-5678");
    }

    [Fact]
    public async Task Update_rejects_removing_the_last_contact_without_changing_the_record()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.UpdateAsync(
            professional.Id,
            new UpdateProfessionalRequest(
                "Nome alterado",
                "52998224725",
                [],
                []));

        Assert.Equal(ProfessionalManagementFailure.MissingContact, result.Failure);
        Assert.Equal("Ana Silva", professional.FullName);
        Assert.Single(await dbContext.ProfessionalContacts.ToListAsync());
    }

    [Fact]
    public async Task Update_rejects_contact_ids_owned_by_another_professional()
    {
        await using var dbContext = CreateDbContext();
        var first = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        var second = new Professional
        {
            FullName = "Maria Souza",
            Cpf = "11144477735",
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "maria@example.com"
                }
            ]
        };
        dbContext.Professionals.AddRange(first, second);
        await dbContext.SaveChangesAsync();
        var otherContactId = second.Contacts.Single().Id;
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.UpdateAsync(
            first.Id,
            new UpdateProfessionalRequest(
                "Ana Silva",
                first.Cpf,
                [],
                [
                    new UpdateProfessionalContactRequest(
                        otherContactId,
                        ContactMethodType.Email,
                        "intruder@example.com")
                ]));

        Assert.Equal(ProfessionalManagementFailure.InvalidContact, result.Failure);
        Assert.Equal("ana@example.com", first.Contacts.Single().Value);
        Assert.Equal("maria@example.com", second.Contacts.Single().Value);
    }

    [Fact]
    public async Task GetProfessionalForEdit_returns_current_details_and_hides_deleted_professionals()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Certifications =
            [
                new ProfessionalCertification
                {
                    Description = "Linguagem",
                    Registration = "CRFA1234",
                    CompletionDate = new DateOnly(2024, 3, 10)
                }
            ],
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var details = await service.GetProfessionalForEditAsync(professional.Id);
        professional.IsDeleted = true;
        await dbContext.SaveChangesAsync();
        var deletedDetails = await service.GetProfessionalForEditAsync(professional.Id);

        Assert.NotNull(details);
        Assert.Equal("Ana Silva", details.FullName);
        Assert.Equal("52998224725", details.Cpf);
        Assert.Equal("Linguagem", Assert.Single(details.Certifications).Description);
        Assert.Equal("ana@example.com", Assert.Single(details.Contacts).Value);
        Assert.Null(deletedDetails);
    }

    [Fact]
    public async Task Update_rejects_a_cpf_owned_by_another_professional()
    {
        await using var dbContext = CreateDbContext();
        var first = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        var second = new Professional
        {
            FullName = "Maria Souza",
            Cpf = "11144477735",
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "maria@example.com"
                }
            ]
        };
        dbContext.Professionals.AddRange(first, second);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.UpdateAsync(
            first.Id,
            new UpdateProfessionalRequest(
                "Nome alterado",
                "111.444.777-35",
                [],
                [
                    new UpdateProfessionalContactRequest(
                        first.Contacts.Single().Id,
                        ContactMethodType.Email,
                        "ana@example.com")
                ]));

        Assert.Equal(ProfessionalManagementFailure.DuplicateCpf, result.Failure);
        Assert.Equal("Ana Silva", first.FullName);
        Assert.Equal("52998224725", first.Cpf);
    }

    [Fact]
    public async Task Update_allows_removing_all_certifications()
    {
        await using var dbContext = CreateDbContext();
        var professional = new Professional
        {
            FullName = "Ana Silva",
            Cpf = "52998224725",
            Certifications =
            [
                new ProfessionalCertification
                {
                    Description = "Linguagem",
                    Registration = "CRFA1234",
                    CompletionDate = new DateOnly(2024, 3, 10)
                }
            ],
            Contacts =
            [
                new ProfessionalContact
                {
                    Type = ContactMethodType.Email,
                    Value = "ana@example.com"
                }
            ]
        };
        dbContext.Professionals.Add(professional);
        await dbContext.SaveChangesAsync();
        var service = new ProfessionalManagementService(dbContext);

        var result = await service.UpdateAsync(
            professional.Id,
            new UpdateProfessionalRequest(
                "Ana Silva",
                "52998224725",
                [],
                [
                    new UpdateProfessionalContactRequest(
                        professional.Contacts.Single().Id,
                        ContactMethodType.Email,
                        "ana@example.com")
                ]));

        Assert.True(result.Succeeded);
        Assert.Empty(await dbContext.ProfessionalCertifications.ToListAsync());
        Assert.Single(await dbContext.ProfessionalContacts.ToListAsync());
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
