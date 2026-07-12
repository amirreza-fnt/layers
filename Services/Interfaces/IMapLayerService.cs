using LayerManager.API.DTOs.MapLayer;

namespace LayerManager.API.Services.Interfaces;

public interface IMapLayerService
{
    Task<IEnumerable<MapLayerDto>> GetActiveAsync();
    Task<IEnumerable<MapLayerDto>> GetAllAsync();
    Task<MapLayerDto?> GetByIdAsync(Guid id);
    Task<MapLayerDto?> GetBySlugAsync(string slug);
    Task<Guid> CreateAsync(CreateMapLayerDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateMapLayerDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ReorderAsync(Guid id, int newOrder);
}
