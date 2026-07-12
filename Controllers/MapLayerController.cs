using Microsoft.AspNetCore.Mvc;
using LayerManager.API.DTOs.MapLayer;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Controllers;

[ApiController]
[Route("api/map-layers")]
[Produces("application/json")]
public class MapLayerController : ControllerBase
{
    private readonly IMapLayerService _layerService;
    private readonly ILogger<MapLayerController> _logger;

    public MapLayerController(IMapLayerService layerService, ILogger<MapLayerController> logger)
    {
        _layerService = layerService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var layers = await _layerService.GetAllAsync();
            return Ok(new { success = true, data = layers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all layers");
            return StatusCode(500, new { success = false, message = "خطا در دریافت لایه‌ها" });
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var layers = await _layerService.GetActiveAsync();
            return Ok(new { success = true, data = layers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active layers");
            return StatusCode(500, new { success = false, message = "خطا در دریافت لایه‌های فعال" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var layer = await _layerService.GetByIdAsync(id);
            if (layer is null)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, data = layer });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting layer {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در دریافت لایه" });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        try
        {
            var layer = await _layerService.GetBySlugAsync(slug);
            if (layer is null)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, data = layer });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting layer by slug {Slug}", slug);
            return StatusCode(500, new { success = false, message = "خطا در دریافت لایه" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMapLayerDto dto)
    {
        try
        {
            var id = await _layerService.CreateAsync(dto);
            return Ok(new { success = true, message = "لایه با موفقیت ایجاد شد", id });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating layer");
            return StatusCode(500, new { success = false, message = "خطا در ایجاد لایه" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMapLayerDto dto)
    {
        try
        {
            var result = await _layerService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, message = "لایه با موفقیت به‌روزرسانی شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating layer {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در به‌روزرسانی لایه" });
        }
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var result = await _layerService.ToggleActiveAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, message = "وضعیت لایه تغییر کرد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling layer {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در تغییر وضعیت لایه" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _layerService.DeleteAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, message = "لایه با موفقیت حذف شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting layer {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در حذف لایه" });
        }
    }

    [HttpPatch("{id:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromQuery] int sortOrder)
    {
        try
        {
            var result = await _layerService.ReorderAsync(id, sortOrder);
            if (!result)
                return NotFound(new { success = false, message = "لایه یافت نشد" });

            return Ok(new { success = true, message = "ترتیب لایه تغییر کرد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering layer {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در تغییر ترتیب لایه" });
        }
    }
}
