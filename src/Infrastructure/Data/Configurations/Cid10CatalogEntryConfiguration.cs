using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class Cid10CatalogEntryConfiguration : IEntityTypeConfiguration<Cid10CatalogEntry>
{
    public void Configure(EntityTypeBuilder<Cid10CatalogEntry> builder)
    {
        builder.ToTable("Cid10CatalogEntries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.VersionFingerprint)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entry => entry.Code)
            .HasMaxLength(8);

        builder.Property(entry => entry.CodeSearchKey)
            .HasMaxLength(8);

        builder.Property(entry => entry.CodeRangeStart)
            .HasMaxLength(3);

        builder.Property(entry => entry.CodeRangeEnd)
            .HasMaxLength(3);

        builder.Property(entry => entry.Description)
            .IsRequired();

        builder.Property(entry => entry.SearchText)
            .IsRequired();

        builder.HasOne(entry => entry.Version)
            .WithMany(version => version.Entries)
            .HasForeignKey(entry => entry.VersionFingerprint)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasAlternateKey(entry => new { entry.VersionFingerprint, entry.Id });

        builder.HasOne(entry => entry.Parent)
            .WithMany(entry => entry.Children)
            .HasForeignKey(entry => new { entry.VersionFingerprint, entry.ParentId })
            .HasPrincipalKey(entry => new { entry.VersionFingerprint, entry.Id })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entry => new { entry.VersionFingerprint, entry.CodeSearchKey })
            .IsUnique()
            .HasFilter("\"CodeSearchKey\" IS NOT NULL");

        builder.HasIndex(entry => new { entry.VersionFingerprint, entry.ParentId });
    }
}
