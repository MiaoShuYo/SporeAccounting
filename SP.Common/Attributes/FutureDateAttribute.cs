using System.ComponentModel.DataAnnotations;

namespace SP.Common.Attributes;

/// <summary>
/// 验证日期时间是否大于等于当前日期时间
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class FutureDateAttribute : ValidationAttribute
{
    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = "日期时间必须大于等于当前日期时间";

    /// <summary>
    /// 验证日期时间是否大于等于当前日期时间
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <returns>验证结果</returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is DateTime dateTimeValue)
        {
            return dateTimeValue >= DateTime.Now;
        }

        return false;
    }
}