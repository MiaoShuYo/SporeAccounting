using OpenAI.Chat;

namespace SP.Common.LLM
{
    /// <summary>
    /// OpenAI 服务接口
    /// </summary>
    public interface IOpenAIService
    {
        /// <summary>
        /// 发送单条用户消息，返回完整回复
        /// </summary>
        Task<string> ChatAsync(string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送多轮对话消息，返回完整回复
        /// </summary>
        Task<string> ChatAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送单条用户消息，以流式方式逐块返回回复
        /// </summary>
        IAsyncEnumerable<string> ChatStreamingAsync(string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送多轮对话消息，以流式方式逐块返回回复
        /// </summary>
        IAsyncEnumerable<string> ChatStreamingAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送消息，要求 AI 返回符合指定类型结构的 JSON，并反序列化为 T
        /// </summary>
        Task<T?> ChatStructuredAsync<T>(string message, CancellationToken cancellationToken = default) where T : class;
    }
}
