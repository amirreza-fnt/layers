using Microsoft.AspNetCore.Mvc;
using LayerManager.API.DTOs.Guide;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Controllers;

[ApiController]
[Route("api/guide")]
[Produces("application/json")]
public class GuideController : ControllerBase
{
    private readonly IGuideService _guideService;
    private readonly ILogger<GuideController> _logger;

    public GuideController(IGuideService guideService, ILogger<GuideController> logger)
    {
        _guideService = guideService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var items = await _guideService.GetAllAsync();
            return Ok(new { success = true, data = items });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all guide entries");
            return StatusCode(500, new { success = false, message = "خطا در دریافت راهنماها" });
        }
    }

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategory(Guid categoryId)
    {
        try
        {
            var items = await _guideService.GetActiveByCategoryIdAsync(categoryId);
            return Ok(new { success = true, data = items });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting guide for category {Id}", categoryId);
            return StatusCode(500, new { success = false, message = "خطا در دریافت راهنمای دسته‌بندی" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var item = await _guideService.GetByIdAsync(id);
            if (item is null)
                return NotFound(new { success = false, message = "راهنما یافت نشد" });

            return Ok(new { success = true, data = item });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting guide {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در دریافت راهنما" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuideDto dto)
    {
        try
        {
            var id = await _guideService.CreateAsync(dto);
            return Ok(new { success = true, message = "راهنما با موفقیت ایجاد شد", id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating guide");
            return StatusCode(500, new { success = false, message = "خطا در ایجاد راهنما" });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuideDto dto)
    {
        try
        {
            var result = await _guideService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(new { success = false, message = "راهنما یافت نشد" });

            return Ok(new { success = true, message = "راهنما با موفقیت به‌روزرسانی شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating guide {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در به‌روزرسانی راهنما" });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _guideService.DeleteAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "راهنما یافت نشد" });

            return Ok(new { success = true, message = "راهنما با موفقیت حذف شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting guide {Id}", id);
            return StatusCode(500, new { success = false, message = "خطا در حذف راهنما" });
        }
    }
}
