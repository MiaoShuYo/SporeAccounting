using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Options;
using SP.Common.ExceptionHandling.Exceptions;

namespace SP.Common.Logger
{
    /// <summary>
    /// 日志服务扩展方法
    /// </summary>
    public static class LoggerServiceExtensions
    {
        /// <summary>
        /// 添加日志服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddLoggerService(this IServiceCollection services, IConfiguration configuration)
        {
            // 从配置中获取Loki选项
            services.Configure<LokiOptions>(configuration.GetSection("Loki"));

            // 注册Loki日志配置服务
            services.AddSingleton<ILokiLoggerConfigService, LokiLoggerConfigService>();

            // 直接从 IConfiguration 绑定 LokiOptions，避免调用 BuildServiceProvider() 创建第二个容器
            var lokiOptions = new LokiOptions();
            configuration.GetSection("Loki").Bind(lokiOptions);

            // 从Nacos配置中获取ServiceName并设置到LokiOptions
            var serviceName = configuration.GetValue<string>("nacos:ServiceName");
            if (!string.IsNullOrEmpty(serviceName))
            {
                lokiOptions.AppName = serviceName;
            }

            // 使用 Options.Create 构建临时服务实例，无需第二个 DI 容器
            var lokiConfigService = new LokiLoggerConfigService(Options.Create(lokiOptions));
            Log.Logger = lokiConfigService.ConfigureLogger();

            // 添加Serilog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            // 注册日志服务
            services.AddSingleton<ILoggerService, LoggerService>();

            return services;
        }
    }
}