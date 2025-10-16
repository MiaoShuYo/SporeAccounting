using System.ComponentModel.DataAnnotations;

namespace SP.Common.Attributes;

/// <summary>
/// 验证开始时间是否大于结束时间
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StartTimeLessThanEndTimeAttribute:ValidationAttribute
{
    private readonly string _startTimePropertyName;
    private readonly string _endTimePropertyName;

    public StartTimeLessThanEndTimeAttribute(string startTimePropertyName, string endTimePropertyName)
    {
        _startTimePropertyName = startTimePropertyName;
        _endTimePropertyName = endTimePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var type = value.GetType();
        var startTimeProperty = type.GetProperty(_startTimePropertyName);
        var endTimeProperty = type.GetProperty(_endTimePropertyName);

        if (startTimeProperty == null)
        {
            return new ValidationResult($"未找到属性 '{_startTimePropertyName}'");
        }

        if (endTimeProperty == null)
        {
            return new ValidationResult($"未找到属性 '{_endTimePropertyName}'");
        }

        var startTimeValue = startTimeProperty.GetValue(value);
        var endTimeValue = endTimeProperty.GetValue(value);

        if (startTimeValue is DateTime startTime && endTimeValue is DateTime endTime)
        {
            if (startTime >= endTime)
            {
                return new ValidationResult($"'{_startTimePropertyName}' 必须小于或等于 '{_endTimePropertyName}'");
            }
        }

        return ValidationResult.Success!;
    }
}