using Microsoft.AspNetCore.Http;

namespace SP.Common.Middleware
{
    /// <summary>
    /// 启用请求缓冲中间件，使请求体可以被多次读取
    /// </summary>
    public class EnableRequestBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">请求委托</param>
        public EnableRequestBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context)
        {
            // 启用请求缓冲，使请求体可以被多次读取
            context.Request.EnableBuffering();

            await _next(context);
        }
    }
} 