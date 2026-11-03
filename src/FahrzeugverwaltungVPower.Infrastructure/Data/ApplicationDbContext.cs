using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Infrastructure.Data;

/// <summary>
/// Der Datenbankkontext für die Fahrzeugverwaltung.
/// </summary>
/// <param name="options"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    /// <summary>
    /// Speichert die Fahrzeuge.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            // Alle anderen Props werden sinnvoll gesetzt. 
            // Nur FirstName und LastName wären nvarchar(max) ohne manuelle Limitierung
            entity.Property(u => u.FirstName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.LastName)
                  .IsRequired()
                  .HasMaxLength(100);

        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles");

            // Definiert feste IDs für die Rollen. Diese dürfen nachträglich nicht mehr verändert werden.
            entity.HasData(
                CreateRole("c5e3271f-fd43-4043-8a26-c151e7074ef3", RoleNames.Admin),
                CreateRole("e2b10d79-a11e-4f27-9082-b7af10aaa992", RoleNames.Creator),
                CreateRole("40a39e45-899b-4825-b600-2ec65fbc3a8e", RoleNames.Editor),
                CreateRole("136482b2-0fb8-45b6-be4d-5622c06de355", RoleNames.Viewer));
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>()
                    .ToTable("UserRoles");

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicles");

            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id)
                  .ValueGeneratedNever();

            entity.Property(v => v.LicensePlate)
                  .IsRequired()
                  .HasMaxLength(11);
            entity.HasIndex(v => v.LicensePlate);

            entity.Property(v => v.FirstRegistrationDate)
                  .IsRequired();

            // Länge frei gewählt, es ist keine Maximallänge angegeben
            entity.Property(v => v.Holder)
                  .IsRequired()
                  .HasMaxLength(200);

            // Länge frei gewählt, es ist keine Maximallänge angegeben
            entity.Property(v => v.HolderFirstName)
                  .HasMaxLength(200);

            entity.Property(v => v.Manufacturer).IsRequired()
                  .IsRequired()
                  .HasMaxLength(25);

            entity.Property(v => v.TypeDesignation).IsRequired()
                  .IsRequired()
                  .HasMaxLength(25);

            entity.Property(v => v.Model)
                  .IsRequired()
                  .HasMaxLength(25);

            entity.Property(v => v.VIN)
                  .IsRequired()
                  .HasMaxLength(17);
            entity.HasIndex(v => v.VIN)
                  .IsUnique();

            entity.Property(v => v.InspectionValidUntil)
                  .IsRequired();

            entity.Property(v => v.HSN)
                  .IsRequired()
                  .HasMaxLength(4);

            entity.Property(v => v.TSN)
                  .IsRequired()
                  .HasMaxLength(9);
        });
    }

    private static ApplicationRole CreateRole(string id, string name) => new()
    {
        Id = Guid.Parse(id),
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = id
    };
}
