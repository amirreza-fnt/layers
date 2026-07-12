using System.ComponentModel.DataAnnotations;

namespace LayerManager.API.DTOs.MapLayer;

public class UpdateMapLayerDto
{
    [Required(ErrorMessage = "نام لایه الزامی است")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? IconName { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string? ComponentName { get; set; }
}
