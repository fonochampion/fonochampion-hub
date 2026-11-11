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

[Authorize(Roles = SystemRoles.Administrator + "," + SystemRoles.Attendant)]
public sealed class PatientController(
    ICid10CatalogQueryService cid10CatalogQueryService,
    IPatientRegistrationService patientRegistrationService,
    IPatientProfessionalAssociationService patientProfessionalAssociationService)
    : Controller
{
    private const int MinimumCidQueryLength = 2;
    private const int MaximumCidResults = 25;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var patients = await patientProfessionalAssociationService.GetPatientsAsync(cancellationToken);
        return View(patients.Select(patient => new PatientListItemViewModel
        {
            Id = patient.Id,
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            Cid10Code = patient.Cid10Code,
            Cid10Description = patient.Cid10Description,
            AssociatedProfessionalCount = patient.AssociatedProfessionalCount
        }).ToArray());
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CreatePatientViewModel();
        await PopulateRegistrationOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> SearchCid(
        [FromQuery] string? query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < MinimumCidQueryLength)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = $"Digite pelo menos {MinimumCidQueryLength} caracteres para buscar."
            });
        }

        if (!await cid10CatalogQueryService.HasCurrentCatalogAsync(cancellationToken))
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                detail: "O catálogo CID-10 ainda não foi carregado neste ambiente.");
        }

        var matches = await cid10CatalogQueryService.SearchAsync(query.Trim(), cancellationToken);
        var options = matches
            .Where(match => match.Code is not null)
            .Select(match => new Cid10Option(
                match.Code!,
                match.Description,
                string.Join(" / ", match.Ancestors.Select(ancestor => ancestor.Description))))
            .ToArray();

        return Json(new
        {
            items = options.Take(MaximumCidResults),
            hasMore = options.Length > MaximumCidResults
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreatePatientViewModel model,
        CancellationToken cancellationToken)
    {
        var responsiblePersonRequests = new List<RegisterResponsiblePersonRequest>();
        var responsiblePersonInputs = model.ResponsiblePersons ?? [];
        for (var index = 0; index < responsiblePersonInputs.Count; index++)
        {
            var input = responsiblePersonInputs[index];
            var fullName = input.FullName?.Trim() ?? string.Empty;
            var contactRequests = new List<RegisterResponsiblePersonContactRequest>();
            var contactInputs = input.Contacts ?? [];
            var hasContactInput = false;
            for (var contactIndex = 0; contactIndex < contactInputs.Count; contactIndex++)
            {
                var contact = contactInputs[contactIndex];
                var value = contact.Value?.Trim() ?? string.Empty;
                if (contact.Type is null && value.Length == 0)
                {
                    continue;
                }

                hasContactInput = true;
                ContactMethodType? validType = null;
                if (contact.Type is { } type && Enum.IsDefined(type))
                {
                    validType = type;
                }
                else
                {
                    ModelState.AddModelError(
                        $"ResponsiblePersons[{index}].Contacts[{contactIndex}].Type",
                        "Selecione o tipo de contato.");
                }

                if (value.Length == 0)
                {
                    ModelState.AddModelError(
                        $"ResponsiblePersons[{index}].Contacts[{contactIndex}].Value",
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
                            $"ResponsiblePersons[{index}].Contacts[{contactIndex}].Value",
                            "Informe um telefone brasileiro ou e-mail válido.");
                    }
                    else
                    {
                        contactRequests.Add(new RegisterResponsiblePersonContactRequest(
                            contactType,
                            value));
                    }
                }
            }

            if (fullName.Length == 0 && input.Kinship is null && !hasContactInput)
            {
                continue;
            }

            if (fullName.Length == 0)
            {
                ModelState.AddModelError(
                    $"ResponsiblePersons[{index}].FullName",
                    "Informe o nome completo do responsável.");
            }

            if (input.Kinship is null || !Enum.IsDefined(input.Kinship.Value))
            {
                ModelState.AddModelError(
                    $"ResponsiblePersons[{index}].Kinship",
                    "Selecione o parentesco do responsável.");
            }

            if (contactRequests.Count == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cada responsável precisa de ao menos um telefone ou e-mail válido.");
            }

            if (fullName.Length > 0 &&
                input.Kinship is { } kinship &&
                Enum.IsDefined(kinship) &&
                contactRequests.Count > 0)
            {
                responsiblePersonRequests.Add(new RegisterResponsiblePersonRequest(
                    fullName,
                    kinship,
                    contactRequests));
            }
        }

        if (!ModelState.IsValid || model.DateOfBirth is null)
        {
            await PopulateRegistrationOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await patientRegistrationService.RegisterAsync(
            new RegisterPatientRequest(
                model.FullName,
                model.DateOfBirth.Value,
                model.Cid10Code,
                responsiblePersonRequests,
                model.ProfessionalIds),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddRegistrationError(result.Failure);
            await PopulateRegistrationOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var patient = result.Patient
            ?? throw new InvalidOperationException("Successful patient registration returned no patient.");

        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";
        return View("Created", new CreatedPatientViewModel
        {
            PatientId = patient.Id,
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            AgeDisplay = patient.GetAgeDisplay(DateOnly.FromDateTime(DateTime.Today)),
            Cid10Code = patient.Cid10CatalogEntry.Code
                ?? throw new InvalidOperationException("Registered patient CID entry has no code."),
            Cid10Description = patient.Cid10CatalogEntry.Description,
            AssociatedProfessionalNames = patient.PatientProfessionals
                .Select(association => association.Professional.FullName)
                .OrderBy(name => name)
                .ToArray()
        });
    }

    [HttpGet]
    public async Task<IActionResult> ManageProfessionals(
        int id,
        CancellationToken cancellationToken)
    {
        var management = await patientProfessionalAssociationService.GetManagementAsync(
            id,
            cancellationToken);
        if (management is null)
        {
            return NotFound();
        }

        return View(ToManagementViewModel(id, management));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageProfessionals(
        int id,
        PatientProfessionalManagementViewModel model,
        CancellationToken cancellationToken)
    {
        model.SelectedProfessionalIds ??= [];
        if (model.SelectedProfessionalIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Selecione ao menos um profissional.");
        }
        else
        {
            var result = await patientProfessionalAssociationService.AddToPatientAsync(
                id,
                model.SelectedProfessionalIds,
                cancellationToken);
            if (result.Failure is PatientProfessionalAssociationFailure.PatientNotFound)
            {
                return NotFound();
            }

            if (result.Failure is PatientProfessionalAssociationFailure.InvalidProfessional)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Selecione apenas profissionais cadastrados.");
            }
            else if (result.Succeeded)
            {
                TempData["PatientProfessionalNotice"] = "Profissionais associados ao paciente.";
                return RedirectToAction(nameof(ManageProfessionals), new { id });
            }
        }

        var management = await patientProfessionalAssociationService.GetManagementAsync(
            id,
            cancellationToken);
        if (management is null)
        {
            return NotFound();
        }

        return View(ToManagementViewModel(id, management, model.SelectedProfessionalIds));
    }

    private void AddRegistrationError(PatientRegistrationFailure? failure)
    {
        var (key, message) = failure switch
        {
            PatientRegistrationFailure.InvalidFullName =>
                (nameof(CreatePatientViewModel.FullName), "Informe um nome completo válido."),
            PatientRegistrationFailure.FutureDateOfBirth =>
                (nameof(CreatePatientViewModel.DateOfBirth), "A data de nascimento não pode estar no futuro."),
            PatientRegistrationFailure.DuplicatePatient =>
                (nameof(CreatePatientViewModel.FullName),
                    "Já existe um paciente com esse nome e data de nascimento."),
            PatientRegistrationFailure.InvalidResponsiblePerson =>
                (string.Empty, "Informe nome completo e parentesco para cada responsável adicionado."),
            PatientRegistrationFailure.DuplicateResponsiblePerson =>
                (string.Empty, "Não é possível cadastrar nomes duplicados de responsáveis para o mesmo paciente."),
            PatientRegistrationFailure.MissingResponsiblePersonContact =>
                (string.Empty, "Cada responsável precisa de ao menos um telefone ou e-mail."),
            PatientRegistrationFailure.InvalidResponsiblePersonContact =>
                (string.Empty, "Informe um telefone brasileiro ou e-mail válido para cada responsável."),
            PatientRegistrationFailure.CatalogUnavailable =>
                (string.Empty, "O catálogo CID-10 ainda não foi carregado neste ambiente."),
            PatientRegistrationFailure.InvalidCid10Code =>
                (nameof(CreatePatientViewModel.Cid10Code), "Selecione um CID-10 válido do catálogo atual."),
            PatientRegistrationFailure.InvalidProfessional =>
                (nameof(CreatePatientViewModel.ProfessionalIds),
                    "Selecione apenas profissionais cadastrados."),
            _ => throw new InvalidOperationException(
                $"Unsupported patient registration failure: {failure}.")
        };

        ModelState.AddModelError(key, message);
    }

    private async Task PopulateRegistrationOptionsAsync(
        CreatePatientViewModel model,
        CancellationToken cancellationToken)
    {
        var professionals = await patientProfessionalAssociationService.GetRegistrationOptionsAsync(
            cancellationToken);
        model.AvailableProfessionals = professionals
            .Select(professional => new ProfessionalOptionViewModel
            {
                Id = professional.Id,
                FullName = professional.FullName,
                IsActive = professional.IsActive
            })
            .ToList();
    }

    private static PatientProfessionalManagementViewModel ToManagementViewModel(
        int patientId,
        PatientProfessionalManagement management,
        IReadOnlyList<int>? selectedProfessionalIds = null) =>
        new()
        {
            PatientId = patientId,
            PatientFullName = management.PatientFullName,
            SelectedProfessionalIds = selectedProfessionalIds?.ToList() ?? [],
            AssociatedProfessionals = management.AssociatedProfessionals
                .Select(professional => new ProfessionalOptionViewModel
                {
                    Id = professional.Id,
                    FullName = professional.FullName,
                    IsActive = professional.IsActive
                })
                .ToArray(),
            AvailableProfessionals = management.AvailableProfessionals
                .Select(professional => new ProfessionalOptionViewModel
                {
                    Id = professional.Id,
                    FullName = professional.FullName,
                    IsActive = professional.IsActive
                })
                .ToArray()
        };

    private sealed record Cid10Option(string Code, string Description, string Hierarchy);
}
