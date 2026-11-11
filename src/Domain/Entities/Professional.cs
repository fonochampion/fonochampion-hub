namespace FonoChampionHub.Domain.Entities;

public sealed class Professional
{
    public const string CpfUniqueIndexName = "IX_Professionals_Cpf";

    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public ICollection<ProfessionalCertification> Certifications { get; set; } =
        new List<ProfessionalCertification>();

    public ICollection<ProfessionalContact> Contacts { get; set; } =
        new List<ProfessionalContact>();

    public ICollection<PatientProfessional> PatientProfessionals { get; set; } =
        new List<PatientProfessional>();
}
