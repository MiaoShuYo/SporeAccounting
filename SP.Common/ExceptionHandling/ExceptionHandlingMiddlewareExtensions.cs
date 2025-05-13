using Microsoft.AspNetCore.Builder;

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
    }
} 