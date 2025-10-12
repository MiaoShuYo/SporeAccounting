namespace SP.MLService.Models.Dto;

/// <summary>
/// 类目DTO
/// </summary>
/// <remarks>
/// 用户自定义类目的数据传输对象
/// </remarks>
public class CategoryDto
{
    /// <summary>类目唯一标识</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>类目显示名称（如"餐饮"、"交通"）</summary>
    public string Name { get; set; } = string.Empty;
}