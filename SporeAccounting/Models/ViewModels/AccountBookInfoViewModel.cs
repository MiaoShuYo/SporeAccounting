namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 账簿信息视图模型
/// </summary>
public class AccountBookInfoViewModel
{
    /// <summary>
    /// 账簿Id
    /// </summary>
    public string AccountBookId { get; set; }

    /// <summary>
    /// 账簿名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 账簿描述
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// 账簿余额
    /// </summary>
    public decimal Balance { get; set; }
}