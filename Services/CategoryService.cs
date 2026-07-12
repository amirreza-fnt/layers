using LayerManager.API.DTOs.Guide;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IGuideRepository _guideRepo;

    public CategoryService(
        ICategoryRepository repo,
        IGuideRepository guideRepo)
    {
        _repo = repo;
        _guideRepo = guideRepo;
    }

    public async Task<IEnumerable<CategoryTreeNodeDto>> GetTreeAsync()
    {
        var categories = await _repo.GetActiveAsync();
        var hierarchies = await _repo.GetHierarchiesAsync();
        var guides = await _guideRepo.GetAllAsync();

        var categoryMap = categories.ToDictionary(c => c.Id);
        var hierarchyMap = hierarchies.ToDictionary(h => h.CategoryId);
        var guideMap = guides.GroupBy(g => g.CategoryId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var roots = new List<CategoryTreeNodeDto>();

        foreach (var cat in categories)
        {
            if (!hierarchyMap.TryGetValue(cat.Id, out var hier) || hier.ParentCategoryId is null)
            {
                roots.Add(BuildNode(cat, categoryMap, hierarchyMap, guideMap));
            }
        }

        return roots.OrderBy(r => r.SortOrder).ToList();
    }

    public async Task<List<CategoryTreeNodeDto>> GetFlatTreeAsync()
    {
        var categories = await _repo.GetActiveAsync();
        var hierarchies = await _repo.GetHierarchiesAsync();
        var hierarchyMap = hierarchies.ToDictionary(h => h.CategoryId);

        return categories.Select(c => new CategoryTreeNodeDto
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon,
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

        var existing = await _repo.GetHierarchyByCategoryIdAsync(categoryId);
        if (existing is not null)
        {
            existing.ParentCategoryId = parentCategoryId;
            await _repo.UpdateHierarchyAsync(existing);
        }
        else
        {
            var hierarchy = new CategoryHierarchy
            {
                CategoryId = categoryId,
                ParentCategoryId = parentCategoryId
            };
            await _repo.CreateHierarchyAsync(hierarchy);
        }

        return true;
    }

    public async Task<bool> SyncCategoriesAsync()
    {
        var existing = await _repo.GetAllAsync();
        var existingNames = existing.Select(c => c.Name).ToHashSet();

        var desired = new[]
        {
            ("پرداخت عوارض", "CreditCard", "#3B82F6", 1),
            ("موکب‌ها", "MapPin", "#8B5CF6", 2),
            ("کوچه‌ها", "Route", "#10B981", 3),
            ("پارک‌ها", "tree", "#33FF57", 4),
            ("مراکز خرید", "shopping-cart", "#3357FF", 5),
            ("مساجد", "mosque", "#FF33F5", 6),
            ("بیمارستان‌ها", "hospital", "#FF3333", 7),
            ("مدارس", "school", "#33FFF5", 8),
            ("اماکن ورزشی", "sports", "#F5FF33", 9),
            ("سایر", "map-pin", "#808080", 10),
        };

        foreach (var (name, icon, color, sortOrder) in desired)
        {
            if (existingNames.Contains(name)) continue;

            var category = new Category
            {
                Name = name,
                Icon = icon,
                Color = color,
                SortOrder = sortOrder,
                IsActive = true
            };
            await _repo.CreateAsync(category);
            existingNames.Add(name);
        }

        return true;
    }

    private CategoryTreeNodeDto BuildNode(
        Category cat,
        Dictionary<Guid, Category> categoryMap,
        Dictionary<Guid, CategoryHierarchy> hierarchyMap,
        Dictionary<Guid, List<MapGuide>> guideMap)
    {
        var node = new CategoryTreeNodeDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Icon = cat.Icon,
            Color = cat.Color,
            SortOrder = cat.SortOrder,
            IsActive = cat.IsActive,
            ComponentName = GetComponentName(cat.Name),
            Guides = guideMap.TryGetValue(cat.Id, out var guides)
                ? guides.Select(g => new GuideDto
                {
                    Id = g.Id,
                    CategoryId = g.CategoryId,
                    CategoryName = g.Category?.Name ?? cat.Name,
                    Title = g.Title,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    Icon = g.Icon,
                    SortOrder = g.SortOrder,
                    IsActive = g.IsActive,
                    CreatedAt = g.CreatedAt
                }).ToList()
                : new List<GuideDto>()
        };

        foreach (var kv in hierarchyMap)
        {
            if (kv.Value.ParentCategoryId == cat.Id)
            {
                if (categoryMap.TryGetValue(kv.Key, out var childCat))
                {
                    node.Children.Add(BuildNode(childCat, categoryMap, hierarchyMap, guideMap));
                }
            }
        }

        node.Children = node.Children.OrderBy(c => c.SortOrder).ToList();
        return node;
    }

    private static string? GetComponentName(string categoryName) => categoryName switch
    {
        "پرداخت عوارض" => "NosaziLayer",
        "موکب‌ها" => "MokebLayer",
        "کوچه‌ها" => "KoocheLayer",
        _ => null
    };
}
