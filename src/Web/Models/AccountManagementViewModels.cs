using System.ComponentModel.DataAnnotations;

namespace FonoChampionHub.Web.Models;

public sealed class AccountListViewModel
{
    public IReadOnlyList<AccountListItemViewModel> Accounts { get; init; } = [];
}

public sealed class AccountListItemViewModel
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public string? Email { get; init; }
    public required string Role { get; init; }
    public bool IsActive { get; init; }
}

public abstract class AccountFormViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome de exibição.")]
    public string DisplayName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public sealed class CreateAccountViewModel : AccountFormViewModel
{
}

public sealed class EditAccountViewModel : AccountFormViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;
}

public sealed class CreatedAccountViewModel
{
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public required string TemporaryPassword { get; init; }
}
