using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 预算生成请求模型
/// </summary>
/// <remarks>
/// 该模型用于AI生成预算，包含了必要的字段和验证规则，确保生成的预算数据有效且符合业务需求。
/// </remarks>
public class BudgetGenerationRequest
{
    /// <summary>
    /// 预算周期
    /// </summary>
    [Required(ErrorMessage = "预算周期不能为空")]
    public PeriodEnum Period { get; set; }
}