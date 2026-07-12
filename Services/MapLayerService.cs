using LayerManager.API.DTOs.MapLayer;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Services;

public class MapLayerService : IMapLayerService
{
    private readonly IMapLayerRepository _repo;

    public MapLayerService(IMapLayerRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<MapLayerDto>> GetActiveAsync()
    {
        var layers = await _repo.GetActiveAsync();
        return layers.Select(MapToDto);
    }

    public async Task<IEnumerable<MapLayerDto>> GetAllAsync()
    {
        var layers = await _repo.GetAllAsync();
        return layers.Select(MapToDto);
    }

    public async Task<MapLayerDto?> GetByIdAsync(Guid id)
    {
        var layer = await _repo.GetByIdAsync(id);
        return layer is null ? null : MapToDto(layer);
    }

    public async Task<MapLayerDto?> GetBySlugAsync(string slug)
    {
        var layer = await _repo.GetBySlugAsync(slug);
        return layer is null ? null : MapToDto(layer);
    }

    public async Task<Guid> CreateAsync(CreateMapLayerDto dto)
    {
        if (await _repo.SlugExistsAsync(dto.Slug))
            throw new InvalidOperationException("شناسه لایه تکراری است");

        var maxOrder = await _repo.GetMaxSortOrderAsync();

        var layer = new MapLayer
        {
            Slug = dto.Slug,
            Name = dto.Name,
            Description = dto.Description,
            IconName = dto.IconName,
            Color = dto.Color,
            SortOrder = dto.SortOrder > 0 ? dto.SortOrder : maxOrder + 1,
            IsActive = dto.IsActive,
            ComponentName = dto.ComponentName
        };

        await _repo.CreateAsync(layer);
        return layer.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMapLayerDto dto)
    {
        var layer = await _repo.GetByIdAsync(id);
        if (layer is null) return false;

        layer.Name = dto.Name;
        layer.Description = dto.Description;
        layer.IconName = dto.IconName;
        layer.Color = dto.Color;
        layer.SortOrder = dto.SortOrder;
        layer.IsActive = dto.IsActive;
        layer.ComponentName = dto.ComponentName;

        await _repo.UpdateAsync(layer);
        return true;
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var layer = await _repo.GetByIdAsync(id);
        if (layer is null) return false;

        layer.IsActive = !layer.IsActive;
        await _repo.UpdateAsync(layer);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var layer = await _repo.GetByIdAsync(id);
        if (layer is null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ReorderAsync(Guid id, int newOrder)
    {
        var layer = await _repo.GetByIdAsync(id);
        if (layer is null) return false;

        layer.SortOrder = newOrder;
        await _repo.UpdateAsync(layer);
        return true;
    }

    private static MapLayerDto MapToDto(MapLayer layer)
    {
        return new MapLayerDto
        {
            Id = layer.Id,
            Slug = layer.Slug,
            Name = layer.Name,
            Description = layer.Description,
            IconName = layer.IconName,
            Color = layer.Color,
            SortOrder = layer.SortOrder,
            IsActive = layer.IsActive,
            ComponentName = layer.ComponentName,
            CreatedAt = layer.CreatedAt,
            UpdatedAt = layer.UpdatedAt
        };
    }
}
