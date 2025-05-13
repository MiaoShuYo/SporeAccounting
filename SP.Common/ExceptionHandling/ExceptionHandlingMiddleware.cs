using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SP.Common.ExceptionHandling
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">请求委托</param>
        /// <param name="logger">日志记录器</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
    }
} 