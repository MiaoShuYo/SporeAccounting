namespace SP.Common.LLM
{
    /// <summary>
    /// OpenAI 配置选项
    /// </summary>
    public class OpenAIOptions
    {
        /// <summary>
        /// API 密钥
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// API 基础 URL，默认 https://api.openai.com/v1
        /// </summary>
        public string ApiSecret { get; set; } = "https://api.openai.com/v1";

        /// <summary>
        /// 模型名称
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// 最大重试次数（仅针对可重试的瞬态错误），默认 3
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// 首次重试基础延迟（毫秒），之后按指数增长，默认 500
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 500;
    }
}
