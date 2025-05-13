using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace SP.Common.Logger
{
    /// <summary>
    /// Loki日志配置服务接口
    /// </summary>
    public interface ILokiLoggerConfigService
    {
        /// <summary>
        /// 配置并返回Serilog日志记录器
        /// </summary>
        /// <returns>已配置的Serilog日志记录器</returns>
        Serilog.Core.Logger ConfigureLogger();
    }

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

        /// <inheritdoc />
        public Serilog.Core.Logger ConfigureLogger()
        {
            // 创建基本标签
            var labels = new Dictionary<string, string>
            {
                { "app", _options.AppName },
                { "environment", _options.Environment }
            };

            // 创建Loki配置
            var credentials = string.IsNullOrEmpty(_options.Username) 
                ? null 
                : new LokiCredentials 
                {
                    Username = _options.Username,
                    Password = _options.Password
                };

            // 配置Serilog
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.GrafanaLoki(
                    uri: _options.Url, 
                    credentials: credentials,
                    labels: labels,
                    textFormatter: null,
                    batchPostingLimit: 100,
                    queueLimit: 10000,
                    period: TimeSpan.FromSeconds(2),
                    restrictedToMinimumLevel: LogEventLevel.Information);

            return configuration.CreateLogger();
        }
    }
} 