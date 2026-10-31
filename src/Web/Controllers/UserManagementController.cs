using System.Security.Cryptography;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Controllers;

[Authorize(Roles = SystemRoles.Administrator)]
public sealed class UserManagementController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users
            .OrderBy(user => user.NormalizedUserName)
            .ToListAsync();
        var accounts = new List<AccountListItemViewModel>(users.Count);

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            accounts.Add(new AccountListItemViewModel
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = string.Join(", ", roles),
                IsActive = user.IsActive
            });
        }

        return View(new AccountListViewModel { Accounts = accounts });
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateRoles();
        return View(new CreateAccountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountViewModel model)
    {
        Normalize(model);
        ValidateRole(model.Role);

        if (!ModelState.IsValid)
        {
            PopulateRoles();
            return View(model);
        }

        var temporaryPassword = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(24));
        var user = new ApplicationUser
        {
            UserName = model.Username,
            DisplayName = model.DisplayName,
            Email = model.Email,
            IsActive = model.IsActive,
            MustChangePassword = true
        };

        await using var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync()
            : null;

        var createResult = await userManager.CreateAsync(user, temporaryPassword);
        if (!createResult.Succeeded)
        {
            await RollbackAsync(transaction);
            AddIdentityErrors(createResult);
            PopulateRoles();
            return View(model);
        }

        var roleResult = await userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            await RollbackAsync(transaction);
            AddIdentityErrors(roleResult);
            PopulateRoles();
            return View(model);
        }

        if (transaction is not null)
        {
            await transaction.CommitAsync();
        }

        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";
        return View("Created", new CreatedAccountViewModel
        {
            Username = model.Username,
            DisplayName = model.DisplayName,
            TemporaryPassword = temporaryPassword
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        PopulateRoles();
        return View(new EditAccountViewModel
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAccountViewModel model)
    {
        Normalize(model);
        ValidateRole(model.Role);

        var user = await userManager.FindByIdAsync(model.Id);
        if (user is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            PopulateRoles();
            return View(model);
        }

        var activeStateChanged = user.IsActive != model.IsActive;
        user.UserName = model.Username;
        user.DisplayName = model.DisplayName;
        user.Email = model.Email;
        user.IsActive = model.IsActive;

        await using var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync()
            : null;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            await RollbackAsync(transaction);
            AddIdentityErrors(updateResult);
            PopulateRoles();
            return View(model);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var roleChanged = currentRoles.Count != 1
            || !currentRoles.Contains(model.Role, StringComparer.Ordinal);
        if (currentRoles.Count != 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                await RollbackAsync(transaction);
                AddIdentityErrors(removeResult);
                PopulateRoles();
                return View(model);
            }
        }

        var addResult = await userManager.AddToRoleAsync(user, model.Role);
        if (!addResult.Succeeded)
        {
            await RollbackAsync(transaction);
            AddIdentityErrors(addResult);
            PopulateRoles();
            return View(model);
        }

        if (activeStateChanged || roleChanged)
        {
            var stampResult = await userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                await RollbackAsync(transaction);
                AddIdentityErrors(stampResult);
                PopulateRoles();
                return View(model);
            }
        }

        if (transaction is not null)
        {
            await transaction.CommitAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private void Normalize(AccountFormViewModel model)
    {
        model.Username = model.Username?.Trim() ?? string.Empty;
        model.DisplayName = model.DisplayName?.Trim() ?? string.Empty;
        model.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();

        ModelState.Remove(nameof(model.Username));
        ModelState.Remove(nameof(model.DisplayName));
        ModelState.Remove(nameof(model.Email));
        ModelState.Remove(nameof(model.Role));
        TryValidateModel(model);
    }

    private void ValidateRole(string role)
    {
        if (role is null || !SystemRoles.All.Contains(role, StringComparer.Ordinal))
        {
            ModelState.AddModelError(nameof(AccountFormViewModel.Role), "Selecione um papel válido.");
        }
    }

    private void PopulateRoles() =>
        ViewBag.Roles = SystemRoles.All
            .Select(role => new SelectListItem(role, role))
            .ToList();

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private static async Task RollbackAsync(
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction)
    {
        if (transaction is not null)
        {
            await transaction.RollbackAsync();
        }
    }
}
