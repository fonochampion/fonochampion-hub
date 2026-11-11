namespace FonoChampionHub.Domain.Entities;

public sealed class PatientProfessional
{
    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    public int ProfessionalId { get; set; }

    public Professional Professional { get; set; } = null!;
}
