using InsureTrust.QueryService.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InsureTrust.QueryService.Data
{
    public class SupportDbContext : DbContext
    {
        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options)
        {
        }

        public DbSet<SupportQuery> SupportQueries => Set<SupportQuery>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SupportQuery>(entity =>
            {
                entity.HasIndex(x => x.TicketNumber).IsUnique();

                entity.Property(x => x.TicketNumber)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(x => x.Subject)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.Description)
                      .HasMaxLength(2000)
                      .IsRequired();

                entity.Property(x => x.Status)
                      .HasMaxLength(20)
                      .HasDefaultValue("Pending")
                      .IsRequired();

                entity.Property(x => x.AttachmentPath)
                      .HasMaxLength(500);

                entity.Property(x => x.AdminResponse)
                      .HasMaxLength(1000);

               
            });
        }
    }
}