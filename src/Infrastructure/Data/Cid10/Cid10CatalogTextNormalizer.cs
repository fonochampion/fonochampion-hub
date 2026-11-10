using System.Globalization;
using System.Text;

namespace FonoChampionHub.Infrastructure.Data.Cid10;

internal static class Cid10CatalogTextNormalizer
{
    public static string NormalizeSearchText(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var normalized = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                normalized.Append(char.ToUpperInvariant(character));
            }
        }

        return normalized.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string NormalizeCode(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
}
