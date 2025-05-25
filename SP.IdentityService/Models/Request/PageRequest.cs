namespace SP.IdentityService.Models.Request;

/// <summary>
/// 分页请求基类
/// </summary>
public class PageRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string SortField { get; set; } = "Id";

    /// <summary>
    /// 排序方式
    /// </summary>
    public string SortOrder { get; set; } = "asc";
}