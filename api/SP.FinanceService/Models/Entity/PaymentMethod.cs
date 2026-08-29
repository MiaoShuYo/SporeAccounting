using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 常用支付方式
/// </summary>
[Table(name: "PaymentMethod")]
public class PaymentMethod : BaseModel
{
    /// <summary>
    /// 支付方式名称
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string Name { get; set; }

    /// <summary>
    /// 支付方式类型
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint")]
    public PaymentMethodTypeEnum Type { get; set; }

    /// <summary>
    /// 电子支付子类型（仅当 Type 为 ElectronicPayment 时有值）
    /// </summary>
    [Column(TypeName = "tinyint")]
    public ElectronicPaymentTypeEnum? ElectronicPaymentType { get; set; }

    /// <summary>
    /// 是否为默认支付方式
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint(1)")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200)]
    [Column(TypeName = "nvarchar(200)")]
    public string? Remark { get; set; }
}
