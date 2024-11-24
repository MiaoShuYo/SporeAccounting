namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 收支记录信息视图模型
/// </summary>
public class IncomeExpenditureRecordInfoViewModel
{
    /// <summary>
    /// 收支记录Id
    /// </summary>
    public string IncomeExpenditureRecordId { get; set; }

    /// <summary>
    /// 转换前金额
    /// </summary>
    public decimal BeforAmount { get; set; }

    /// <summary>
    /// 转换后金额
    /// </summary>
    public decimal AfterAmount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    public string IncomeExpenditureClassificationId { get; set; }

    /// <summary>
    /// 收支分类名称
    /// </summary>
    public string IncomeExpenditureClassificationName { get; set; }

    /// <summary>
    /// 账簿Id
    /// </summary>
    public string AccountBookId { get; set; }

    /// <summary>
    /// 账簿名称
    /// </summary>
    public string AccountBookName { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 转换前币种Id
    /// </summary>
    public string CurrencyId { get; set; }

    /// <summary>
    /// 转换前币种名称
    /// </summary>
    public string CurrencyName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}