using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class ProfessionalCertificationConfiguration :
    IEntityTypeConfiguration<ProfessionalCertification>
{
    public void Configure(EntityTypeBuilder<ProfessionalCertification> builder)
    {
        builder.ToTable("ProfessionalCertifications");

        builder.HasKey(certification => certification.Id);

        builder.Property(certification => certification.Description)
            .IsRequired();

        builder.Property(certification => certification.Registration)
            .IsRequired();

        builder.Property(certification => certification.CompletionDate)
            .IsRequired();

        builder.HasOne(certification => certification.Professional)
            .WithMany(professional => professional.Certifications)
            .HasForeignKey(certification => certification.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
