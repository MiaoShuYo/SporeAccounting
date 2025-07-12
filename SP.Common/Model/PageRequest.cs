namespace SP.Common.Model;

/// <summary>
/// 分页模型
/// </summary>
public class PageRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;
}