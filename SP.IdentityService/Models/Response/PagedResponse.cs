namespace SP.IdentityService.Models.Response;

public class PagedResponse<T> where T : class
{
    /// <summary>
    /// 总行数
    /// </summary>
    public long TotalRow { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public long TotalPage { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public List<T> Data { get; set; } = new List<T>();
}