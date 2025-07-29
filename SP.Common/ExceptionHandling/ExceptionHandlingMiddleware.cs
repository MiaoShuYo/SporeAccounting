using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Logger;

namespace SP.Common.ExceptionHandling
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ILoggerService _loggerService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">请求委托</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="loggerService">日志服务</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger,
            ILoggerService loggerService)
        {
            _next = next;
            _logger = logger;
            _loggerService = loggerService;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 记录详细异常信息到日志
            _logger.LogError(exception, "处理请求时发生未处理的异常: {Message}", exception.Message);
            
            // 记录到Loki日志（包含更详细的信息）
            LogExceptionToLoki(context, exception);

            // 设置响应内容类型
            context.Response.ContentType = "application/json";

            // 获取状态码和错误消息
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string errorMessage = "服务器内部错误，请稍后再试";
            object errors = null;

            // 根据异常类型设置不同的状态码和错误消息
            if (exception is ValidationException validationException)
            {
                statusCode = validationException.StatusCode;
                errorMessage = validationException.Message;
                errors = validationException.Errors;
            }
            else if (exception is AppException appException)
            {
                statusCode = appException.StatusCode;
                errorMessage = appException.Message;
            }
            else if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                errorMessage = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                errorMessage = "未授权访问";
            }
            // 可以根据需要添加更多的异常类型处理

            // 设置响应状态码
            context.Response.StatusCode = (int)statusCode;

            // 创建异常响应对象
            var response = new ExceptionResponse
            {
                StatusCode = statusCode,
                ErrorMessage = errorMessage
            };

            // 如果有验证错误，添加到响应中
            if (errors != null)
            {
                var jsonResponse = JsonSerializer.Serialize(new
                {
                    response.StatusCode,
                    response.ErrorMessage,
                    Errors = errors
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            // 在开发环境下可以返回详细的异常信息，生产环境下只返回友好消息
            #if DEBUG
            if (!(exception is AppException))
            {
                response.ErrorMessage = exception.Message;
            }
            response.StackTrace = exception.StackTrace;
            #endif

            // 序列化响应对象
            var jsonResponseDefault = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // 写入响应
            await context.Response.WriteAsync(jsonResponseDefault);
        }

        /// <summary>
        /// 记录异常到Loki日志
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="exception">异常</param>
        private void LogExceptionToLoki(HttpContext context, Exception exception)
        {
            try
            {
                // 收集请求信息
                var request = context.Request;
                var requestPath = request.Path;
                var requestMethod = request.Method;
                var requestQuery = request.QueryString.ToString();
                var requestHeaders = SerializeHeaders(request.Headers);
                string requestBody = "未捕获";

                try
                {
                    // 如果请求是可重置的，尝试读取body
                    if (request.Body.CanSeek)
                    {
                        var position = request.Body.Position;
                        request.Body.Position = 0;
                        using var reader = new StreamReader(request.Body, leaveOpen: true);
                        requestBody = reader.ReadToEndAsync().GetAwaiter().GetResult();
                        request.Body.Position = position;
                    }
                }
                catch
                {
                    // 忽略读取请求体的错误
                }

                // 构建详细的日志消息
                var errorLogModel = new
                {
                    RequestInfo = new
                    {
                        Url = requestPath,
                        Method = requestMethod,
                        QueryString = requestQuery,
                        Headers = requestHeaders,
                        Body = requestBody
                    },
                    ExceptionInfo = new
                    {
                        Message = exception.Message,
                        ExceptionType = exception.GetType().FullName,
                        StackTrace = exception.StackTrace,
                        InnerException = exception.InnerException?.Message
                    },
                    User = GetUserInfo(context),
                    Timestamp = DateTime.UtcNow
                };

                // 序列化为JSON以便于在Loki中查看
                var errorLogJson = JsonSerializer.Serialize(errorLogModel, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // 记录到Loki
                _loggerService.LogError(exception, "处理请求发生异常: {ErrorDetails}", errorLogJson);
            }
            catch (Exception ex)
            {
                // 如果记录日志本身出错，使用标准日志记录
                _logger.LogError(ex, "记录异常到Loki时发生错误");
            }
        }

        /// <summary>
        /// 序列化请求头
        /// </summary>
        /// <param name="headers">请求头集合</param>
        /// <returns>序列化后的请求头</returns>
        private Dictionary<string, string> SerializeHeaders(IHeaderDictionary headers)
        {
            var result = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                // 排除敏感信息，如Authorization、Cookie等
                if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                    !header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    result[header.Key] = header.Value.ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>用户信息</returns>
        private object GetUserInfo(HttpContext context)
        {
            try
            {
                var userId = context.User?.Identity?.Name;
                var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

                return new
                {
                    UserId = userId,
                    IsAuthenticated = isAuthenticated,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString()
                };
            }
            catch
            {
                return new { IpAddress = context.Connection.RemoteIpAddress?.ToString() };
            }
        }
    }
} 