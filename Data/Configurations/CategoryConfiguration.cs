using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LayerManager.API.Models;

namespace LayerManager.API.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.SortOrder);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
    }
}
