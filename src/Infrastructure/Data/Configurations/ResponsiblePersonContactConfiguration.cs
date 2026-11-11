using FonoChampionHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonoChampionHub.Infrastructure.Data.Configurations;

public sealed class ResponsiblePersonContactConfiguration : IEntityTypeConfiguration<ResponsiblePersonContact>
{
    public void Configure(EntityTypeBuilder<ResponsiblePersonContact> builder)
    {
        builder.ToTable("ResponsiblePersonContacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Type)
            .IsRequired();

        builder.Property(contact => contact.Value)
            .IsRequired();

        builder.HasOne(contact => contact.ResponsiblePerson)
            .WithMany(person => person.Contacts)
            .HasForeignKey(contact => contact.ResponsiblePersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
