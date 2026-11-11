using FonoChampionHub.Domain.Services;

namespace FonoChampionHub.Domain.Entities;

public sealed class Patient
{
    public const string FullNameDateOfBirthIndexName =
        "IX_Patients_NormalizedFullName_DateOfBirth";

    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string NormalizedFullName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public int Cid10CatalogEntryId { get; set; }

    public Cid10CatalogEntry Cid10CatalogEntry { get; set; } = null!;

    public ICollection<ResponsiblePerson> ResponsiblePersons { get; set; } =
        new List<ResponsiblePerson>();

    public static string NormalizeFullNameForComparison(string fullName) =>
        FullNameNormalizer.NormalizeForComparison(fullName);

    public string GetAgeDisplay(DateOnly asOf)
    {
        var years = asOf.Year - DateOfBirth.Year;
        if (DateOfBirth.AddYears(years) > asOf)
        {
            years--;
        }

        if (years >= 1)
        {
            return years == 1 ? "1 ano" : $"{years} anos";
        }

        var months = (asOf.Year - DateOfBirth.Year) * 12 + asOf.Month - DateOfBirth.Month;
        if (DateOfBirth.AddMonths(months) > asOf)
        {
            months--;
        }

        return months == 1 ? "1 mês" : $"{months} meses";
    }
}
