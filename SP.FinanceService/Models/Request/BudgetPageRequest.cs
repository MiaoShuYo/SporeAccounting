using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 预算分页请求模型
/// </summary>
public class BudgetPageRequest : PageRequest
{
    ///<summary>
    /// 预算年份
    ///</summary>
    public int Year { get; set; }

    ///<summary>
    /// 预算月份
    /// </summary>
    public int Month { get; set; }

    ///<summary>
    /// 预算季度
    /// </summary>
    public int Quarter { get; set; }

    ///<summary>
    /// 预算周期
    ///</summary>
    public PeriodEnum Period { get; set; }
}