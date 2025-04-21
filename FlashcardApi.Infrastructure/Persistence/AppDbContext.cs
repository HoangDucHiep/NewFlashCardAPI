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
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Folder -> User
        builder.Entity<Folder>()
            .HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa user khi xóa folder

        // Desk -> User
        builder.Entity<Desk>()
            .HasOne(d => d.Owner)
            .WithMany()
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa user khi xóa desk

        // Folder relationships
        builder.Entity<Folder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.SubFolders)
            .HasForeignKey(f => f.ParentFolderId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp cascade

        // Desk -> Folder
        builder.Entity<Desk>()
            .HasOne(d => d.Folder)
            .WithMany(f => f.Desks)
            .HasForeignKey(d => d.FolderId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa folder sẽ xóa desks

        // Card -> Desk
        builder.Entity<Card>()
            .HasOne(c => c.Desk)
            .WithMany(d => d.Cards)
            .HasForeignKey(c => c.DeskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review -> Card
        builder.Entity<Review>()
            .HasOne(r => r.Card)
            .WithOne(c => c.Review)
            .HasForeignKey<Review>(r => r.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Session -> Desk
        builder.Entity<Session>()
            .HasOne(s => s.Desk)
            .WithMany(d => d.Sessions)
            .HasForeignKey(s => s.DeskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Image -> User
        builder.Entity<Image>()
            .HasOne(i => i.UploadedByUser)
            .WithMany()
            .HasForeignKey(i => i.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa user khi xóa image
    }
}