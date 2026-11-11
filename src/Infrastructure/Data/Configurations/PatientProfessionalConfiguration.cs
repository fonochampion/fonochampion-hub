using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class PatientProfessionalConfiguration : IEntityTypeConfiguration<PatientProfessional>
{
    public void Configure(EntityTypeBuilder<PatientProfessional> builder)
    {
        builder.ToTable("PatientProfessionals");

        builder.HasKey(association => new
        {
            association.PatientId,
            association.ProfessionalId
        });

        builder.HasOne(association => association.Patient)
            .WithMany(patient => patient.PatientProfessionals)
            .HasForeignKey(association => association.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(association => association.Professional)
            .WithMany(professional => professional.PatientProfessionals)
            .HasForeignKey(association => association.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
