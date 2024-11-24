using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 收支记录修改视图模型
/// </summary>
public class IncomeExpenditureRecordEditViewModel
{
    /// <summary>
    /// 收支记录Id
    /// </summary>
    [MaxLength(36, ErrorMessage = "收支记录Id最大长度为36")]
    [Required(ErrorMessage = "收支记录Id不能为空")]
    public string IncomeExpenditureRecordId { get; set; }

    /// <summary>
    /// 转换前金额
    /// </summary>
    [Required(ErrorMessage = "转换前金额不能为空")]
    public decimal BeforAmount { get; set; }

    /// <summary>
    /// 转换后金额
    /// </summary>
    [Required(ErrorMessage = "转换后金额不能为空")]
    public decimal AfterAmount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    [MaxLength(36, ErrorMessage = "收支分类Id最大长度为36")]
    [Required(ErrorMessage = "收支分类Id不能为空")]
    public string IncomeExpenditureClassificationId { get; set; }

    /// <summary>
    /// 账簿Id
    /// </summary>
    [MaxLength(36, ErrorMessage = "账簿Id最大长度为36")]
    [Required(ErrorMessage = "账簿Id不能为空")]
    public string AccountBookId { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Required(ErrorMessage = "记录日期不能为空")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 转换前币种Id
    /// </summary>
    [Required(ErrorMessage = "转换前币种Id不能为空")]
    [MaxLength(36, ErrorMessage = "转换前币种Id最大长度为36")]
    public string CurrencyId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(100, ErrorMessage = "备注最大长度为100")]
    public string? Remark { get; set; }
}