using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LayerManager.API.Models;

namespace LayerManager.API.Data.Configurations;

public class MapGuideConfiguration : IEntityTypeConfiguration<MapGuide>
{
    public void Configure(EntityTypeBuilder<MapGuide> builder)
    {
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.SortOrder);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Icon).HasMaxLength(100);
    }
}