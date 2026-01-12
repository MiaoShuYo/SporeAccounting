using SP.Common.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 定期支出规则执行记录
/// </summary>
[Table(name: "RecurringExpenseRuleExecutionRecord")]
public class RecurringExpenseRuleExecutionRecord : BaseModel
{
    /// <summary>
    /// 定期支出规则id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long RecurringExpenseRuleExecutioId { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint")]
    public bool IsOK { get; set; } = true;


}