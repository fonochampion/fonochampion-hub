using FonoChampionHub.Web.Identity;

namespace FonoChampionHub.Web.Middleware;

public sealed class RequirePasswordChangeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var mustChangePassword = context.User
            .FindFirst(ApplicationUserClaimsPrincipalFactory.MustChangePasswordClaim)?.Value == "True";

        if (mustChangePassword && !IsAllowedDuringPasswordChange(context.Request.Path))
        {
            context.Response.Redirect("/Account/ChangePassword");
            return;
        }

        await next(context);
    }

    private static bool IsAllowedDuringPasswordChange(PathString path) =>
        path.StartsWithSegments("/Account/ChangePassword")
        || path.StartsWithSegments("/Account/Logout")
        || path.StartsWithSegments("/css")
        || path.StartsWithSegments("/js")
        || path.StartsWithSegments("/lib")
        || path.Equals(new PathString("/favicon.ico"));
}
