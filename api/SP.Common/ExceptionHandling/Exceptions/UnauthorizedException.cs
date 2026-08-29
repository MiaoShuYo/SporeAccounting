using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions
{
    /// <summary>
    /// 未授权异常
    /// </summary>
    public class UnauthorizedException : AppException
    {
        /// <summary>
        /// 创建一个未授权异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public UnauthorizedException(string message = "您没有权限执行此操作") 
            : base(message, HttpStatusCode.Unauthorized)
        {
        }

        /// <summary>
        /// 创建一个未授权异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public UnauthorizedException(string message, Exception innerException) 
            : base(message, innerException, HttpStatusCode.Unauthorized)
        {
        }
    }
} 