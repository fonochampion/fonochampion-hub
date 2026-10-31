using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FonoChampionHub.Infrastructure.Identity;

public sealed class InitialAdministratorBootstrapper(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    private const string ConfigurationSection = "InitialAdministrator";

    public async Task ProvisionAsync(
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var username = configuration[$"{ConfigurationSection}:Username"]?.Trim();
        var password = configuration[$"{ConfigurationSection}:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Configuration keys '{ConfigurationSection}:Username' and '{ConfigurationSection}:Password' are required.");
        }

        var configuredUser = await userManager.FindByNameAsync(username);
        var administratorRoleExists = await roleManager.RoleExistsAsync(SystemRoles.Administrator);

        if (configuredUser is not null)
        {
            if (!administratorRoleExists
                || !await userManager.IsInRoleAsync(configuredUser, SystemRoles.Administrator))
            {
                throw new InvalidOperationException(
                    $"The configured initial Administrator username '{username}' belongs to an account without the '{SystemRoles.Administrator}' role.");
            }
        }
        else if (administratorRoleExists
            && (await userManager.GetUsersInRoleAsync(SystemRoles.Administrator)).Count != 0)
        {
            throw new InvalidOperationException(
                $"An '{SystemRoles.Administrator}' account already exists with a different username. The configured initial Administrator was not created.");
        }

        foreach (var roleName in SystemRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                EnsureSucceeded(await roleManager.CreateAsync(new IdentityRole(roleName)));
            }
        }

        if (configuredUser is not null)
        {
            return;
        }

        var transaction = dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(cancellationToken)
            : null;

        try
        {
            var user = new ApplicationUser
            {
                UserName = username,
                DisplayName = username,
                IsActive = true,
                MustChangePassword = true
            };

            EnsureSucceeded(await userManager.CreateAsync(user, password));
            EnsureSucceeded(await userManager.AddToRoleAsync(user, SystemRoles.Administrator));
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Initial Administrator provisioning failed: {string.Join("; ", result.Errors.Select(error => error.Code))}");
        }
    }
}
