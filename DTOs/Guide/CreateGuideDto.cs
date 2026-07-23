using System.ComponentModel.DataAnnotations;

namespace LayerManager.API.DTOs.Guide;

public class CreateGuideDto
{
    [Required(ErrorMessage = "عنوان راهنما الزامی است")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(100)]
    public string? MapIcon { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}