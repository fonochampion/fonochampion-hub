using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("Professionals");

        builder.HasKey(professional => professional.Id);

        builder.Property(professional => professional.FullName)
            .IsRequired();

        builder.Property(professional => professional.Cpf)
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(professional => professional.Cpf)
            .IsUnique()
            .HasDatabaseName(Professional.CpfUniqueIndexName);
    }
}
