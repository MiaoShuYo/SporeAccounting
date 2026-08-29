using System.Text.Json.Serialization;
using SP.ResourceService.Models.Enumeration;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// 请求数据
/// </summary>
public class RequestData
{
    /// <summary>
    /// 对话的消息列表
    /// </summary>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }=new List<Message>();

    /// <summary>
    /// 使用的模型
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = AIModel.DeepSeekChat;

    /// <summary>
    /// 重复惩罚
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0d;

    /// <summary>
    /// 一次请求中模型生成 completion 的最大 token 数
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// 存在惩罚系数，范围为-2到2，数值越大，模型越倾向于生成新的内容
    /// </summary>
    private double _presencePenalty = 0d;

    /// <summary>
    /// 存在惩罚系数，范围为-2到2，数值越大，模型越倾向于生成新的内容
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty
    {
        get => _presencePenalty;
        set
        {
            if (value < -2) _presencePenalty = -2;
            else if (value > 2) _presencePenalty = 2;
            else _presencePenalty = value;
        }
    }

    /// <summary>
    /// 指定模型必须输出的格式
    /// </summary>
    [JsonPropertyName("response_format")]
    public ResponseFormat ResponseFormat { get; set; }

    /// <summary>
    /// 停止词，可以为字符串或字符串数组，模型遇到这些内容时会停止生成
    /// </summary>
    [JsonPropertyName("stop")]
    public object Stop { get; set; }

    /// <summary>
    /// 是否以流式方式返回结果
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    /// <summary>
    /// 流式选项，具体结构根据接口定义
    /// </summary>
    [JsonPropertyName("stream_options")]
    public object StreamOptions { get; set; }

    /// <summary>
    /// 采样温度，控制输出的随机性，值越高结果越随机，取值范围为[0,2]
    /// </summary>
    private double _temperature = 1d;

    /// <summary>
    /// 采样温度，控制输出的随机性，值越高结果越随机，取值范围为[0,2]
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature
    {
        get => _temperature;
        set
        {
            if (value < 0) _temperature = 0;
            else if (value > 2) _temperature = 2;
            else _temperature = value;
        }
    }

    /// <summary>
    /// 作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。
    /// 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。
    /// 取值范围(0,1]
    /// </summary>
    private double _topP=1;

    /// <summary>
    /// 作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。
    /// 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。
    /// 取值范围(0,1]
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP
    {
        get => _topP;
        set
        {
            if (value <= 0) _topP = 0.01; // 防止为0或负数，最小为0.01
            else if (value > 1) _topP = 1;
            else _topP = value;
        }
    }

    /// <summary>
    /// 工具列表
    /// </summary>
    [JsonPropertyName("tools")]
    public object Tools { get; set; }

    /// <summary>
    /// 工具选择，指定使用的工具
    /// </summary>
    [JsonPropertyName("tool_choice")]
    public string ToolChoice { get; set; }

    /// <summary>
    /// 是否返回每个token的对数概率
    /// </summary>
    [JsonPropertyName("logprobs")]
    public bool Logprobs { get; set; } = false;

    /// <summary>
    /// 返回的top logprobs数量，介于 0 到 20 之间的整数 N
    /// </summary>
    private int? _topLogprobs;
    /// <summary>
    /// 返回的top logprobs数量，介于 0 到 20 之间的整数 N
    /// </summary>
    [JsonPropertyName("top_logprobs")]
    public int? TopLogprobs
    {
        get => _topLogprobs;
        set
        {
            if (value == null)
            {
                _topLogprobs = null;
            }
            else if (value < 0)
            {
                _topLogprobs = 0;
            }
            else if (value > 20)
            {
                _topLogprobs = 20;
            }
            else
            {
                _topLogprobs = value;
            }
        }
    }
}