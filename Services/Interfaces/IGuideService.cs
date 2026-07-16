using LayerManager.API.DTOs.Guide;

namespace LayerManager.API.Services.Interfaces;

public interface IGuideService
{
    Task<IEnumerable<GuideDto>> GetAllAsync();
    Task<GuideDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateGuideDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateGuideDto dto);
    Task<bool> DeleteAsync(Guid id);
}