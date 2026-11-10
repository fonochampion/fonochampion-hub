using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class Cid10CatalogVersionConfiguration : IEntityTypeConfiguration<Cid10CatalogVersion>
{
    public void Configure(EntityTypeBuilder<Cid10CatalogVersion> builder)
    {
        builder.ToTable("Cid10CatalogVersions");

        builder.HasKey(version => version.Fingerprint);

        builder.Property(version => version.Fingerprint)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(version => version.ImportedAtUtc)
            .IsRequired();

        builder.HasIndex(version => version.IsCurrent)
            .IsUnique()
            .HasFilter("\"IsCurrent\" = TRUE");
    }
}
