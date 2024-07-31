namespace SporeAccounting.BaseModels.ViewModel.Response;

/// <summary>
/// 分页查询响应基类
/// </summary>
public class PageResponseViewModel<T>
{
    /// <summary>
    /// 总页数
    /// </summary>
    public int PageCount { get; set; } = 0;
    /// <summary>
    /// 总行数
    /// </summary>
    public int RowCount { get; set; }= 0;
    /// <summary>
    /// 返回的数据集合
    /// </summary>
    public List<T> Data { get; set; }=new List<T>();
}