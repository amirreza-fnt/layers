using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LayerManager.API.Models;

namespace LayerManager.API.Data.Configurations;

public class CategoryHierarchyConfiguration : IEntityTypeConfiguration<CategoryHierarchy>
{
    public void Configure(EntityTypeBuilder<CategoryHierarchy> builder)
    {
        builder.HasIndex(e => e.CategoryId).IsUnique();
        builder.HasIndex(e => e.ParentCategoryId);

        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentCategory)
            .WithMany()
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
