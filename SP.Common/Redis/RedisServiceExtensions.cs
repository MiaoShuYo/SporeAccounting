 using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SP.Common.Redis
{
    /// <summary>
    /// Redis服务扩展方法
    /// </summary>
    public static class RedisServiceExtensions
    {
        /// <summary>
        /// 添加Redis服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRedisService(this IServiceCollection services, IConfiguration configuration)
        {
            // 从配置中获取Redis节点
            var redisSection = configuration.GetSection("Redis");

            // 注册RedisOptions
            services.Configure<RedisOptions>(options =>
            {
                // 将配置节点中的值绑定到options对象
                if (redisSection["ConnectionString"] != null)
                    options.ConnectionString = redisSection["ConnectionString"];
                
                if (int.TryParse(redisSection["DefaultDatabase"], out int defaultDb))
                    options.DefaultDatabase = defaultDb;
                
                if (int.TryParse(redisSection["PoolSize"], out int poolSize))
                    options.PoolSize = poolSize;
                
                if (int.TryParse(redisSection["ConnectionIdleTimeout"], out int idleTimeout))
                    options.ConnectionIdleTimeout = idleTimeout;
                
                if (int.TryParse(redisSection["ConnectTimeout"], out int connectTimeout))
                    options.ConnectTimeout = connectTimeout;
                
                if (int.TryParse(redisSection["DefaultExpireSeconds"], out int expireSeconds))
                    options.DefaultExpireSeconds = expireSeconds;
            });
            
            // 注册Redis服务
            services.AddSingleton<IRedisService, RedisService>();
            
            return services;
        }
        
        /// <summary>
        /// 添加Redis服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRedisService(this IServiceCollection services, string connectionString)
        {
            // 注册RedisOptions
            services.Configure<RedisOptions>(options =>
            {
                options.ConnectionString = connectionString;
            });
            
            // 注册Redis服务
            services.AddSingleton<IRedisService, RedisService>();
            
            return services;
        }
    }
}