using Microsoft.AspNetCore.Authentication;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Extensions;
using FonoChampionHub.Infrastructure.Identity;
using FonoChampionHub.Web.Identity;
using FonoChampionHub.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

string? cid10ImportDirectory = null;
if (args.Length > 0 && args[0] == "--import-cid10")
{
    if (args.Length != 2 || string.IsNullOrWhiteSpace(args[1]))
    {
        throw new ArgumentException("Usage: --import-cid10 <source-directory>");
    }

    cid10ImportDirectory = args[1];
}
else if (args.Contains("--import-cid10", StringComparer.Ordinal))
{
    throw new ArgumentException("--import-cid10 must be the first and only application argument.");
}

var builder = WebApplication.CreateBuilder(cid10ImportDirectory is null ? args : []);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddInfrastructure(builder.Configuration);
new IdentityBuilder(typeof(ApplicationUser), builder.Services).AddSignInManager();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events.OnValidatePrincipal = async context =>
        {
            var signInManager = context.HttpContext.RequestServices
                .GetRequiredService<SignInManager<ApplicationUser>>();
            if (await signInManager.ValidateSecurityStampAsync(context.Principal!) is null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (cid10ImportDirectory is not null)
{
    await using var importScope = app.Services.CreateAsyncScope();
    var importer = importScope.ServiceProvider.GetRequiredService<ICid10CatalogImporter>();
    var importResult = await importer.ImportAsync(cid10ImportDirectory);
    Console.WriteLine(
        $"CID-10 catalog imported: {importResult.TotalEntryCount} entries " +
        $"(chapters: {importResult.ChapterCount}, groups: {importResult.GroupCount}, " +
        $"categories: {importResult.CategoryCount}, subcategories: {importResult.SubcategoryCount}); " +
        $"fingerprint: {importResult.Fingerprint}; already current: {importResult.WasAlreadyCurrent}.");
    return;
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var bootstrapper = scope.ServiceProvider.GetRequiredService<InitialAdministratorBootstrapper>();
    await bootstrapper.ProvisionAsync(app.Configuration);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<RequirePasswordChangeMiddleware>();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program { }
