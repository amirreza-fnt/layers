using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LayerManager.API.Models;

[Table("CategoryHierarchies")]
public class CategoryHierarchy
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CategoryId { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public int SortOrder { get; set; } = 0;

    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey(nameof(ParentCategoryId))]
    public virtual Category? ParentCategory { get; set; }
}
