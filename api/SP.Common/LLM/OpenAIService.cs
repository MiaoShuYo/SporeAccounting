using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace SP.Common.LLM
{
    /// <summary>
    /// OpenAI 接口封装，包含日志、异常处理与指数退避重试
    /// </summary>
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<OpenAIService> _logger;
        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">OpenAI 配置选项</param>
        /// <param name="logger">日志记录器</param>
        public OpenAIService(OpenAIOptions options, ILogger<OpenAIService> logger)
        {
            _logger = logger;
            _maxRetries = Math.Max(1, options.MaxRetries);
            _retryDelayMs = Math.Max(0, options.RetryDelayMilliseconds);

            var endpoint = new Uri(options.ApiSecret);
            var credential = new ApiKeyCredential(options.ApiKey);
            var clientOptions = new OpenAIClientOptions { Endpoint = endpoint };
            _chatClient = new OpenAIClient(credential, clientOptions).GetChatClient(options.Model);
        }

        /// <inheritdoc />
        public Task<string> ChatAsync(string message, CancellationToken cancellationToken = default)
            => ChatAsync([ChatMessage.CreateUserMessage(message)], cancellationToken);

        /// <inheritdoc />
        public async Task<string> ChatAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
        {
            IList<ChatMessage> messageList = messages as IList<ChatMessage> ?? messages.ToList();
            _logger.LogDebug("[OpenAI] ChatAsync 开始，消息数：{Count}", messageList.Count);

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    ClientResult<ChatCompletion> result =
                        await _chatClient.CompleteChatAsync(messageList, cancellationToken: cancellationToken);

                    string text = result.Value.Content[0].Text;
                    _logger.LogDebug("[OpenAI] ChatAsync 成功，Token 用量：{Tokens}",
                        result.Value.Usage?.TotalTokenCount);
                    return text;
                }
                catch (ClientResultException ex) when (IsTransient(ex) && attempt < _maxRetries)
                {
                    int delay = CalculateDelay(attempt);
                    _logger.LogWarning(ex,
                        "[OpenAI] ChatAsync 瞬态错误（HTTP {Status}），第 {Attempt}/{Max} 次，{Delay}ms 后重试",
                        ex.Status, attempt, _maxRetries, delay);
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("[OpenAI] ChatAsync 已取消");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OpenAI] ChatAsync 不可重试的错误（第 {Attempt} 次尝试）", attempt);
                    throw;
                }
            }

            _logger.LogError("[OpenAI] ChatAsync 已达最大重试次数 {Max}", _maxRetries);
            throw new InvalidOperationException($"OpenAI ChatAsync 超出最大重试次数（{_maxRetries}）。");
        }

        /// <inheritdoc />
        public IAsyncEnumerable<string> ChatStreamingAsync(string message, CancellationToken cancellationToken = default)
            => ChatStreamingAsync([ChatMessage.CreateUserMessage(message)], cancellationToken);

        /// <inheritdoc />
        /// <remarks>
        /// 重试时会缓冲完整响应后再逐块 yield，以保证重试语义正确。
        /// </remarks>
        public async IAsyncEnumerable<string> ChatStreamingAsync(
            IEnumerable<ChatMessage> messages,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IList<ChatMessage> messageList = messages as IList<ChatMessage> ?? messages.ToList();
            _logger.LogDebug("[OpenAI] ChatStreamingAsync 开始，消息数：{Count}", messageList.Count);

            List<string> chunks = await CollectStreamWithRetryAsync(messageList, cancellationToken);

            foreach (string chunk in chunks)
                yield return chunk;
        }

        public async Task<T?> ChatStructuredAsync<T>(string message, CancellationToken cancellationToken = default) where T : class
        {
            // 从 T 的类型自动生成 JSON Schema
            JsonNode schemaNode = JsonSchemaExporter.GetJsonSchemaAsNode(JsonSerializerOptions.Default, typeof(T));
            var schema = BinaryData.FromString(schemaNode.ToJsonString());

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: typeof(T).Name,
                    jsonSchema: schema,
                    jsonSchemaIsStrict: true
                )
            };

            var result = await _chatClient.CompleteChatAsync(
                [ChatMessage.CreateUserMessage(message)], options, cancellationToken);

            string json = result.Value.Content[0].Text;
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        // ── 私有辅助 ────────────────────────────────────────────────────────────

        /// <summary>
        /// 带重试的流式数据收集，将所有文本块缓冲到列表中返回。
        /// </summary>
        private async Task<List<string>> CollectStreamWithRetryAsync(
            IList<ChatMessage> messages, CancellationToken cancellationToken)
        {
            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    var chunks = new List<string>();
                    AsyncCollectionResult<StreamingChatCompletionUpdate> stream =
                        _chatClient.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);

                    await foreach (StreamingChatCompletionUpdate update in stream.WithCancellation(cancellationToken))
                    {
                        foreach (ChatMessageContentPart part in update.ContentUpdate)
                        {
                            if (!string.IsNullOrEmpty(part.Text))
                                chunks.Add(part.Text);
                        }
                    }

                    _logger.LogDebug("[OpenAI] ChatStreamingAsync 成功，共 {Count} 个文本块", chunks.Count);
                    return chunks;
                }
                catch (ClientResultException ex) when (IsTransient(ex) && attempt < _maxRetries)
                {
                    int delay = CalculateDelay(attempt);
                    _logger.LogWarning(ex,
                        "[OpenAI] ChatStreamingAsync 瞬态错误（HTTP {Status}），第 {Attempt}/{Max} 次，{Delay}ms 后重试",
                        ex.Status, attempt, _maxRetries, delay);
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("[OpenAI] ChatStreamingAsync 已取消");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[OpenAI] ChatStreamingAsync 不可重试的错误（第 {Attempt} 次尝试）", attempt);
                    throw;
                }
            }

            _logger.LogError("[OpenAI] ChatStreamingAsync 已达最大重试次数 {Max}", _maxRetries);
            throw new InvalidOperationException($"OpenAI ChatStreamingAsync 超出最大重试次数（{_maxRetries}）。");
        }

        /// <summary>
        /// 判断是否为可重试的瞬态 HTTP 错误（限流 429 / 服务端 5xx）
        /// </summary>
        private static bool IsTransient(ClientResultException ex)
            => ex.Status == 429 || (ex.Status >= 500 && ex.Status <= 599);

        /// <summary>
        /// 计算指数退避延迟：delay * 2^(attempt-1)
        /// </summary>
        private int CalculateDelay(int attempt)
            => _retryDelayMs * (int)Math.Pow(2, attempt - 1);
    }
}
