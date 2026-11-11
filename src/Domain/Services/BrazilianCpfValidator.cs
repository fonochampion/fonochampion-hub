namespace FonoChampionHub.Domain.Services;

public static class BrazilianCpfValidator
{
    public static string? NormalizeAndValidate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        string digits;
        if (trimmed.Length == 11 && trimmed.All(IsAsciiDigit))
        {
            digits = trimmed;
        }
        else if (trimmed.Length == 14 &&
                 trimmed[3] == '.' &&
                 trimmed[7] == '.' &&
                 trimmed[11] == '-' &&
                 Enumerable.Range(0, trimmed.Length)
                     .Where(index => index is not (3 or 7 or 11))
                     .All(index => IsAsciiDigit(trimmed[index])))
        {
            digits = string.Concat(
                trimmed.AsSpan(0, 3),
                trimmed.AsSpan(4, 3),
                trimmed.AsSpan(8, 3),
                trimmed.AsSpan(12, 2));
        }
        else
        {
            return null;
        }

        if (digits.All(digit => digit == digits[0]))
        {
            return null;
        }

        var firstCheckDigit = CalculateCheckDigit(digits.AsSpan(0, 9), 10);
        if (digits[9] - '0' != firstCheckDigit)
        {
            return null;
        }

        var secondCheckDigit = CalculateCheckDigit(digits.AsSpan(0, 10), 11);
        return digits[10] - '0' == secondCheckDigit ? digits : null;
    }

    private static int CalculateCheckDigit(ReadOnlySpan<char> digits, int initialWeight)
    {
        var sum = 0;
        for (var index = 0; index < digits.Length; index++)
        {
            sum += (digits[index] - '0') * (initialWeight - index);
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static bool IsAsciiDigit(char value) => value is >= '0' and <= '9';
}
