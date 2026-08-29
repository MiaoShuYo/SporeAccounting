using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SP.Common.LLM
{
    /// <summary>
    /// OpenAI 服务扩展方法
    /// </summary>
    public static class OpenAIServiceExtensions
    {
        /// <summary>
        /// 注册 OpenAI 服务（<see cref="IOpenAIService"/>）到依赖注入容器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">应用配置，需包含 "OpenAI" 节点</param>
        /// <returns>服务集合，支持链式调用</returns>
        public static IServiceCollection AddOpenAIService(this IServiceCollection services, IConfiguration configuration)
        {
            var openAIOptions = configuration.GetSection("OpenAI").Get<OpenAIOptions>()
                ?? throw new InvalidOperationException("配置中缺少 \"OpenAI\" 节点。");

            services.AddSingleton(openAIOptions);
            services.AddSingleton<IOpenAIService, OpenAIService>();

            return services;
        }
    }
}
