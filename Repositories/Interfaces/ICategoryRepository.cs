using LayerManager.API.Models;

namespace LayerManager.API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByExactNameAsync(string name);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
}