using System.ComponentModel.DataAnnotations;
using FonoChampionHub.Domain.Enums;

namespace FonoChampionHub.Domain.Entities;

public sealed class ResponsiblePersonContact
{
    public int Id { get; set; }

    public ResponsiblePersonContactType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public int ResponsiblePersonId { get; set; }

    public ResponsiblePerson ResponsiblePerson { get; set; } = null!;

    public static bool IsValidBrazilianTelephone(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var telephone = value.Trim();
        if (telephone.StartsWith("+55", StringComparison.Ordinal))
        {
            telephone = telephone[3..];
        }
        else if (telephone.StartsWith('+'))
        {
            return false;
        }

        var digitCount = 0;
        foreach (var character in telephone)
        {
            if (character is >= '0' and <= '9')
            {
                digitCount++;
                continue;
            }

            if (character is ' ' or '(' or ')' or '-')
            {
                continue;
            }

            return false;
        }

        return digitCount is 10 or 11;
    }

    public static bool IsValidEmailAddress(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new EmailAddressAttribute().IsValid(value);
    }
}
