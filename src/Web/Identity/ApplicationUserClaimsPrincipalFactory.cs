using System.Security.Claims;
using FonoChampionHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FonoChampionHub.Web.Identity;

public sealed class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(
        userManager,
        roleManager,
        optionsAccessor)
{
    public const string MustChangePasswordClaim = "must_change_password";

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(MustChangePasswordClaim, user.MustChangePassword.ToString()));
        return identity;
    }
}
