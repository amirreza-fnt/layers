using LayerManager.API.Models;

namespace LayerManager.API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByExactNameAsync(string name);
    Task<Category> CreateAsync(Category category);
    Task<IEnumerable<CategoryHierarchy>> GetHierarchiesAsync();
    Task<CategoryHierarchy?> GetHierarchyByCategoryIdAsync(Guid categoryId);
    Task<CategoryHierarchy> CreateHierarchyAsync(CategoryHierarchy hierarchy);
    Task UpdateHierarchyAsync(CategoryHierarchy hierarchy);
    Task DeleteHierarchyAsync(Guid id);
}
