using System.Text.Json;
using System.Text.Json.Serialization;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// IncomeExpenditureTypeEnmu 枚举类型转换器
/// </summary>
public class IncomeExpenditureTypeEnmuConverter : JsonConverter<IncomeExpenditureTypeEnmu>
{
    /// <summary>
    /// 读取 JSON 并转换为 IncomeExpenditureTypeEnmu 枚举类型
    /// </summary>
    public override IncomeExpenditureTypeEnmu Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var enumString = reader.GetString();
            if (Enum.TryParse(enumString, out IncomeExpenditureTypeEnmu enumValue))
            {
                return enumValue;
            }
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {nameof(IncomeExpenditureTypeEnmu)}.");
    }

    /// <summary>
    /// 将 IncomeExpenditureTypeEnmu 枚举类型写入 JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IncomeExpenditureTypeEnmu value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
