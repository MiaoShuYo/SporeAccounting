using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 报表视图模型
/// </summary>
public class ReportViewModel
{
    /// <summary>
    /// 报表类型
    /// </summary>
    [Required(ErrorMessage = "报表类型不能为空")]
    public ReportTypeEnum ReportType { get; set; }
    /// <summary>
    /// 年份
    /// </summary>
    [Required(ErrorMessage = "年份不能为空")]
    public int Year { get; set; }
    /// <summary>
    /// 月份
    /// </summary>
    public int? Month { get; set; }
}