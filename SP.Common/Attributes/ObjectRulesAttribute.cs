using System.ComponentModel.DataAnnotations;

namespace SP.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ObjectRulesAttribute : ValidationAttribute
{
    public string[] AnyOf { get; set; } = Array.Empty<string>();
    // 规则格式："A=>B" 表示当 A 有值时，B 必填
    public string[] RequireIfPresent { get; set; } = Array.Empty<string>();

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var type = value.GetType();
        var errors = new List<ValidationResult>();

        bool HasValue(string? s) => !string.IsNullOrWhiteSpace(s);

        // AnyOf：至少一个字段有值
        if (AnyOf != null && AnyOf.Length > 0)
        {
            var anyHas = AnyOf.Any(p =>
            {
                var v = type.GetProperty(p)?.GetValue(value) as string;
                return HasValue(v);
            });
            if (!anyHas)
            {
                errors.Add(new ValidationResult(
                    $"以下字段至少填写一个：{string.Join(", ", AnyOf)}",
                    AnyOf));
            }
        }

        // RequireIfPresent：当 A 有值时，B 必填
        if (RequireIfPresent != null && RequireIfPresent.Length > 0)
        {
            foreach (var rule in RequireIfPresent)
            {
                var parts = rule.Split("=>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;

                var src = parts[0];
                var dst = parts[1];

                var srcVal = type.GetProperty(src)?.GetValue(value) as string;
                var dstVal = type.GetProperty(dst)?.GetValue(value) as string;

                if (HasValue(srcVal) && !HasValue(dstVal))
                {
                    errors.Add(new ValidationResult(
                        $"当填写了“{src}”时，“{dst}”为必填项",
                        new[] { dst }));
                }
            }
        }

        if (errors.Count > 0)
        {
            if (errors.Count == 1) return errors[0];
            // 聚合错误
            var members = errors.SelectMany(e => e.MemberNames).Distinct().ToArray();
            return new ValidationResult(string.Join("；", errors.Select(e => e.ErrorMessage)), members);
        }

        return ValidationResult.Success;
    }
}