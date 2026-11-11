using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(patient => patient.Id);

        builder.Property(patient => patient.FullName)
            .IsRequired();

        builder.Property(patient => patient.NormalizedFullName)
            .IsRequired();

        builder.Property(patient => patient.DateOfBirth)
            .IsRequired();

        builder.HasIndex(patient => new { patient.NormalizedFullName, patient.DateOfBirth })
            .IsUnique()
            .HasDatabaseName(Patient.FullNameDateOfBirthIndexName);

        builder.HasOne(patient => patient.Cid10CatalogEntry)
            .WithMany()
            .HasForeignKey(patient => patient.Cid10CatalogEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
