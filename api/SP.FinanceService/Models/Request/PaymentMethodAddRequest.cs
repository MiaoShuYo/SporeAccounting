using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 支付方式添加请求模型
/// </summary>
public class PaymentMethodAddRequest
{
    /// <summary>
    /// 支付方式名称
    /// </summary>
    [Required(ErrorMessage = "支付方式名称不能为空")]
    [MaxLength(20, ErrorMessage = "支付方式名称不能超过20个字符")]
    public string Name { get; set; }

    /// <summary>
    /// 支付方式类型
    /// </summary>
    [Required(ErrorMessage = "支付方式类型不能为空")]
    public PaymentMethodTypeEnum Type { get; set; }

    /// <summary>
    /// 电子支付子类型（Type 为 ElectronicPayment 时必填）
    /// </summary>
    public ElectronicPaymentTypeEnum? ElectronicPaymentType { get; set; }

    /// <summary>
    /// 是否设为默认支付方式
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200, ErrorMessage = "备注不能超过200个字符")]
    public string? Remark { get; set; }
}
