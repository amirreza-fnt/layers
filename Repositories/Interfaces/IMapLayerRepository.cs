using LayerManager.API.Models;

namespace LayerManager.API.Repositories.Interfaces;

public interface IMapLayerRepository
{
    Task<MapLayer?> GetByIdAsync(Guid id);
    Task<MapLayer?> GetBySlugAsync(string slug);
    Task<IEnumerable<MapLayer>> GetActiveAsync();
    Task<IEnumerable<MapLayer>> GetAllAsync();
    Task<MapLayer> CreateAsync(MapLayer layer);
    Task UpdateAsync(MapLayer layer);
    Task DeleteAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<int> GetMaxSortOrderAsync();
}
