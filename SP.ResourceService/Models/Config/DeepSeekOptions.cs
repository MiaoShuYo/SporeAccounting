namespace SP.ResourceService.Models.Config;

/// <summary>
/// DeepSeek 配置选项
/// </summary>
public class DeepSeekOptions
{
    /// <summary>
    /// API Key
    /// </summary>
    public string APIKey { get; set; }
    
    /// <summary>
    /// 基础URL
    /// </summary>
    public string BaseUrl {get;set;}
    
    /// <summary>
    /// 对话补全地址
    /// </summary>
    public string Chat { get; set; }
}