using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Domain.Services;

namespace FonoChampionHub.Domain.Entities;

public sealed class ResponsiblePerson
{
    public const string PatientNormalizedFullNameIndexName =
        "IX_ResponsiblePersons_PatientId_NormalizedFullName";

    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string NormalizedFullName { get; set; } = string.Empty;

    public ResponsiblePersonKinship Kinship { get; set; }

    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public ICollection<ResponsiblePersonContact> Contacts { get; set; } =
        new List<ResponsiblePersonContact>();

    public static string NormalizeFullNameForComparison(string fullName) =>
        FullNameNormalizer.NormalizeForComparison(fullName);
}
