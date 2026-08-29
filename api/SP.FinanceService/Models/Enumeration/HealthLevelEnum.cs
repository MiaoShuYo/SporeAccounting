namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 财务健康等级
/// </summary>
public enum HealthLevelEnum
{
    /// <summary>
    /// 较差（0~39 分）
    /// </summary>
    Poor = 0,

    /// <summary>
    /// 一般（40~59 分）
    /// </summary>
    Fair = 1,

    /// <summary>
    /// 良好（60~79 分）
    /// </summary>
    Good = 2,

    /// <summary>
    /// 优秀（80~100 分）
    /// </summary>
    Excellent = 3
}
