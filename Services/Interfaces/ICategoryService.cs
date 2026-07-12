using LayerManager.API.DTOs.Guide;

namespace LayerManager.API.Services.Interfaces;

public class CategoryTreeNodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ComponentName { get; set; }
    public List<CategoryTreeNodeDto> Children { get; set; } = new();
    public List<GuideDto> Guides { get; set; } = new();
}

public interface ICategoryService
{
    Task<IEnumerable<CategoryTreeNodeDto>> GetTreeAsync();
    Task<List<CategoryTreeNodeDto>> GetFlatTreeAsync();
    Task<bool> SetParentAsync(Guid categoryId, Guid? parentCategoryId);
    Task<bool> SyncCategoriesAsync();
}
