using LayerManager.API.DTOs.Guide;
using LayerManager.API.Repositories.Interfaces;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Services;

public class GuideService : IGuideService
{
    private readonly IGuideRepository _repo;

    public GuideService(IGuideRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<GuideDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<GuideDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<Guid> CreateAsync(CreateGuideDto dto)
    {
        var guide = new Models.MapGuide
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Icon = dto.Icon,
            MapIcon = dto.MapIcon,
            SortOrder = dto.SortOrder,
            IsActive = dto.IsActive
        };

        await _repo.CreateAsync(guide);
        return guide.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateGuideDto dto)
    {
        var guide = await _repo.GetByIdAsync(id);
        if (guide is null) return false;

        guide.Title = dto.Title;
        guide.Description = dto.Description;
        guide.ImageUrl = dto.ImageUrl;
        guide.Icon = dto.Icon;
        guide.MapIcon = dto.MapIcon;
        guide.SortOrder = dto.SortOrder;
        guide.IsActive = dto.IsActive;

        await _repo.UpdateAsync(guide);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var guide = await _repo.GetByIdAsync(id);
        if (guide is null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }

    private static GuideDto MapToDto(Models.MapGuide g) => new()
    {
        Id = g.Id,
        Title = g.Title,
        Description = g.Description,
        ImageUrl = g.ImageUrl,
        Icon = g.Icon,
        MapIcon = g.MapIcon,
        SortOrder = g.SortOrder,
        IsActive = g.IsActive,
        CreatedAt = g.CreatedAt
    };
}