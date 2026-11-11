using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class ResponsiblePersonConfiguration : IEntityTypeConfiguration<ResponsiblePerson>
{
    public void Configure(EntityTypeBuilder<ResponsiblePerson> builder)
    {
        builder.ToTable("ResponsiblePersons");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.FullName)
            .IsRequired();

        builder.Property(person => person.NormalizedFullName)
            .IsRequired();

        builder.Property(person => person.Kinship)
            .IsRequired();

        builder.HasIndex(person => new { person.PatientId, person.NormalizedFullName })
            .IsUnique()
            .HasDatabaseName(ResponsiblePerson.PatientNormalizedFullNameIndexName);

        builder.HasOne(person => person.Patient)
            .WithMany(patient => patient.ResponsiblePersons)
            .HasForeignKey(person => person.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
