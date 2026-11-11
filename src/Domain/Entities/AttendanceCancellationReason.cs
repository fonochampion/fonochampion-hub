namespace FonoChampionHub.Domain.Entities;

public sealed class AttendanceCancellationReason
{
    public const string NormalizedNameIndexName =
        "IX_AttendanceCancellationReasons_NormalizedName";

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public static string NormalizeNameForComparison(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return name.Trim().ToUpperInvariant();
    }
}
