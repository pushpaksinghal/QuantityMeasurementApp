using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.RepositoryLayer.Records;
using System.Reflection.Emit;

namespace QuantityMeasurementApp.RepositoryLayer.Context
{
    public class QuantityDbContext : DbContext
    {
        public QuantityDbContext(DbContextOptions<QuantityDbContext> options)
            : base(options)
        {
        }

        public DbSet<QuantityHistoryRecord> QuantityHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuantityHistoryRecord>(entity =>
            {
                entity.ToTable("QuantityHistory");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(x => x.Category)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.OperationType)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.FirstValue)
                      .IsRequired();

                entity.Property(x => x.FirstUnit)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(x => x.SecondValue)
                      .IsRequired(false);

                entity.Property(x => x.SecondUnit)
                      .HasMaxLength(50)
                      .IsRequired(false);

                entity.Property(x => x.TargetUnit)
                      .HasMaxLength(50)
                      .IsRequired(false);

                entity.Property(x => x.ResultValue)
                      .IsRequired();

                entity.Property(x => x.ResultUnit)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(x => x.CreatedAt)
                      .IsRequired();
            });
        }
    }
}