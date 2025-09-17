namespace SP.ResourceService.Models.Enumeration;

/// <summary>
/// AI工具选择
/// </summary>
public class AIToolChoice
{
    /// <summary>
    /// 不使用
    /// </summary>
    public const string None = "none";

    /// <summary>
    /// 自动选择
    /// </summary>
    public const string Auto = "auto";

    /// <summary>
    /// 必须使用一个或多个
    /// </summary>
    public const string Required = "required";
}