using System.Net;

namespace SP.Common.ExceptionHandling
{
    /// <summary>
    /// 异常响应类
    /// </summary>
    public class ExceptionResponse
    {
        /// <summary>
        /// HTTP状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// 堆栈跟踪（仅在DEBUG模式下返回）
        /// </summary>
        public string? StackTrace { get; set; }
    }
} 