namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 分摊方式
/// </summary>
public enum SplitTypeEnum
{
    /// <summary>
    /// 按比例
    /// </summary>
    Rata =0,

    /// <summary>
    /// 按人头均摊
    /// </summary>
    PerCapita=1,

    /// <summary>
    /// 按金额
    /// </summary>
    Amount =2,

    /// <summary>
    /// 混合
    /// </summary>
    Mixture=3
}