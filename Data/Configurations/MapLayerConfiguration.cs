using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LayerManager.API.Models;

namespace LayerManager.API.Data.Configurations;

public class MapLayerConfiguration : IEntityTypeConfiguration<MapLayer>
{
    public void Configure(EntityTypeBuilder<MapLayer> builder)
    {
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.SortOrder);

        builder.Property(e => e.Slug).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IconName).HasMaxLength(100);
        builder.Property(e => e.Color).HasMaxLength(20);
        builder.Property(e => e.ComponentName).HasMaxLength(100);
    }
}
