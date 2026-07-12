using System.ComponentModel.DataAnnotations;

namespace LayerManager.API.DTOs.MapLayer;

public class CreateMapLayerDto
{
    [Required(ErrorMessage = "شناسه لایه (Slug) الزامی است")]
    [MaxLength(50)]
    [RegularExpression(@"^[a-z][a-z0-9\-]*$", ErrorMessage = "شناسه لایه باید با حرف انگلیسی شروع شود و فقط شامل حروف کوچک، اعداد و خط تیره باشد")]
    public string Slug { get; set; } = string.Empty;

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
