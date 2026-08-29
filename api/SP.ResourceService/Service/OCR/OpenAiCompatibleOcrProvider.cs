using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SP.ResourceService.Models.AI.DeepSeek;
using SP.ResourceService.Models.Config;
using SP.ResourceService.Models.Enumeration;
using SP.ResourceService.Models.OCR;

namespace SP.ResourceService.Service.OCR;

/// <summary>
/// OpenAI兼容OCR供应商
/// </summary>
public class OpenAiCompatibleOcrProvider : IOcrProvider
{
    /// <summary>
    /// HttpClient名称
    /// </summary>
    public const string HttpClientName = "OpenAiCompatibleOcr";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<LlmOptions> _llmOptions;
    private readonly IOptionsMonitor<OcrOptions> _ocrOptions;
    private readonly ILogger<OpenAiCompatibleOcrProvider> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="llmOptions"></param>
    /// <param name="ocrOptions"></param>
    /// <param name="logger"></param>
    public OpenAiCompatibleOcrProvider(IHttpClientFactory httpClientFactory,
        IOptionsMonitor<LlmOptions> llmOptions,
        IOptionsMonitor<OcrOptions> ocrOptions,
        ILogger<OpenAiCompatibleOcrProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _llmOptions = llmOptions;
        _ocrOptions = ocrOptions;
        _logger = logger;
    }

    /// <summary>
    /// 识别图片文字
    /// </summary>
    /// <param name="image">图片字节</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>OCR识别结果</returns>
    public async Task<OcrRecognitionResult> RecognizeAsync(byte[] image, CancellationToken cancellationToken = default)
    {
        LlmOptions llmOptions = _llmOptions.CurrentValue;
        OcrOptions ocrOptions = _ocrOptions.CurrentValue;
        ValidateConfiguration(llmOptions, ocrOptions);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, ocrOptions.TimeoutSeconds)));

        string rawResponse = await SendRequestAsync(image, llmOptions, ocrOptions, timeoutCts.Token);
        string content = GetMessageContent(rawResponse);
        OcrTextResponse textResponse = ParseOcrTextResponse(content);
        string recognizedText = string.IsNullOrWhiteSpace(textResponse.Text)
            ? string.Join("", textResponse.Lines)
            : textResponse.Text;

        if (string.IsNullOrWhiteSpace(recognizedText))
        {
            throw new InvalidOperationException("OCR模型未返回有效文字");
        }

        return new OcrRecognitionResult
        {
            RecognizedText = recognizedText,
            Lines = textResponse.Lines,
            Provider = ocrOptions.Provider,
            RawResponse = rawResponse
        };
    }

    private async Task<string> SendRequestAsync(byte[] image, LlmOptions llmOptions, OcrOptions ocrOptions,
        CancellationToken cancellationToken)
    {
        string requestUrl = BuildRequestUrl(llmOptions);
        string mediaType = DetectImageMediaType(image);
        string imageDataUrl = $"data:{mediaType};base64,{Convert.ToBase64String(image)}";
        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = ocrOptions.Model,
            ["messages"] = new object[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = AIRole.System,
                    ["content"] = ocrOptions.Prompt
                },
                new Dictionary<string, object?>
                {
                    ["role"] = AIRole.User,
                    ["content"] = new object[]
                    {
                        new Dictionary<string, object?>
                        {
                            ["type"] = "text",
                            ["text"] = "请识别这张图片中的文字。"
                        },
                        new Dictionary<string, object?>
                        {
                            ["type"] = "image_url",
                            ["image_url"] = new Dictionary<string, object?>
                            {
                                ["url"] = imageDataUrl,
                                ["detail"] = ocrOptions.ImageDetail
                            }
                        }
                    }
                }
            },
            ["max_tokens"] = Math.Max(1, ocrOptions.MaxTokens),
            ["temperature"] = Math.Clamp(ocrOptions.Temperature, 0d, 2d),
            ["response_format"] = new Dictionary<string, string>
            {
                ["type"] = AIResponseFormat.JsonObject
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", llmOptions.APIKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OCR请求失败，状态码：{StatusCode}，响应：{Response}", response.StatusCode, responseContent);
            throw new InvalidOperationException($"OCR请求失败，状态码：{response.StatusCode}");
        }

        return responseContent;
    }

    private string GetMessageContent(string rawResponse)
    {
        DeepSeekChatResponse? response = JsonSerializer.Deserialize<DeepSeekChatResponse>(rawResponse, _jsonSerializerOptions);
        string? content = response?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("OCR模型响应中没有有效的消息内容");
        }

        return content;
    }

    private OcrTextResponse ParseOcrTextResponse(string content)
    {
        string jsonContent = StripJsonCodeFence(content);
        try
        {
            OcrTextResponse? response = JsonSerializer.Deserialize<OcrTextResponse>(jsonContent, _jsonSerializerOptions);
            if (response != null)
            {
                return response;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "OCR模型返回内容不是预期JSON，按纯文本处理：{Content}", content);
        }

        return new OcrTextResponse
        {
            Text = jsonContent,
            Lines = jsonContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList()
        };
    }

    private string StripJsonCodeFence(string content)
    {
        string trimmed = content.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        int firstLineEnd = trimmed.IndexOf('\n');
        int lastFenceStart = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        if (firstLineEnd < 0 || lastFenceStart <= firstLineEnd)
        {
            return trimmed;
        }

        return trimmed.Substring(firstLineEnd + 1, lastFenceStart - firstLineEnd - 1).Trim();
    }

    private string BuildRequestUrl(LlmOptions options)
    {
        string chat = string.IsNullOrWhiteSpace(options.Chat) ? "/chat/completions" : options.Chat.Trim();
        if (Uri.TryCreate(chat, UriKind.Absolute, out _))
        {
            return chat;
        }

        return options.BaseUrl.TrimEnd('/') + "/" + chat.TrimStart('/');
    }

    private string DetectImageMediaType(byte[] image)
    {
        if (image.Length >= 8 && image[0] == 0x89 && image[1] == 0x50 && image[2] == 0x4E && image[3] == 0x47)
        {
            return "image/png";
        }

        if (image.Length >= 3 && image[0] == 0xFF && image[1] == 0xD8 && image[2] == 0xFF)
        {
            return "image/jpeg";
        }

        return "image/jpeg";
    }

    private void ValidateConfiguration(LlmOptions llmOptions, OcrOptions ocrOptions)
    {
        if (string.IsNullOrWhiteSpace(llmOptions.BaseUrl) && string.IsNullOrWhiteSpace(llmOptions.Chat))
        {
            throw new ArgumentException("LLM BaseUrl不能为空");
        }

        if (string.IsNullOrWhiteSpace(llmOptions.APIKey))
        {
            throw new ArgumentException("LLM API Key不能为空");
        }

        if (string.IsNullOrWhiteSpace(ocrOptions.Model))
        {
            throw new ArgumentException("OCR模型不能为空");
        }

        if (!string.Equals(ocrOptions.Provider, "OpenAiCompatible", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("当前仅支持OpenAiCompatible OCR供应商");
        }
    }
}