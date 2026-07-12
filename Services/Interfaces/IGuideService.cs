using LayerManager.API.DTOs.Guide;

namespace LayerManager.API.Services.Interfaces;

public interface IGuideService
{
    Task<IEnumerable<GuideDto>> GetAllAsync();
    Task<IEnumerable<GuideDto>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<GuideDto>> GetActiveByCategoryIdAsync(Guid categoryId);
    Task<GuideDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateGuideDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateGuideDto dto);
    Task<bool> DeleteAsync(Guid id);
}
