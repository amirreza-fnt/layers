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
        builder.HasIndex(e => e.ParentId);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}