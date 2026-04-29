using InsureTrust.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.PaymentService.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.PaymentNumber).IsUnique();

                entity.Property(x => x.PaymentNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.UserId)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.Property(x => x.PaymentMethod)
                    .HasMaxLength(50);

                entity.Property(x => x.TransactionId)
                    .HasMaxLength(100);

                entity.Property(x => x.PaymentDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(x => x.Remarks)
                    .HasMaxLength(500);
            });
        }
    }
} 