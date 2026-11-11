using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;
using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonoChampionHub.Web.Controllers;

[Authorize(Roles = SystemRoles.Administrator)]
public sealed class ProfessionalController(
    IProfessionalRegistrationService professionalRegistrationService,
    IProfessionalManagementService professionalManagementService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var professionals = await professionalManagementService.GetProfessionalsAsync(
            cancellationToken);
        return View(professionals.Select(professional => new ProfessionalListItemViewModel
        {
            Id = professional.Id,
            FullName = professional.FullName,
            Cpf = professional.Cpf,
            IsActive = professional.IsActive,
            AssociatedPatientCount = professional.AssociatedPatientCount
        }).ToArray());
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateProfessionalViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Inactivate(int id, CancellationToken cancellationToken) =>
        SetActiveAsync(id, isActive: false, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Reactivate(int id, CancellationToken cancellationToken) =>
        SetActiveAsync(id, isActive: true, cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateProfessionalViewModel model,
        CancellationToken cancellationToken)
    {
        model.Certifications ??= [];
        model.Contacts ??= [];
        var fullName = model.FullName?.Trim() ?? string.Empty;
        var today = DateOnly.FromDateTime(DateTime.Today);

        var certificationRequests = new List<RegisterProfessionalCertificationRequest>();
        var certificationInputs = model.Certifications ?? [];
        for (var index = 0; index < certificationInputs.Count; index++)
        {
            var input = certificationInputs[index];
            var description = input.Description?.Trim() ?? string.Empty;
            var registration = input.Registration?.Trim() ?? string.Empty;
            var hasCompletionDate = input.CompletionDate is not null;
            if (description.Length == 0 && registration.Length == 0 && !hasCompletionDate)
            {
                continue;
            }

            if (description.Length == 0)
            {
                ModelState.AddModelError(
                    $"Certifications[{index}].Description",
                    "Informe a descrição da certificação.");
            }

            if (registration.Length == 0 || !registration.All(char.IsLetterOrDigit))
            {
                ModelState.AddModelError(
                    $"Certifications[{index}].Registration",
                    "Informe um registro alfanumérico.");
            }

            if (input.CompletionDate is not { } completionDate)
            {
                ModelState.AddModelError(
                    $"Certifications[{index}].CompletionDate",
                    "Informe a data de conclusão.");
            }
            else if (completionDate > today)
            {
                ModelState.AddModelError(
                    $"Certifications[{index}].CompletionDate",
                    "A data de conclusão não pode estar no futuro.");
            }

            if (description.Length > 0 &&
                registration.Length > 0 &&
                registration.All(char.IsLetterOrDigit) &&
                input.CompletionDate is { } validCompletionDate &&
                validCompletionDate <= today)
            {
                certificationRequests.Add(new RegisterProfessionalCertificationRequest(
                    description,
                    registration,
                    validCompletionDate));
            }
        }

        var contactRequests = new List<RegisterProfessionalContactRequest>();
        var contactInputs = model.Contacts ?? [];
        for (var index = 0; index < contactInputs.Count; index++)
        {
            var input = contactInputs[index];
            var value = input.Value?.Trim() ?? string.Empty;
            if (input.Type is null && value.Length == 0)
            {
                continue;
            }

            ContactMethodType? validType = null;
            if (input.Type is { } type && Enum.IsDefined(type))
            {
                validType = type;
            }
            else
            {
                ModelState.AddModelError(
                    $"Contacts[{index}].Type",
                    "Selecione o tipo de contato.");
            }

            if (value.Length == 0)
            {
                ModelState.AddModelError(
                    $"Contacts[{index}].Value",
                    "Informe o telefone ou e-mail.");
            }
            else if (validType is { } contactType)
            {
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
                    ModelState.AddModelError(
                        $"Contacts[{index}].Value",
                        "Informe um telefone brasileiro ou e-mail válido.");
                }
                else
                {
                    contactRequests.Add(new RegisterProfessionalContactRequest(
                        contactType,
                        value));
                }
            }
        }

        if (contactRequests.Count == 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "Informe ao menos um telefone ou e-mail válido para o profissional.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await professionalRegistrationService.RegisterAsync(
            new RegisterProfessionalRequest(
                fullName,
                model.Cpf?.Trim(),
                certificationRequests,
                contactRequests),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddRegistrationError(result.Failure);
            return View(model);
        }

        var professional = result.Professional
            ?? throw new InvalidOperationException(
                "Successful professional registration returned no professional.");

        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";
        return View("Created", new CreatedProfessionalViewModel
        {
            FullName = professional.FullName
        });
    }

    private void AddRegistrationError(ProfessionalRegistrationFailure? failure)
    {
        var (key, message) = failure switch
        {
            ProfessionalRegistrationFailure.InvalidFullName =>
                (nameof(CreateProfessionalViewModel.FullName), "Informe um nome completo válido."),
            ProfessionalRegistrationFailure.InvalidCpf =>
                (nameof(CreateProfessionalViewModel.Cpf), "Informe um CPF válido."),
            ProfessionalRegistrationFailure.DuplicateCpf =>
                (nameof(CreateProfessionalViewModel.Cpf),
                    "Já existe um profissional cadastrado com esse CPF."),
            ProfessionalRegistrationFailure.InvalidCertification =>
                (string.Empty, "Confira os dados das certificações informadas."),
            ProfessionalRegistrationFailure.MissingContact =>
                (string.Empty, "Informe ao menos um telefone ou e-mail para o profissional."),
            ProfessionalRegistrationFailure.InvalidContact =>
                (string.Empty, "Informe um telefone brasileiro ou e-mail válido."),
            _ => throw new InvalidOperationException(
                $"Unsupported professional registration failure: {failure}.")
        };

        ModelState.AddModelError(key, message);
    }

    private async Task<IActionResult> SetActiveAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var result = await professionalManagementService.SetActiveAsync(
            id,
            isActive,
            cancellationToken);
        if (result.Failure is { } failure)
        {
            return failure switch
            {
                ProfessionalManagementFailure.ProfessionalNotFound => NotFound(),
                _ => throw new InvalidOperationException(
                    $"Unsupported professional management failure: {failure}.")
            };
        }

        TempData["ProfessionalNotice"] = isActive
            ? "Profissional reativado."
            : "Profissional inativado.";
        return RedirectToAction(nameof(Index));
    }
}
