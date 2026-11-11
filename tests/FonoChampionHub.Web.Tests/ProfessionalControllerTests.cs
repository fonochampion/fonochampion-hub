using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Web.Controllers;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FonoChampionHub.Web.Tests;

public sealed class ProfessionalControllerTests
{
    [Fact]
    public async Task Create_trims_and_submits_valid_certifications_and_contacts()
    {
        var service = new CapturingProfessionalRegistrationService();
        var controller = CreateController(service);

        var result = await controller.Create(
            new CreateProfessionalViewModel
            {
                FullName = " Ana Silva ",
                Cpf = " 529.982.247-25 ",
                Certifications =
                [
                    new ProfessionalCertificationInputViewModel
                    {
                        Description = " Linguagem ",
                        Registration = " CRFA1234 ",
                        CompletionDate = new DateOnly(2024, 3, 10)
                    }
                ],
                Contacts =
                [
                    new ProfessionalContactInputViewModel
                    {
                        Type = ContactMethodType.Telephone,
                        Value = " +55 (11) 91234-5678 "
                    },
                    new ProfessionalContactInputViewModel
                    {
                        Type = ContactMethodType.Email,
                        Value = " ana@example.com "
                    }
                ]
            },
            CancellationToken.None);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Created", view.ViewName);
        Assert.Equal("Ana Silva", service.Request!.FullName);
        Assert.Equal("529.982.247-25", service.Request.Cpf);
        Assert.Equal("Linguagem", service.Request.Certifications![0].Description);
        Assert.Equal("CRFA1234", service.Request.Certifications[0].Registration);
        Assert.Equal("+55 (11) 91234-5678", service.Request.Contacts![0].Value);
        Assert.Equal("ana@example.com", service.Request.Contacts[1].Value);
    }

    [Fact]
    public async Task Create_requires_at_least_one_valid_contact()
    {
        var service = new CapturingProfessionalRegistrationService();
        var controller = CreateController(service);

        var result = await controller.Create(
            new CreateProfessionalViewModel
            {
                FullName = "Ana Silva",
                Cpf = "52998224725",
                Contacts = []
            },
            CancellationToken.None);

        Assert.IsType<ViewResult>(result);
        Assert.Null(service.Request);
        Assert.Contains(
            controller.ModelState[string.Empty]!.Errors,
            error => error.ErrorMessage.Contains("ao menos um telefone", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Inactivate_sets_professional_inactive_and_returns_to_list()
    {
        var managementService = new CapturingProfessionalManagementService();
        var controller = CreateController(
            new CapturingProfessionalRegistrationService(),
            managementService);

        var result = await controller.Inactivate(42, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(42, managementService.ProfessionalId);
        Assert.False(managementService.IsActive);
    }

    [Fact]
    public async Task Delete_marks_professional_deleted_and_returns_to_list()
    {
        var managementService = new CapturingProfessionalManagementService();
        var controller = CreateController(
            new CapturingProfessionalRegistrationService(),
            managementService);

        var result = await controller.Delete(42, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal(42, managementService.DeletedProfessionalId);
        Assert.Equal("Profissional excluído.", controller.TempData["ProfessionalNotice"]);
    }

    private static ProfessionalController CreateController(
        CapturingProfessionalRegistrationService service,
        CapturingProfessionalManagementService? managementService = null) =>
        new(service, managementService ?? new CapturingProfessionalManagementService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            },
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TestTempDataProvider())
        };

    private sealed class CapturingProfessionalRegistrationService
        : IProfessionalRegistrationService
    {
        public RegisterProfessionalRequest? Request { get; private set; }

        public Task<ProfessionalRegistrationResult> RegisterAsync(
            RegisterProfessionalRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;
            return Task.FromResult(ProfessionalRegistrationResult.Success(
                new Professional { FullName = request.FullName!.Trim() }));
        }
    }

    private sealed class CapturingProfessionalManagementService
        : IProfessionalManagementService
    {
        public int? ProfessionalId { get; private set; }

        public bool IsActive { get; private set; }

        public int? DeletedProfessionalId { get; private set; }

        public Task<IReadOnlyList<ProfessionalListItem>> GetProfessionalsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProfessionalListItem>>([]);

        public Task<ProfessionalManagementResult> SetActiveAsync(
            int professionalId,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            ProfessionalId = professionalId;
            IsActive = isActive;
            return Task.FromResult(ProfessionalManagementResult.Success());
        }

        public Task<ProfessionalManagementResult> DeleteAsync(
            int professionalId,
            CancellationToken cancellationToken = default)
        {
            DeletedProfessionalId = professionalId;
            return Task.FromResult(ProfessionalManagementResult.Success());
        }
    }

    private sealed class TestTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}
