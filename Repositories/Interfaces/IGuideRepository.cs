using LayerManager.API.Models;

namespace LayerManager.API.Repositories.Interfaces;

public interface IGuideRepository
{
    Task<IEnumerable<MapGuide>> GetAllAsync();
    Task<IEnumerable<MapGuide>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<MapGuide>> GetActiveByCategoryIdAsync(Guid categoryId);
    Task<MapGuide?> GetByIdAsync(Guid id);
    Task<MapGuide> CreateAsync(MapGuide guide);
    Task UpdateAsync(MapGuide guide);
    Task DeleteAsync(Guid id);
}
