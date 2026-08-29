using System.ComponentModel.DataAnnotations;
using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Request;

/// <summary>
/// 报表智能解读请求模型
/// </summary>
public class ReportInsightRequest
{
    /// <summary>
    /// 报表类型
    /// </summary>
    [Required(ErrorMessage = "报表类型不能为空")]
    public ReportTypeEnum ReportType { get; set; } = ReportTypeEnum.Month;

    /// <summary>
    /// 年份
    /// </summary>
    [Required(ErrorMessage = "年份不能为空")]
    public int Year { get; set; }

    /// <summary>
    /// 月份，月报表解读时使用
    /// </summary>
    public int? Month { get; set; }
}