using Microsoft.EntityFrameworkCore;
using InsureTrust.ClaimService.Models;

namespace InsureTrust.ClaimService.Data
{
    public class ClaimDbContext : DbContext
    {
        public ClaimDbContext(DbContextOptions<ClaimDbContext> options)
            : base(options) { }

        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Claim Fluent API Configurations
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UserPolicyId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ClaimStatus).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.MaturityAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AdminRemarks).HasMaxLength(500);
            });
        }
    }
}