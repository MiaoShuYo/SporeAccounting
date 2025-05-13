using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions
{
    /// <summary>
    /// 数据验证异常
    /// </summary>
    public class ValidationException : AppException
    {
        /// <summary>
        /// 验证错误
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// 创建一个数据验证异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public ValidationException(string message) 
            : base(message, HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// 创建一个数据验证异常实例
        /// </summary>
        /// <param name="errors">验证错误</param>
        public ValidationException(IDictionary<string, string[]> errors) 
            : base("提交的数据无效", HttpStatusCode.BadRequest)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        /// <summary>
        /// 创建一个数据验证异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="errors">验证错误</param>
        public ValidationException(string message, IDictionary<string, string[]> errors) 
            : base(message, HttpStatusCode.BadRequest)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
} 