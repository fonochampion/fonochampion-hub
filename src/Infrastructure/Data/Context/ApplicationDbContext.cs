using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Infrastructure.Data.Context;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Cid10CatalogVersion> Cid10CatalogVersions => Set<Cid10CatalogVersion>();

    public DbSet<Cid10CatalogEntry> Cid10CatalogEntries => Set<Cid10CatalogEntry>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<ResponsiblePerson> ResponsiblePersons => Set<ResponsiblePerson>();

    public DbSet<ResponsiblePersonContact> ResponsiblePersonContacts =>
        Set<ResponsiblePersonContact>();

    public DbSet<Professional> Professionals => Set<Professional>();

    public DbSet<ProfessionalCertification> ProfessionalCertifications =>
        Set<ProfessionalCertification>();

    public DbSet<ProfessionalContact> ProfessionalContacts => Set<ProfessionalContact>();

    public DbSet<AttendanceCancellationReason> AttendanceCancellationReasons =>
        Set<AttendanceCancellationReason>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<ApplicationUser>()
            .Property(user => user.IsActive)
            .HasDefaultValue(true);

        builder.Entity<ApplicationUser>()
            .Property(user => user.MustChangePassword)
            .HasDefaultValue(false);
    }
}
