using System.Net;

namespace SP.Common.ExceptionHandling.Exceptions;

/// <summary>
/// 业务异常
/// </summary>
public class BusinessException : AppException
{
    public BusinessException(string message = "业务异常",
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(
        message, statusCode)
    {
    }

    public BusinessException(string message, Exception innerException,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message, innerException, statusCode)
    {
    }
}