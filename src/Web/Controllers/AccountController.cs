using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Identity;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FonoChampionHub.Web.Controllers;

public sealed class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
    : Controller
{
    private const string GenericLoginError =
        "Não foi possível entrar. Verifique as credenciais ou tente novamente mais tarde.";

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (MustChangePassword())
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { Username = string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var user = await userManager.FindByNameAsync(username);

        if (user?.LockoutEnd is { } lockoutEnd && lockoutEnd <= DateTimeOffset.UtcNow)
        {
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            EnsureSucceeded(await userManager.UpdateAsync(user));
        }

        if (user is { IsActive: true })
        {
            var result = await signInManager.PasswordSignInAsync(
                username,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (user.MustChangePassword)
                {
                    return RedirectToAction(nameof(ChangePassword));
                }

                if (Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError(string.Empty, GenericLoginError);
        return View(model);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!MustChangePassword())
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!MustChangePassword())
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            await signInManager.SignOutAsync();
            return Challenge();
        }

        user.MustChangePassword = false;
        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            user.MustChangePassword = true;
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível alterar a senha. Confira a senha atual e os critérios da nova senha.");
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private bool MustChangePassword() =>
        User.FindFirst(ApplicationUserClaimsPrincipalFactory.MustChangePasswordClaim)?.Value == "True";

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not reset expired login lockout: {string.Join("; ", result.Errors.Select(error => error.Code))}");
        }
    }
}
