using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Tests;

public sealed class ProfessionalRegistrationTests
{
    [Fact]
    public async Task RegisterAsync_persists_normalized_cpf_and_multiple_certifications_and_contacts()
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "  Ana Silva  ",
            "529.982.247-25",
            [
                new RegisterProfessionalCertificationRequest(
                    "Linguagem",
                    "CRFa1234",
                    new DateOnly(2024, 3, 10)),
                new RegisterProfessionalCertificationRequest(
                    "Voz",
                    "VOZ2",
                    new DateOnly(2023, 6, 20))
            ],
            [
                new RegisterProfessionalContactRequest(
                    ContactMethodType.Telephone,
                    " +55 (11) 91234-5678 "),
                new RegisterProfessionalContactRequest(
                    ContactMethodType.Email,
                    " ana@example.com ")
            ]));

        Assert.True(result.Succeeded);
        Assert.Equal("Ana Silva", result.Professional!.FullName);
        Assert.Equal("52998224725", result.Professional.Cpf);
        Assert.Equal(2, await dbContext.ProfessionalCertifications.CountAsync());
        Assert.Equal(2, await dbContext.ProfessionalContacts.CountAsync());
        Assert.Collection(
            await dbContext.ProfessionalContacts.OrderBy(contact => contact.Id).ToListAsync(),
            contact =>
            {
                Assert.Equal(ContactMethodType.Telephone, contact.Type);
                Assert.Equal("+55 (11) 91234-5678", contact.Value);
            },
            contact =>
            {
                Assert.Equal(ContactMethodType.Email, contact.Type);
                Assert.Equal("ana@example.com", contact.Value);
            });
    }

    [Fact]
    public async Task RegisterAsync_rejects_duplicate_cpf_after_normalizing_punctuation()
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var first = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            "52998224725",
            Contacts: ValidEmailContact()));
        var duplicate = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Souza",
            "529.982.247-25",
            Contacts: ValidEmailContact()));

        Assert.True(first.Succeeded);
        Assert.Equal(ProfessionalRegistrationFailure.DuplicateCpf, duplicate.Failure);
        Assert.Single(await dbContext.Professionals.ToListAsync());
    }

    [Theory]
    [InlineData("52998224724")]
    [InlineData("11111111111")]
    [InlineData("529 982 247 25")]
    [InlineData("")]
    public async Task RegisterAsync_rejects_invalid_cpf(string cpf)
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            cpf,
            Contacts: ValidEmailContact()));

        Assert.Equal(ProfessionalRegistrationFailure.InvalidCpf, result.Failure);
        Assert.Empty(await dbContext.Professionals.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_requires_at_least_one_contact()
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            "52998224725"));

        Assert.Equal(ProfessionalRegistrationFailure.MissingContact, result.Failure);
    }

    [Theory]
    [InlineData(ContactMethodType.Telephone, "123")]
    [InlineData(ContactMethodType.Email, "not-an-email")]
    public async Task RegisterAsync_rejects_invalid_contact_formats(
        ContactMethodType type,
        string value)
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            "52998224725",
            Contacts: [new RegisterProfessionalContactRequest(type, value)]));

        Assert.Equal(ProfessionalRegistrationFailure.InvalidContact, result.Failure);
        Assert.Empty(await dbContext.Professionals.ToListAsync());
    }

    [Theory]
    [InlineData("CRFA-1234", 2024, 3, 10)]
    public async Task RegisterAsync_rejects_invalid_certification_values(
        string registration,
        int year,
        int month,
        int day)
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            "52998224725",
            [
                new RegisterProfessionalCertificationRequest(
                    "Linguagem",
                    registration,
                    new DateOnly(year, month, day))
            ],
            ValidEmailContact()));

        Assert.Equal(ProfessionalRegistrationFailure.InvalidCertification, result.Failure);
        Assert.Empty(await dbContext.Professionals.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_rejects_future_certification_completion_date()
    {
        await using var dbContext = CreateDbContext();
        var service = new ProfessionalRegistrationService(dbContext);

        var result = await service.RegisterAsync(new RegisterProfessionalRequest(
            "Ana Silva",
            "52998224725",
            [
                new RegisterProfessionalCertificationRequest(
                    "Linguagem",
                    "CRFA1234",
                    DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            ],
            ValidEmailContact()));

        Assert.Equal(ProfessionalRegistrationFailure.InvalidCertification, result.Failure);
        Assert.Empty(await dbContext.Professionals.ToListAsync());
    }

    [Theory]
    [InlineData("52998224725", "52998224725")]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData(" 529.982.247-25 ", "52998224725")]
    [InlineData("52998224724", null)]
    public void NormalizeAndValidate_returns_digits_only_for_valid_cpf(
        string value,
        string? expected)
    {
        Assert.Equal(expected, BrazilianCpfValidator.NormalizeAndValidate(value));
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static IReadOnlyList<RegisterProfessionalContactRequest> ValidEmailContact() =>
    [
        new RegisterProfessionalContactRequest(
            ContactMethodType.Email,
            "ana@example.com")
    ];
}
