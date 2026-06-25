using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Models;

namespace PawdoptApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<PetListing>          PetListings          { get; set; }
    public DbSet<AdoptionApplication> AdoptionApplications { get; set; }
    public DbSet<Message>             Messages             { get; set; }
    public DbSet<Notification>        Notifications        { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // PetListing → Rehomer (restrict delete so orphaned listings don't cascade-delete users)
        builder.Entity<PetListing>()
            .HasOne(l => l.Rehomer)
            .WithMany()
            .HasForeignKey(l => l.RehomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // AdoptionApplication → Adopter
        builder.Entity<AdoptionApplication>()
            .HasOne(a => a.Adopter)
            .WithMany()
            .HasForeignKey(a => a.AdopterId)
            .OnDelete(DeleteBehavior.Restrict);

        // AdoptionApplication → PetListing (required; cascade so listing deletion removes applications)
        builder.Entity<AdoptionApplication>()
            .HasOne(a => a.PetListing)
            .WithMany(l => l.Applications)
            .HasForeignKey(a => a.PetListingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Message → Sender (restrict to prevent cascade cycles)
        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message → Receiver
        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message → RelatedListing (optional; set null on listing delete)
        builder.Entity<Message>()
            .HasOne(m => m.RelatedListing)
            .WithMany(l => l.Messages)
            .HasForeignKey(m => m.RelatedListingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Notification → User
        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
