using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace SP.Common.Logger
{
    /// <summary>
    /// Loki日志配置服务实现
    /// </summary>
    public class LokiLoggerConfigService : ILokiLoggerConfigService
    {
        private readonly LokiOptions _options;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Loki配置选项</param>
        public LokiLoggerConfigService(IOptions<LokiOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// 配置并返回Serilog日志记录器
        /// </summary>
        /// <returns>已配置的Serilog日志记录器</returns>
        public Serilog.Core.Logger ConfigureLogger()
        {
            // 清理URL（移除末尾斜杠）
            var lokiUrl = _options.Url.TrimEnd('/');

            // 创建基本标签
            var labels = new List<LokiLabel>()
            {
                new LokiLabel()
                {
                    Key = "app",
                    Value = _options.AppName
                },
                new LokiLabel()
                {
                    Key = "environment",
                    Value = _options.Environment
                }
            };

            // 创建Loki配置
            var credentials = string.IsNullOrEmpty(_options.Username)
                ? null
                : new LokiCredentials
                {
                    Login = _options.Username,
                    Password = _options.Password
                };

            // 配置Serilog
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

            // 只有在URL配置时才添加Loki sink
            if (!string.IsNullOrEmpty(lokiUrl))
            {
                configuration = configuration.WriteTo.GrafanaLoki(
                    uri: lokiUrl,
                    credentials: credentials,
                    textFormatter: null,
                    batchPostingLimit: 100,
                    queueLimit: 10000,
                    period: TimeSpan.FromSeconds(2),
                    labels: labels,
                    restrictedToMinimumLevel: LogEventLevel.Information);
            }

            return configuration.CreateLogger();
        }
    }
}