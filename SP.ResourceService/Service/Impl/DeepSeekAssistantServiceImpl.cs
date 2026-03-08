using System.Text.Json;
using Microsoft.Extensions.Options;
using RestSharp;
using SP.ResourceService.Models.Config;
using SP.ResourceService.Models.Enumeration;
using SP.ResourceService.Models.AI.DeepSeek;
using SP.ResourceService.Models.Response;

namespace SP.ResourceService.Service.Impl;

/// <summary>
/// DeepSeek助手服务实现
/// </summary>
public class DeepSeekAssistantServiceImpl : IAssistantService
{
    /// <summary>
    /// 提示词配置选项
    /// </summary>
    private readonly PromptsOptions _promptsOptions;

    /// <summary>
    /// DeepSeek 配置选项
    /// </summary>
    private readonly DeepSeekOptions _deepSeekOptions;

    private readonly ILogger<DeepSeekAssistantServiceImpl> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="promptsOptions"></param>
    /// <param name="deepSeekOptions"></param>
    /// <param name="logger"></param>
    public DeepSeekAssistantServiceImpl(IOptions<PromptsOptions> promptsOptions,
        IOptions<DeepSeekOptions> deepSeekOptions, ILogger<DeepSeekAssistantServiceImpl> logger)
    {
        ValidatePromptsConfiguration(promptsOptions.Value);
        ValidateDeepSeekConfiguration(deepSeekOptions.Value);
        _promptsOptions = promptsOptions.Value;
        _deepSeekOptions = deepSeekOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// 提取文字中的金额和消费类型
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <returns>金额和消费类型</returns>
    public async Task<AmountAndCategoryExtractionResponse> ExtractAmountAndCategoryAsync(string text)
    {
        string url = _deepSeekOptions.BaseUrl + _deepSeekOptions.Chat;
        string apiKey = _deepSeekOptions.APIKey;
        var options = new RestClientOptions(url)
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest(url, Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Bearer " + apiKey);
        // 构造请求体
        RequestData requestData = new RequestData();

        // 新建角色
        Message systemMessage = new Message();
        systemMessage.Content = _promptsOptions.OCRAmount;
        systemMessage.Role = AIRole.System;
        List<Message> messages = new List<Message>();
        Message userMessage = new Message();
        userMessage.Content = text;
        userMessage.Role = AIRole.User;
        messages.Add(systemMessage);
        messages.Add(userMessage);

        requestData.Messages = messages;
        requestData.Temperature = 0.7d;
        string body = JsonSerializer.Serialize(requestData);
        request.AddStringBody(body, DataFormat.Json);
        RestResponse response = await client.ExecuteAsync(request);
        DeepSeekChatResponse deepSeekChatResponse = JsonSerializer.Deserialize<DeepSeekChatResponse>(response.Content);
        List<Choice> choices = deepSeekChatResponse.Choices;
        if (choices != null && choices.Count > 0)
        {
            _logger.LogInformation(response.Content);
            string content = choices[0].Message.Content;
            AmountAndCategoryExtractionResponse result = JsonSerializer.Deserialize<AmountAndCategoryExtractionResponse>(content);
            return result;
        }
        else
        {
            _logger.LogError("DeepSeek未返回有效的回答，"+response.Content);
            return new AmountAndCategoryExtractionResponse();
        }
    }

    /// <summary>
    /// 校验Prompts参数
    /// </summary>
    /// <param name="promptsOptions"></param>
    private void ValidatePromptsConfiguration(PromptsOptions promptsOptions)
    {
        if (string.IsNullOrWhiteSpace(promptsOptions.OCRAmount))
        {
            throw new ArgumentException("OCR金额提示词不能为空");
        }
    }

    /// <summary>
    /// 校验DeepSeek参数
    /// </summary>
    /// <param name="deepSeekOptions"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ValidateDeepSeekConfiguration(DeepSeekOptions deepSeekOptions)
    {
        if (string.IsNullOrWhiteSpace(deepSeekOptions.APIKey))
        {
            throw new ArgumentException("DeepSeek API Key不能为空");
        }

        if (string.IsNullOrWhiteSpace(deepSeekOptions.BaseUrl))
        {
            throw new ArgumentException("DeepSeek BaseUrl不能为空");
        }

        if (string.IsNullOrWhiteSpace(deepSeekOptions.Chat))
        {
            throw new ArgumentException("DeepSeek Chat地址不能为空");
        }
    }
}