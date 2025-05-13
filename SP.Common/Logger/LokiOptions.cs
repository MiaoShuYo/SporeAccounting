namespace SP.Common.Logger
{
    /// <summary>
    /// Loki日志配置选项
    /// </summary>
    public class LokiOptions
    {
        /// <summary>
        /// Loki服务器地址，例如：http://loki:3100
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 应用名称，用于标识日志来源
        /// </summary>
        public string AppName { get; set; } = "SporeAccounting";

        /// <summary>
        /// 环境名称，如development、production等
        /// </summary>
        public string Environment { get; set; } = "development";

        /// <summary>
        /// 用户名（如果Loki配置了基本认证）
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 密码（如果Loki配置了基本认证）
        /// </summary>
        public string? Password { get; set; }
    }
} 