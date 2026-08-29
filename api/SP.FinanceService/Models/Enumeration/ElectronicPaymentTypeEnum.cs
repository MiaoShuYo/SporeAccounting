namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 电子支付子类型
/// </summary>
public enum ElectronicPaymentTypeEnum
{
    /// <summary>
    /// 支付宝
    /// </summary>
    Alipay = 0,

    /// <summary>
    /// 微信
    /// </summary>
    WeChatPay = 1,

    /// <summary>
    /// 云闪付
    /// </summary>
    UnionPay = 2,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 3
}
