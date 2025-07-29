using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions
{
    /// <summary>
    /// 禁止访问异常
    /// </summary>
    public class ForbiddenException : AppException
    {
        /// <summary>
        /// 创建一个禁止访问异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public ForbiddenException(string message = "禁止访问该资源") 
            : base(message, HttpStatusCode.Forbidden)
        {
        }

        /// <summary>
        /// 创建一个禁止访问异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public ForbiddenException(string message, Exception innerException) 
            : base(message, innerException, HttpStatusCode.Forbidden)
        {
        }
    }
} 