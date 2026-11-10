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
