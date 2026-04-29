using Microsoft.EntityFrameworkCore;
using InsureTrust.ProductService.Models;

namespace InsureTrust.ProductService.Data
{
    public class InsureTrustProductServiceContext : DbContext
    {
        public InsureTrustProductServiceContext(
            DbContextOptions<InsureTrustProductServiceContext> options)
            : base(options)
        {
        }

        // ================= TABLES =================

        public DbSet<PolicyType> PolicyTypes { get; set; } = default!;
        public DbSet<UserPolicy> UserPolicies { get; set; } = default!;
        public DbSet<PolicyTerm> PolicyTerms { get; set; } = default!;
        public DbSet<PolicyRequiredField> PolicyRequiredFields { get; set; } = default!;

        // ================= MODEL CONFIG =================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------- PolicyType --------
            modelBuilder.Entity<PolicyType>(entity =>
            {
                entity.ToTable("PolicyTypes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.Icon)
                    .HasMaxLength(50);

                entity.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                // Relationships
                entity.HasMany(x => x.Terms)
                    .WithOne(t => t.PolicyType)
                    .HasForeignKey(t => t.PolicyTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.RequiredFields)
                    .WithOne(f => f.PolicyType)
                    .HasForeignKey(f => f.PolicyTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.UserPolicies)
                    .WithOne(u => u.PolicyType)
                    .HasForeignKey(u => u.PolicyTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // -------- UserPolicy --------
            modelBuilder.Entity<UserPolicy>(entity =>
            {
                entity.ToTable("UserPolicies");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.PolicyNumber)
                    .HasMaxLength(20);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");

                entity.Property(x => x.PackageAmount)
                    .HasPrecision(18, 2);

                entity.Property(x => x.AdminRemarks)
                    .HasMaxLength(500);

                entity.Property(x => x.DynamicFieldsJson)
                    .HasColumnType("nvarchar(max)");

                // Indexes (IMPORTANT 🚀)
                entity.HasIndex(x => x.PolicyNumber)
                    .IsUnique();

                entity.HasIndex(x => x.UserId);

                entity.HasIndex(x => x.PolicyTypeId);
            });

            // -------- PolicyTerm --------
            modelBuilder.Entity<PolicyTerm>(entity =>
            {
                entity.ToTable("PolicyTerms");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TermText)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.DisplayOrder)
                    .HasDefaultValue(0);
            });

            // -------- PolicyRequiredField --------
            modelBuilder.Entity<PolicyRequiredField>(entity =>
            {
                entity.ToTable("PolicyRequiredFields");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FieldName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.FieldType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Placeholder)
                    .HasMaxLength(200);

                entity.Property(x => x.DisplayOrder)
                    .HasDefaultValue(0);
            });
        }
    }
}