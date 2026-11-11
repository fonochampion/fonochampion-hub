using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class ProfessionalContactConfiguration : IEntityTypeConfiguration<ProfessionalContact>
{
    public void Configure(EntityTypeBuilder<ProfessionalContact> builder)
    {
        builder.ToTable("ProfessionalContacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Type)
            .IsRequired();

        builder.Property(contact => contact.Value)
            .IsRequired();

        builder.HasOne(contact => contact.Professional)
            .WithMany(professional => professional.Contacts)
            .HasForeignKey(contact => contact.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
