namespace SporeAccounting.Models.ViewModels;

public class SysUrlQueryViewModel
{
    /// <summary>
    /// URL Id
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// URL 
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// URL描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 是否可以删除
    /// </summary>
    public bool CanDelete { get; set; }

}