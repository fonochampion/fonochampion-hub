using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FonoChampionHub.Web.Controllers;

[Authorize(Roles = SystemRoles.Administrator)]
public sealed class CancellationReasonsController(ApplicationDbContext dbContext) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var reasons = await dbContext.AttendanceCancellationReasons
            .AsNoTracking()
            .OrderBy(reason => reason.NormalizedName)
            .Select(reason => new CancellationReasonListItemViewModel
            {
                Id = reason.Id,
                Name = reason.Name,
                Description = reason.Description,
                IsActive = reason.IsActive
            })
            .ToListAsync(cancellationToken);

        return View(new CancellationReasonListViewModel { Reasons = reasons });
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateCancellationReasonViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateCancellationReasonViewModel model,
        CancellationToken cancellationToken)
    {
        Normalize(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedName = AttendanceCancellationReason.NormalizeNameForComparison(model.Name);
        if (await dbContext.AttendanceCancellationReasons
            .AnyAsync(reason => reason.NormalizedName == normalizedName, cancellationToken))
        {
            AddDuplicateNameError();
            return View(model);
        }

        var reason = new AttendanceCancellationReason
        {
            Name = model.Name,
            NormalizedName = normalizedName,
            Description = model.Description,
            IsActive = true
        };
        dbContext.AttendanceCancellationReasons.Add(reason);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateNameConstraintViolation(exception))
        {
            AddDuplicateNameError();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var reason = await dbContext.AttendanceCancellationReasons
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (reason is null)
        {
            return NotFound();
        }

        return View(new EditCancellationReasonViewModel
        {
            Id = reason.Id,
            Name = reason.Name,
            Description = reason.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EditCancellationReasonViewModel model,
        CancellationToken cancellationToken)
    {
        var reason = await dbContext.AttendanceCancellationReasons
            .SingleOrDefaultAsync(item => item.Id == model.Id, cancellationToken);
        if (reason is null)
        {
            return NotFound();
        }

        Normalize(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedName = AttendanceCancellationReason.NormalizeNameForComparison(model.Name);
        if (await dbContext.AttendanceCancellationReasons
            .AnyAsync(
                item => item.Id != reason.Id && item.NormalizedName == normalizedName,
                cancellationToken))
        {
            AddDuplicateNameError();
            return View(model);
        }

        reason.Name = model.Name;
        reason.NormalizedName = normalizedName;
        reason.Description = model.Description;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateNameConstraintViolation(exception))
        {
            AddDuplicateNameError();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivate(int id, CancellationToken cancellationToken)
    {
        var reason = await dbContext.AttendanceCancellationReasons
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (reason is null)
        {
            return NotFound();
        }

        if (reason.IsActive)
        {
            reason.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }

    private void Normalize(CancellationReasonFormViewModel model)
    {
        model.Name = model.Name?.Trim() ?? string.Empty;
        ModelState.Remove(nameof(model.Name));
        TryValidateModel(model);
    }

    private void AddDuplicateNameError() =>
        ModelState.AddModelError(
            nameof(CancellationReasonFormViewModel.Name),
            "Já existe um motivo com esse nome, ativo ou inativo.");

    private static bool IsDuplicateNameConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: AttendanceCancellationReason.NormalizedNameIndexName
        };
}
