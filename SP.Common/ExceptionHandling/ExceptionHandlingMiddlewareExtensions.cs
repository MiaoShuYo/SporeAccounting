using Microsoft.AspNetCore.Builder;
using SP.Common.Middleware;

namespace SP.Common.ExceptionHandling
{
    /// <summary>
    /// 异常处理中间件扩展
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// 使用全局异常处理中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        /// <summary>
        /// 启用请求缓冲，使请求体可以被多次读取
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseRequestBuffering(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnableRequestBufferingMiddleware>();
        }

        /// <summary>
        /// 使用全局异常处理（包括请求缓冲）
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseFullExceptionHandling(this IApplicationBuilder builder)
        {
            return builder
                .UseRequestBuffering()
                .UseExceptionHandling();
        }
    }
} 