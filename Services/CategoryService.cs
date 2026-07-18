using LayerManager.API.DTOs.Guide;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CategoryTreeNodeDto>> GetTreeAsync()
    {
        var categories = await _repo.GetActiveAsync();
        var categoryMap = categories.ToDictionary(c => c.Id);

        var roots = categories
            .Where(c => c.ParentId is null)
            .Select(c => BuildNode(c, categoryMap))
            .OrderBy(n => n.SortOrder)
            .ToList();

        return roots;
    }

    public async Task<List<CategoryTreeNodeDto>> GetFlatTreeAsync()
    {
        var categories = await _repo.GetActiveAsync();

        return categories.Select(c => new CategoryTreeNodeDto
        {
            Id = c.Id,
            Name = c.Name,
            Color = c.Color,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            Children = new List<CategoryTreeNodeDto>(),
            Guides = new List<GuideDto>()
        }).ToList();
    }

    public async Task<bool> SetParentAsync(Guid categoryId, Guid? parentCategoryId)
    {
        var category = await _repo.GetByIdAsync(categoryId);
        if (category is null) return false;

        if (parentCategoryId.HasValue)
        {
            var parent = await _repo.GetByIdAsync(parentCategoryId.Value);
            if (parent is null) return false;
        }

        category.ParentId = parentCategoryId;
        await _repo.UpdateAsync(category);
        return true;
    }

    public async Task<bool> SyncCategoriesAsync()
    {
        var existing = await _repo.GetAllAsync();
        var existingNames = existing.Select(c => c.Name).ToHashSet();

        var desired = new[]
        {
            ("پرداخت عوارض", "#3B82F6", 1),
            ("موکب‌ها", "#8B5CF6", 2),
            ("کوچه‌ها", "#10B981", 3),
            ("پارک‌ها", "#33FF57", 4),
            ("مراکز خرید", "#3357FF", 5),
            ("مساجد", "#FF33F5", 6),
            ("بیمارستان‌ها", "#FF3333", 7),
            ("مدارس", "#33FFF5", 8),
            ("اماکن ورزشی", "#F5FF33", 9),
            ("سایر", "#808080", 10),
        };

        foreach (var (name, color, sortOrder) in desired)
        {
            if (existingNames.Contains(name)) continue;

            var category = new Category
            {
                Name = name,
                Color = color,
                SortOrder = sortOrder,
                IsActive = true
            };
            await _repo.CreateAsync(category);
            existingNames.Add(name);
        }

        return true;
    }

    private static CategoryTreeNodeDto BuildNode(
        Category cat,
        Dictionary<Guid, Category> categoryMap)
    {
        var children = categoryMap.Values
            .Where(c => c.ParentId == cat.Id)
            .OrderBy(c => c.SortOrder)
            .Select(c => BuildNode(c, categoryMap))
            .ToList();

        return new CategoryTreeNodeDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Color = cat.Color,
            SortOrder = cat.SortOrder,
            IsActive = cat.IsActive,
            ComponentName = GetComponentName(cat.Name),
            Children = children,
            Guides = new List<GuideDto>()
        };
    }

    private static string? GetComponentName(string categoryName) => categoryName switch
    {
        "پرداخت عوارض" => "NosaziLayer",
        "موکب‌ها" => "MokebLayer",
        "کوچه‌ها" => "KoocheLayer",
        _ => null
    };
}