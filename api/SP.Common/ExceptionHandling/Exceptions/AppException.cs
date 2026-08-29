using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions
{
    /// <summary>
    /// 应用程序自定义异常基类
    /// </summary>
    public class AppException : Exception
    {
        /// <summary>
        /// HTTP状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// 创建一个应用程序异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="statusCode">HTTP状态码</param>
        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// 创建一个应用程序异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        /// <param name="statusCode">HTTP状态码</param>
        public AppException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
} 