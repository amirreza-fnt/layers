using Map.Shared.Auth.Authorization;
using Map.Shared.Auth.Permissions;
using Microsoft.AspNetCore.Mvc;
using LayerManager.API.Services.Interfaces;

namespace LayerManager.API.Controllers;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        try
        {
            var tree = await _categoryService.GetTreeAsync();
            return Ok(new { success = true, data = tree });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category tree");
            return StatusCode(500, new { success = false, message = "خطا در دریافت درخت دسته‌بندی" });
        }
    }

    [HttpGet("tree/flat")]
    public async Task<IActionResult> GetFlatTree()
    {
        try
        {
            var flat = await _categoryService.GetFlatTreeAsync();
            return Ok(new { success = true, data = flat });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flat category list");
            return StatusCode(500, new { success = false, message = "خطا در دریافت لیست دسته‌بندی" });
        }
    }

    [HttpPatch("{categoryId:guid}/parent")]
    [HasPermission(PermissionConstants.CategoryUpdate)]
    public async Task<IActionResult> SetParent(Guid categoryId, [FromQuery] Guid? parentId)
    {
        try
        {
            var result = await _categoryService.SetParentAsync(categoryId, parentId);
            if (!result)
                return NotFound(new { success = false, message = "دسته‌بندی یافت نشد" });

            return Ok(new { success = true, message = "والد دسته‌بندی با موفقیت تنظیم شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting parent for category {Id}", categoryId);
            return StatusCode(500, new { success = false, message = "خطا در تنظیم والد دسته‌بندی" });
        }
    }

    [HttpPost("sync")]
    [HasPermission(PermissionConstants.CategoryCreate)]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _categoryService.SyncCategoriesAsync();
            return Ok(new { success = true, message = "دسته‌بندی‌ها همگام‌سازی شدند" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing categories");
            return StatusCode(500, new { success = false, message = "خطا در همگام‌سازی دسته‌بندی‌ها" });
        }
    }
}
