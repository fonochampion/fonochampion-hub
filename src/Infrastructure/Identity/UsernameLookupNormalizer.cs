using Microsoft.AspNetCore.Identity;

namespace FonoChampionHub.Infrastructure.Identity;

public sealed class UsernameLookupNormalizer : ILookupNormalizer
{
    public string? NormalizeName(string? name) => name?.Trim().ToUpperInvariant();

    public string? NormalizeEmail(string? email) => email?.ToUpperInvariant();
}
