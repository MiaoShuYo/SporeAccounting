namespace SP.Common.Model;

/// <summary>
/// 分页响应模型
/// </summary>
public class PageResponseModel<T>
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页数据
    /// </summary>
    public List<T> Data { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; set; }
}