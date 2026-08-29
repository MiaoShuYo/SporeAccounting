using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

/// <summary>
/// 支付方式响应模型
/// </summary>
public class PaymentMethodResponse
{
    /// <summary>
    /// 支付方式ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 支付方式名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 支付方式类型
    /// </summary>
    public PaymentMethodTypeEnum Type { get; set; }

    /// <summary>
    /// 电子支付子类型
    /// </summary>
    public ElectronicPaymentTypeEnum? ElectronicPaymentType { get; set; }

    /// <summary>
    /// 是否为默认支付方式
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDateTime { get; set; }
}
