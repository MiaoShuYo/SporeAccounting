using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions
{
    /// <summary>
    /// 错误请求异常
    /// </summary>
    public class BadRequestException : AppException
    {
        /// <summary>
        /// 创建一个错误请求异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public BadRequestException(string message) 
            : base(message, HttpStatusCode.BadRequest)
        {
        }

        /// <summary>
        /// 创建一个错误请求异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public BadRequestException(string message, Exception innerException) 
            : base(message, innerException, HttpStatusCode.BadRequest)
        {
        }
    }
} 