using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SP.Common.Logger
{
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
        
        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        /// <summary>
        /// 记录关键错误级别的日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }

        /// <summary>
        /// 记录关键错误级别的日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogCritical(Exception exception, string message, params object[] args)
        {
            _logger.LogCritical(exception, message, args);
        }
    }
} 