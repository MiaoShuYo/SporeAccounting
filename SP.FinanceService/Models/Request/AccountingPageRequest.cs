using System.ComponentModel.DataAnnotations;
using SP.Common.Model;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 记账分页请求模型
/// </summary>
public class AccountingPageRequest : PageRequest
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public long? CategoryId { get; set; }

    /// <summary>
    /// 账本id
    /// </summary>
    [Required(ErrorMessage = "账本id不能为空")]
    public long AccountBookId { get; set; }
}