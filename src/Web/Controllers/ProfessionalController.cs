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
            IsDeleted = professional.IsDeleted,
            AssociatedPatientCount = professional.AssociatedPatientCount
        }).ToArray());
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateProfessionalViewModel());

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var professional = await professionalManagementService.GetProfessionalForEditAsync(
            id,
            cancellationToken);
        if (professional is null)
        {
            return NotFound();
        }

        return View(new EditProfessionalViewModel
        {
            Id = professional.Id,
            FullName = professional.FullName,
            Cpf = professional.Cpf,
            Certifications = professional.Certifications.Select(certification =>
                new ProfessionalCertificationInputViewModel
                {
                    Id = certification.Id,
                    Description = certification.Description,
                    Registration = certification.Registration,
                    CompletionDate = certification.CompletionDate
                }).ToList(),
            Contacts = professional.Contacts.Select(contact =>
                new ProfessionalContactInputViewModel
                {
                    Id = contact.Id,
                    Type = contact.Type,
                    Value = contact.Value
                }).ToList()
        });
    }

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
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await professionalManagementService.DeleteAsync(id, cancellationToken);
        if (result.Failure is { } failure)
        {
            if (failure == ProfessionalManagementFailure.ProfessionalNotFound)
            {
                return NotFound();
            }

            TempData["ProfessionalError"] = failure switch
            {
                ProfessionalManagementFailure.ProfessionalAlreadyDeleted =>
                    "O profissional já está excluído.",
                ProfessionalManagementFailure.ProfessionalHasAssociatedPatients =>
                    "O profissional possui pacientes associados e não pode ser excluído.",
                _ => throw new InvalidOperationException(
                    $"Unsupported professional management failure: {failure}.")
            };
            return RedirectToAction(nameof(Index));
        }

        TempData["ProfessionalNotice"] = "Profissional excluído.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EditProfessionalViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        if (!ValidateProfessionalForm(model))
        {
            return View(model);
        }

        var result = await professionalManagementService.UpdateAsync(
            id,
            new UpdateProfessionalRequest(
                model.FullName?.Trim(),
                model.Cpf?.Trim(),
                model.Certifications
                    .Where(HasCertificationInput)
                    .Select(certification => new UpdateProfessionalCertificationRequest(
                        certification.Id,
                        certification.Description?.Trim(),
                        certification.Registration?.Trim(),
                        certification.CompletionDate))
                    .ToArray(),
                model.Contacts
                    .Where(HasContactInput)
                    .Select(contact => new UpdateProfessionalContactRequest(
                        contact.Id,
                        contact.Type,
                        contact.Value?.Trim()))
                    .ToArray()),
            cancellationToken);

        if (result.Failure is { } failure)
        {
            if (failure is ProfessionalManagementFailure.ProfessionalNotFound or
                ProfessionalManagementFailure.ProfessionalAlreadyDeleted)
            {
                return NotFound();
            }

            AddProfessionalManagementError(failure);
            return View(model);
        }

        TempData["ProfessionalNotice"] = "Profissional atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateProfessionalViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ValidateProfessionalForm(model))
        {
            return View(model);
        }

        var certificationRequests = model.Certifications
            .Where(HasCertificationInput)
            .Select(certification => new RegisterProfessionalCertificationRequest(
                certification.Description?.Trim(),
                certification.Registration?.Trim(),
                certification.CompletionDate))
            .ToArray();
        var contactRequests = model.Contacts
            .Where(HasContactInput)
            .Select(contact => new RegisterProfessionalContactRequest(
                contact.Type,
                contact.Value?.Trim()))
            .ToArray();
        var result = await professionalRegistrationService.RegisterAsync(
            new RegisterProfessionalRequest(
                model.FullName!.Trim(),
                model.Cpf!.Trim(),
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

    private bool ValidateProfessionalForm(ProfessionalFormViewModel model)
    {
        model.Certifications ??= [];
        model.Contacts ??= [];

        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            ModelState.AddModelError(
                nameof(model.FullName),
                "Informe o nome completo.");
        }

        if (string.IsNullOrWhiteSpace(model.Cpf) ||
            BrazilianCpfValidator.NormalizeAndValidate(model.Cpf) is null)
        {
            ModelState.AddModelError(
                nameof(model.Cpf),
                "Informe um CPF válido.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        for (var index = 0; index < model.Certifications.Count; index++)
        {
            var input = model.Certifications[index];
            var description = input.Description?.Trim() ?? string.Empty;
            var registration = input.Registration?.Trim() ?? string.Empty;
            if (input.Id is null &&
                description.Length == 0 &&
                registration.Length == 0 &&
                input.CompletionDate is null)
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
        }

        var validContactCount = 0;
        for (var index = 0; index < model.Contacts.Count; index++)
        {
            var input = model.Contacts[index];
            var value = input.Value?.Trim() ?? string.Empty;
            if (input.Id is null && input.Type is null && value.Length == 0)
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
                    validContactCount++;
                }
            }
        }

        if (validContactCount == 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "Informe ao menos um telefone ou e-mail válido para o profissional.");
        }

        return ModelState.IsValid;
    }

    private static bool HasCertificationInput(ProfessionalCertificationInputViewModel input) =>
        input.Id is not null ||
        !string.IsNullOrWhiteSpace(input.Description) ||
        !string.IsNullOrWhiteSpace(input.Registration) ||
        input.CompletionDate is not null;

    private static bool HasContactInput(ProfessionalContactInputViewModel input) =>
        input.Id is not null ||
        input.Type is not null ||
        !string.IsNullOrWhiteSpace(input.Value);

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

    private void AddProfessionalManagementError(ProfessionalManagementFailure failure)
    {
        var (key, message) = failure switch
        {
            ProfessionalManagementFailure.InvalidFullName =>
                (nameof(EditProfessionalViewModel.FullName), "Informe um nome completo válido."),
            ProfessionalManagementFailure.InvalidCpf =>
                (nameof(EditProfessionalViewModel.Cpf), "Informe um CPF válido."),
            ProfessionalManagementFailure.DuplicateCpf =>
                (nameof(EditProfessionalViewModel.Cpf),
                    "Já existe um profissional cadastrado com esse CPF."),
            ProfessionalManagementFailure.InvalidCertification =>
                (string.Empty, "Confira os dados das certificações informadas."),
            ProfessionalManagementFailure.MissingContact =>
                (string.Empty, "Informe ao menos um telefone ou e-mail para o profissional."),
            ProfessionalManagementFailure.InvalidContact =>
                (string.Empty, "Informe um telefone brasileiro ou e-mail válido."),
            _ => throw new InvalidOperationException(
                $"Unsupported professional management failure: {failure}.")
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
            if (failure == ProfessionalManagementFailure.ProfessionalAlreadyDeleted)
            {
                TempData["ProfessionalError"] = "Um profissional excluído não pode ser reativado.";
                return RedirectToAction(nameof(Index));
            }

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
