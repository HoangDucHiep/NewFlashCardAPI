using FlashcardApi.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Desk> Desks { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<RevokedToken> RevokedTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Desk>().HasOne<ApplicationUser>().WithMany().HasForeignKey(d => d.OwnerId);
        builder.Entity<Card>().HasOne<Desk>().WithMany().HasForeignKey(c => c.DeskId);
        builder.Entity<Folder>().HasOne<ApplicationUser>().WithMany().HasForeignKey(f => f.OwnerId);
        builder.Entity<Review>().HasOne<Card>().WithMany().HasForeignKey(r => r.CardId);
        builder.Entity<Session>().HasOne<Desk>().WithMany().HasForeignKey(s => s.DeskId);

        builder.Entity<Session>()
            .HasOne<Desk>()
            .WithMany()
            .HasForeignKey(s => s.DeskId);

        builder
            .Entity<Folder>()
            .HasOne<Folder>()
            .WithMany()
            .HasForeignKey(f => f.ParentFolderId)
            .IsRequired(false);
    }
}
