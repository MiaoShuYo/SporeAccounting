using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions;

/// <summary>
/// Refit 异常
/// </summary>
public class RefitException : AppException
{
    /// <summary>
    /// 创建一个Refit异常实例
    /// </summary>
    /// <param name="message">错误消息</param>
    public RefitException(string message) 
        : base(message, HttpStatusCode.InternalServerError)
    {
    }

    /// <summary>
    /// 创建一个Refit异常实例
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="innerException">内部异常</param>
    public RefitException(string message, Exception innerException) 
        : base(message, innerException, HttpStatusCode.InternalServerError)
    {
    }
}