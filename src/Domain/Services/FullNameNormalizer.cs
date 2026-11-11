using System.Text;

namespace FonoChampionHub.Domain.Services;

public static class FullNameNormalizer
{
    public static string NormalizeForComparison(string fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);

        var normalized = new StringBuilder(fullName.Length);
        var pendingWhitespace = false;

        foreach (var character in fullName.Normalize(NormalizationForm.FormC))
        {
            if (char.IsWhiteSpace(character))
            {
                pendingWhitespace = normalized.Length > 0;
                continue;
            }

            if (pendingWhitespace)
            {
                normalized.Append(' ');
                pendingWhitespace = false;
            }

            normalized.Append(char.ToUpperInvariant(character));
        }

        return normalized.ToString();
    }
}
