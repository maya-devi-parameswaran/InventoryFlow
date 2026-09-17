using InventoryFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryFlow.Infrastructure.Persistence.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.QuantityOnHand).IsRequired();
        builder.Property(s => s.QuantityReserved).IsRequired();
        builder.Property(s => s.ReorderThreshold).IsRequired();

        // Optimistic concurrency token — this is the key line for Phase 4's
        // "prevent overselling under concurrent load" scenario
        builder.Property(s => s.Version)
            .IsRowVersion();

        builder.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}