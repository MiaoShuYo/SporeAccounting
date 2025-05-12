namespace SP.Common.Redis
{
    /// <summary>
    /// Redis配置选项
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "localhost:6379";
        
        /// <summary>
        /// 默认数据库索引
        /// </summary>
        public int DefaultDatabase { get; set; } = 0;
        
        /// <summary>
        /// 连接池大小
        /// </summary>
        public int PoolSize { get; set; } = 50;
        
        /// <summary>
        /// 连接空闲超时时间(秒)
        /// </summary>
        public int ConnectionIdleTimeout { get; set; } = 180;
        
        /// <summary>
        /// 连接超时时间(毫秒)
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;
        
        /// <summary>
        /// 默认缓存过期时间(秒)
        /// </summary>
        public int DefaultExpireSeconds { get; set; } = 3600;
    }
} 