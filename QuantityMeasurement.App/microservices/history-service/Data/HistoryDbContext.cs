using Microsoft.EntityFrameworkCore;
using HistoryService.Models;

namespace HistoryService.Data;

public class HistoryDbContext : DbContext
{
    public HistoryDbContext(DbContextOptions<HistoryDbContext> options) : base(options) { }

    public DbSet<QuantityHistoryEntity> QuantityHistory => Set<QuantityHistoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QuantityHistoryEntity>(e =>
        {
            e.HasIndex(x => x.Category);
            e.HasIndex(x => x.CreatedAt);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
