using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonoChampionHub.Web.Controllers;

[Authorize(Roles = SystemRoles.Administrator + "," + SystemRoles.Attendant)]
public sealed class PatientController(
    ICid10CatalogQueryService cid10CatalogQueryService,
    IPatientRegistrationService patientRegistrationService)
    : Controller
{
    private const int MinimumCidQueryLength = 2;
    private const int MaximumCidResults = 25;

    [HttpGet]
    public IActionResult Create() => View(new CreatePatientViewModel());

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
        if (!ModelState.IsValid || model.DateOfBirth is null)
        {
            return View(model);
        }

        var result = await patientRegistrationService.RegisterAsync(
            new RegisterPatientRequest(model.FullName, model.DateOfBirth.Value, model.Cid10Code),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddRegistrationError(result.Failure);
            return View(model);
        }

        var patient = result.Patient
            ?? throw new InvalidOperationException("Successful patient registration returned no patient.");

        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";
        return View("Created", new CreatedPatientViewModel
        {
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            AgeDisplay = patient.GetAgeDisplay(DateOnly.FromDateTime(DateTime.Today)),
            Cid10Code = patient.Cid10CatalogEntry.Code
                ?? throw new InvalidOperationException("Registered patient CID entry has no code."),
            Cid10Description = patient.Cid10CatalogEntry.Description
        });
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
            PatientRegistrationFailure.CatalogUnavailable =>
                (string.Empty, "O catálogo CID-10 ainda não foi carregado neste ambiente."),
            PatientRegistrationFailure.InvalidCid10Code =>
                (nameof(CreatePatientViewModel.Cid10Code), "Selecione um CID-10 válido do catálogo atual."),
            _ => throw new InvalidOperationException(
                $"Unsupported patient registration failure: {failure}.")
        };

        ModelState.AddModelError(key, message);
    }

    private sealed record Cid10Option(string Code, string Description, string Hierarchy);
}
