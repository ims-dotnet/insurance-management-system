using InsureTrust.IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.IdentityService.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UserNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.HasIndex(e => e.UserNumber).IsUnique();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNo).IsRequired().HasMaxLength(15);
                entity.Property(e => e.PanCard).IsRequired().HasMaxLength(20);
                entity.Property(e => e.KycDocumentPath).HasMaxLength(500);
                
                entity.Property(e => e.KycStatus).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("Customer");
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ColorCode).IsRequired().HasMaxLength(20).HasDefaultValue("Blue");
                entity.Property(e => e.IsRead).HasDefaultValue(false);
                entity.Property(e => e.RelatedFeature).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
