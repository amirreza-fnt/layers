using Microsoft.EntityFrameworkCore;
using LayerManager.API.Data;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;

namespace LayerManager.API.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category?> GetByExactNameAsync(string name)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<IEnumerable<CategoryHierarchy>> GetHierarchiesAsync()
    {
        return await _context.CategoryHierarchies
            .Include(h => h.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CategoryHierarchy?> GetHierarchyByCategoryIdAsync(Guid categoryId)
    {
        return await _context.CategoryHierarchies
            .Include(h => h.Category)
            .FirstOrDefaultAsync(h => h.CategoryId == categoryId);
    }

    public async Task<CategoryHierarchy> CreateHierarchyAsync(CategoryHierarchy hierarchy)
    {
        _context.CategoryHierarchies.Add(hierarchy);
        await _context.SaveChangesAsync();
        return hierarchy;
    }

    public async Task UpdateHierarchyAsync(CategoryHierarchy hierarchy)
    {
        _context.CategoryHierarchies.Update(hierarchy);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteHierarchyAsync(Guid id)
    {
        var entity = await _context.CategoryHierarchies.FindAsync(id);
        if (entity is not null)
        {
            _context.CategoryHierarchies.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
