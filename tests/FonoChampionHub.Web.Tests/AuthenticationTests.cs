using System.Net;
using System.Text.RegularExpressions;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FonoChampionHub.Web.Tests;

public sealed class AuthenticationTests
{
    [Fact]
    public async Task Bootstrap_creates_active_administrator_with_required_password_change()
    {
        using var factory = new AuthenticationWebApplicationFactory();

        var administrator = await factory.GetUserAsync("bootstrap");

        Assert.True(administrator.IsActive);
        Assert.True(administrator.MustChangePassword);
        Assert.True(await factory.CheckPasswordAsync(administrator, "BootstrapPassword123"));
        Assert.Equal(["Administrador"], await factory.GetRolesAsync(administrator));
    }

    [Fact]
    public async Task Bootstrap_leaves_existing_administrator_unchanged()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        var administrator = await factory.GetUserAsync("bootstrap");
        await factory.SetUserStateAsync("bootstrap", isActive: false, mustChangePassword: false);

        await factory.ProvisionAsync(" bootstrap ", "ChangedBootstrapPassword123");

        administrator = await factory.GetUserAsync("bootstrap");
        Assert.False(administrator.IsActive);
        Assert.False(administrator.MustChangePassword);
        Assert.True(await factory.CheckPasswordAsync(administrator, "BootstrapPassword123"));
        Assert.False(await factory.CheckPasswordAsync(administrator, "ChangedBootstrapPassword123"));
    }

    [Fact]
    public async Task Bootstrap_fails_when_configured_username_differs_from_existing_administrator()
    {
        using var factory = new AuthenticationWebApplicationFactory();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.ProvisionAsync("renamed-bootstrap", "AnotherPassword123"));

        Assert.Contains("already exists with a different username", exception.Message);
        Assert.Equal(1, await factory.CountAdministratorsAsync());
        Assert.False(await factory.UserExistsAsync("renamed-bootstrap"));
    }

    [Fact]
    public async Task Bootstrap_fails_when_configured_username_belongs_to_non_administrator()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        await factory.AddUserAsync(username: "Alice");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.ProvisionAsync(" alice ", "AnotherPassword123"));

        Assert.Contains("without the 'Administrador' role", exception.Message);
        var user = await factory.GetUserAsync("alice");
        Assert.Equal([], await factory.GetRolesAsync(user));
        Assert.False(user.MustChangePassword);
    }

    [Fact]
    public async Task Login_normalizes_username_and_enforces_first_password_change()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await factory.AddUserAsync(mustChangePassword: true);

        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        using var loginResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "  ALICE  "),
            ("Password", "InitialPassword123"),
            ("RememberMe", "false"));

        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
        Assert.Equal("/Account/ChangePassword", loginResponse.Headers.Location?.OriginalString);

        using var blockedResponse = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, blockedResponse.StatusCode);
        Assert.Equal("/Account/ChangePassword", blockedResponse.Headers.Location?.OriginalString);

        var passwordToken = await GetAntiforgeryTokenAsync(client, "/Account/ChangePassword");
        using var invalidPasswordResponse = await PostFormAsync(
            client,
            "/Account/ChangePassword",
            passwordToken,
            ("CurrentPassword", "InitialPassword123"),
            ("NewPassword", "short"));

        Assert.Equal(HttpStatusCode.OK, invalidPasswordResponse.StatusCode);
        var invalidPasswordBody = await invalidPasswordResponse.Content.ReadAsStringAsync();
        Assert.Contains("no mínimo 12 caracteres", WebUtility.HtmlDecode(invalidPasswordBody));

        passwordToken = await GetAntiforgeryTokenAsync(client, "/Account/ChangePassword");
        using var changeResponse = await PostFormAsync(
            client,
            "/Account/ChangePassword",
            passwordToken,
            ("CurrentPassword", "InitialPassword123"),
            ("NewPassword", "a-new-password"));

        Assert.Equal(HttpStatusCode.Redirect, changeResponse.StatusCode);
        Assert.Equal("/", changeResponse.Headers.Location?.OriginalString);

        using var homeResponse = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.OK, homeResponse.StatusCode);
    }

    [Fact]
    public async Task Login_uses_same_error_and_resets_failure_count_after_lockout_expires()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await factory.AddUserAsync();

        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        using var unknownUserResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "unknown"),
            ("Password", "IncorrectPassword123"),
            ("RememberMe", "false"));
        var unknownUserBody = await unknownUserResponse.Content.ReadAsStringAsync();

        using var incorrectPasswordResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "alice"),
            ("Password", "IncorrectPassword123"),
            ("RememberMe", "false"));
        var incorrectPasswordBody = await incorrectPasswordResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, unknownUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, incorrectPasswordResponse.StatusCode);
        Assert.Contains("Não foi possível entrar.", WebUtility.HtmlDecode(unknownUserBody));
        Assert.Contains("Não foi possível entrar.", WebUtility.HtmlDecode(incorrectPasswordBody));

        string? lastFailureBody = null;
        for (var attempt = 1; attempt < 5; attempt++)
        {
            using var response = await PostFormAsync(
                client,
                "/Account/Login",
                token,
                ("Username", "alice"),
                ("Password", "IncorrectPassword123"),
                ("RememberMe", "false"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            lastFailureBody = await response.Content.ReadAsStringAsync();
        }

        Assert.Contains("Não foi possível entrar.", WebUtility.HtmlDecode(lastFailureBody));
        var user = await factory.GetUserAsync();
        Assert.True(user.LockoutEnabled, $"LockoutEnabled was {user.LockoutEnabled}.");
        Assert.True(user.LockoutEnd > DateTimeOffset.UtcNow);
        Assert.Equal(0, user.AccessFailedCount);

        await factory.SetLockoutEndAsync(DateTimeOffset.UtcNow.AddSeconds(-1));

        using var afterLockoutResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "alice"),
            ("Password", "IncorrectPassword123"),
            ("RememberMe", "false"));

        Assert.Equal(HttpStatusCode.OK, afterLockoutResponse.StatusCode);
        user = await factory.GetUserAsync();
        Assert.Equal(1, user.AccessFailedCount);
        Assert.Null(user.LockoutEnd);

        using var successResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "ALICE"),
            ("Password", "InitialPassword123"),
            ("RememberMe", "false"));

        Assert.Equal(HttpStatusCode.Redirect, successResponse.StatusCode);
        user = await factory.GetUserAsync();
        Assert.Equal(0, user.AccessFailedCount);
    }

    [Fact]
    public async Task Inactive_user_cannot_sign_in()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await factory.AddUserAsync(isActive: false);

        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        using var response = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "alice"),
            ("Password", "InitialPassword123"),
            ("RememberMe", "false"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "Não foi possível entrar.",
            WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));

        using var changePasswordResponse = await client.GetAsync("/Account/ChangePassword");
        Assert.Equal(HttpStatusCode.Redirect, changePasswordResponse.StatusCode);
        Assert.Equal("/Account/Login", changePasswordResponse.Headers.Location?.AbsolutePath);
        Assert.Contains("ReturnUrl=", changePasswordResponse.Headers.Location!.Query);
    }

    [Fact]
    public async Task Session_expires_after_thirty_minutes_without_activity()
    {
        var clock = new TestTimeProvider();
        using var factory = new AuthenticationWebApplicationFactory(clock);
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await factory.AddUserAsync();

        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        using var loginResponse = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", "alice"),
            ("Password", "InitialPassword123"),
            ("RememberMe", "false"));
        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        using var activeSessionResponse = await client.GetAsync("/Account/ChangePassword");
        Assert.Equal(HttpStatusCode.Redirect, activeSessionResponse.StatusCode);
        Assert.Equal("/", activeSessionResponse.Headers.Location?.OriginalString);

        clock.Advance(TimeSpan.FromMinutes(31));

        using var expiredSessionResponse = await client.GetAsync("/Account/ChangePassword");
        Assert.Equal(HttpStatusCode.Redirect, expiredSessionResponse.StatusCode);
        Assert.Equal("/Account/Login", expiredSessionResponse.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task User_management_requires_administrator_role()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var anonymousClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using var anonymousResponse = await anonymousClient.GetAsync("/UserManagement/Index");
        Assert.Equal(HttpStatusCode.Redirect, anonymousResponse.StatusCode);
        Assert.Equal("/Account/Login", anonymousResponse.Headers.Location?.AbsolutePath);

        await factory.AddUserAsync();
        using var userClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await LoginAsync(userClient, "Alice", "InitialPassword123");

        using var deniedResponse = await userClient.GetAsync("/UserManagement/Index");
        Assert.Equal(HttpStatusCode.Redirect, deniedResponse.StatusCode);
        Assert.Equal("/Account/Login", deniedResponse.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task Administrator_can_create_account_with_one_time_temporary_password()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var adminClient = await CreateAdministratorClientAsync(factory);
        var token = await GetAntiforgeryTokenAsync(adminClient, "/UserManagement/Create");

        using var response = await PostFormAsync(
            adminClient,
            "/UserManagement/Create",
            token,
            ("Username", " NewUser "),
            ("DisplayName", "  Nome Novo  "),
            ("Email", "  new@example.com  "),
            ("Role", "Terapeuta"),
            ("IsActive", "true"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("no-store", response.Headers.CacheControl?.ToString());
        var html = await response.Content.ReadAsStringAsync();
        var passwordMatch = Regex.Match(html, "<code>([^<]+)</code>", RegexOptions.CultureInvariant);
        Assert.True(passwordMatch.Success, "The created account page must show the temporary password once.");
        var temporaryPassword = WebUtility.HtmlDecode(passwordMatch.Groups[1].Value);
        Assert.True(temporaryPassword.Length >= 12);

        var user = await factory.GetUserAsync("newuser");
        Assert.Equal("Nome Novo", user.DisplayName);
        Assert.Equal("new@example.com", user.Email);
        Assert.True(user.IsActive);
        Assert.True(user.MustChangePassword);
        Assert.True(await factory.CheckPasswordAsync(user, temporaryPassword));
        Assert.Equal(["Terapeuta"], await factory.GetRolesAsync(user));

        using var listResponse = await adminClient.GetAsync("/UserManagement/Index");
        Assert.DoesNotContain(temporaryPassword, await listResponse.Content.ReadAsStringAsync());

        using var createdUserClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await LoginAsync(createdUserClient, "newuser", temporaryPassword);
        using var changePasswordResponse = await createdUserClient.GetAsync("/Account/ChangePassword");
        Assert.Equal(HttpStatusCode.OK, changePasswordResponse.StatusCode);
    }

    [Fact]
    public async Task Administrator_cannot_create_account_with_blank_trimmed_display_name()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var adminClient = await CreateAdministratorClientAsync(factory);
        var token = await GetAntiforgeryTokenAsync(adminClient, "/UserManagement/Create");

        using var response = await PostFormAsync(
            adminClient,
            "/UserManagement/Create",
            token,
            ("Username", "blank-display"),
            ("DisplayName", "   "),
            ("Email", string.Empty),
            ("Role", "Atendente"),
            ("IsActive", "true"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "Informe o nome de exibição.",
            WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        Assert.False(await factory.UserExistsAsync("blank-display"));
    }

    [Fact]
    public async Task Administrator_can_edit_account_fields_role_and_active_state()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        await factory.AddUserAsync();
        using var adminClient = await CreateAdministratorClientAsync(factory);
        var user = await factory.GetUserAsync();
        var token = await GetAntiforgeryTokenAsync(
            adminClient,
            $"/UserManagement/Edit/{user.Id}");

        using var response = await PostFormAsync(
            adminClient,
            "/UserManagement/Edit",
            token,
            ("Id", user.Id),
            ("Username", " UpdatedAlice "),
            ("DisplayName", "  Nome Atualizado "),
            ("Email", "  "),
            ("Role", "Atendente"),
            ("IsActive", "false"));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var updatedUser = await factory.GetUserAsync("updatedalice");
        Assert.Equal("Nome Atualizado", updatedUser.DisplayName);
        Assert.Null(updatedUser.Email);
        Assert.False(updatedUser.IsActive);
        Assert.Equal(["Atendente"], await factory.GetRolesAsync(updatedUser));
    }

    [Fact]
    public async Task Changing_the_current_administrators_role_revokes_the_existing_session()
    {
        using var factory = new AuthenticationWebApplicationFactory();
        using var adminClient = await CreateAdministratorClientAsync(factory);
        var administrator = await factory.GetUserAsync("bootstrap");
        var token = await GetAntiforgeryTokenAsync(
            adminClient,
            $"/UserManagement/Edit/{administrator.Id}");

        using var editResponse = await PostFormAsync(
            adminClient,
            "/UserManagement/Edit",
            token,
            ("Id", administrator.Id),
            ("Username", "bootstrap"),
            ("DisplayName", administrator.DisplayName),
            ("Email", string.Empty),
            ("Role", "Atendente"),
            ("IsActive", "true"));

        Assert.Equal(HttpStatusCode.Redirect, editResponse.StatusCode);
        using var revokedSessionResponse = await adminClient.GetAsync("/UserManagement/Index");
        Assert.Equal(HttpStatusCode.Redirect, revokedSessionResponse.StatusCode);
        Assert.Equal("/Account/Login", revokedSessionResponse.Headers.Location?.AbsolutePath);
    }

    private static async Task<HttpClient> CreateAdministratorClientAsync(
        AuthenticationWebApplicationFactory factory)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        await LoginAsync(client, "bootstrap", "BootstrapPassword123");
        var token = await GetAntiforgeryTokenAsync(client, "/Account/ChangePassword");
        using var changePasswordResponse = await PostFormAsync(
            client,
            "/Account/ChangePassword",
            token,
            ("CurrentPassword", "BootstrapPassword123"),
            ("NewPassword", "BootstrapPassword456"));
        Assert.Equal(HttpStatusCode.Redirect, changePasswordResponse.StatusCode);
        return client;
    }

    private static async Task LoginAsync(HttpClient client, string username, string password)
    {
        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        using var response = await PostFormAsync(
            client,
            "/Account/Login",
            token,
            ("Username", username),
            ("Password", password),
            ("RememberMe", "false"));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    private static async Task<string> GetAntiforgeryTokenAsync(HttpClient client, string path)
    {
        using var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(
            html,
            "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"",
            RegexOptions.CultureInvariant);
        Assert.True(match.Success, "The form must include an antiforgery token.");
        return match.Groups[1].Value;
    }

    private static Task<HttpResponseMessage> PostFormAsync(
        HttpClient client,
        string path,
        string token,
        params (string Name, string Value)[] fields)
    {
        var form = fields.ToDictionary(field => field.Name, field => field.Value);
        form["__RequestVerificationToken"] = token;
        return client.PostAsync(path, new FormUrlEncodedContent(form));
    }

    private sealed class AuthenticationWebApplicationFactory(TestTimeProvider? clock = null)
        : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private readonly TestTimeProvider _clock = clock ?? new TestTimeProvider();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["InitialAdministrator:Username"] = "bootstrap",
                    ["InitialAdministrator:Password"] = "BootstrapPassword123"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<ApplicationDbContext>();
                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase(_databaseName));
                services.PostConfigure<CookieAuthenticationOptions>(
                    IdentityConstants.ApplicationScheme,
                    options => options.TimeProvider = _clock);
            });
        }

        public async Task AddUserAsync(
            bool mustChangePassword = false,
            bool isActive = true,
            string username = "Alice")
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser
            {
                UserName = username,
                DisplayName = username,
                IsActive = isActive,
                MustChangePassword = mustChangePassword
            };
            var result = await userManager.CreateAsync(user, "InitialPassword123");
            Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Code)));
        }

        public async Task<ApplicationUser> GetUserAsync(string username = "alice")
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return (await userManager.FindByNameAsync(username))!;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await userManager.GetRolesAsync(user);
        }

        public async Task SetUserStateAsync(string username, bool isActive, bool mustChangePassword)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = (await userManager.FindByNameAsync(username))!;
            user.IsActive = isActive;
            user.MustChangePassword = mustChangePassword;
            var result = await userManager.UpdateAsync(user);
            Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Code)));
        }

        public async Task ProvisionAsync(string username, string password)
        {
            using var scope = Services.CreateScope();
            var bootstrapper = scope.ServiceProvider.GetRequiredService<InitialAdministratorBootstrapper>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["InitialAdministrator:Username"] = username,
                    ["InitialAdministrator:Password"] = password
                })
                .Build();
            await bootstrapper.ProvisionAsync(configuration);
        }

        public async Task<int> CountAdministratorsAsync()
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return (await userManager.GetUsersInRoleAsync("Administrador")).Count;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await userManager.FindByNameAsync(username) is not null;
        }

        public async Task SetLockoutEndAsync(DateTimeOffset lockoutEnd)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = (await userManager.FindByNameAsync("alice"))!;
            user.LockoutEnd = lockoutEnd;
            var result = await userManager.UpdateAsync(user);
            Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Code)));
        }
    }

    private sealed class TestTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => _now;

        public void Advance(TimeSpan duration) => _now += duration;
    }
}
