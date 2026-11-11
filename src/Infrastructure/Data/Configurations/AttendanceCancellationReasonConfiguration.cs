using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class AttendanceCancellationReasonConfiguration
    : IEntityTypeConfiguration<AttendanceCancellationReason>
{
    public void Configure(EntityTypeBuilder<AttendanceCancellationReason> builder)
    {
        builder.ToTable("AttendanceCancellationReasons");

        builder.HasKey(reason => reason.Id);

        builder.Property(reason => reason.Name)
            .IsRequired();

        builder.Property(reason => reason.NormalizedName)
            .IsRequired();

        builder.Property(reason => reason.Description);

        builder.Property(reason => reason.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(reason => reason.NormalizedName)
            .IsUnique()
            .HasDatabaseName(AttendanceCancellationReason.NormalizedNameIndexName);
    }
}
