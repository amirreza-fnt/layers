using Microsoft.EntityFrameworkCore;
using LayerManager.API.Data;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;

namespace LayerManager.API.Repositories;

public class MapLayerRepository : IMapLayerRepository
{
    private readonly AppDbContext _context;

    public MapLayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MapLayer?> GetByIdAsync(Guid id)
    {
        return await _context.MapLayers.FindAsync(id);
    }

    public async Task<MapLayer?> GetBySlugAsync(string slug)
    {
        return await _context.MapLayers.FirstOrDefaultAsync(l => l.Slug == slug);
    }

    public async Task<IEnumerable<MapLayer>> GetActiveAsync()
    {
        return await _context.MapLayers
            .Where(l => l.IsActive)
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<MapLayer>> GetAllAsync()
    {
        return await _context.MapLayers
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<MapLayer> CreateAsync(MapLayer layer)
    {
        _context.MapLayers.Add(layer);
        return Task.FromResult(layer);
    }

    public Task UpdateAsync(MapLayer layer)
    {
        layer.UpdatedAt = DateTime.UtcNow;
        _context.MapLayers.Update(layer);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var layer = await _context.MapLayers.FindAsync(id);
        if (layer != null)
            _context.MapLayers.Remove(layer);
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _context.MapLayers.AnyAsync(l => l.Slug == slug && l.Id != excludeId.Value);
        }
        return await _context.MapLayers.AnyAsync(l => l.Slug == slug);
    }

    public async Task<int> GetMaxSortOrderAsync()
    {
        if (!await _context.MapLayers.AnyAsync()) return 0;
        return await _context.MapLayers.MaxAsync(l => l.SortOrder);
    }
}
