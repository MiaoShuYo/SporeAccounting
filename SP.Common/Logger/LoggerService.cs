using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SP.Common.Logger
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogInformation(string message, params object[] args);

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogError(string message, params object[] args);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogError(Exception exception, string message, params object[] args);

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogDebug(string message, params object[] args);

        /// <summary>
        /// 记录关键错误日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogCritical(string message, params object[] args);

        /// <summary>
        /// 记录关键错误日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        void LogCritical(Exception exception, string message, params object[] args);
    }

    /// <summary>
    /// 日志服务实现
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        /// <inheritdoc />
        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        /// <inheritdoc />
        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        /// <inheritdoc />
        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        /// <inheritdoc />
        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        /// <inheritdoc />
        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }

        /// <inheritdoc />
        public void LogCritical(Exception exception, string message, params object[] args)
        {
            _logger.LogCritical(exception, message, args);
        }
    }
} 