namespace SP.Common.Redis
{
    /// <summary>
    /// Redis服务接口
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>字符串值</returns>
        Task<string?> GetStringAsync(string key);

        /// <summary>
        /// 设置字符串值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expirySeconds">过期时间(秒)，默认使用配置中的默认过期时间</param>
        /// <returns>是否成功</returns>
        Task<bool> SetStringAsync(string key, string value, int? expirySeconds = null);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>对象</returns>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// 设置对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expirySeconds">过期时间(秒)，默认使用配置中的默认过期时间</param>
        /// <returns>是否成功</returns>
        Task<bool> SetAsync<T>(string key, T value, int? expirySeconds = null) where T : class;

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expirySeconds">过期时间(秒)</param>
        /// <returns>是否成功</returns>
        Task<bool> SetExpiryAsync(string key, int expirySeconds);

        /// <summary>
        /// 批量获取
        /// </summary>
        /// <param name="keys">键集合</param>
        /// <returns>值字典</returns>
        Task<Dictionary<string, string>> GetAllStringAsync(IEnumerable<string> keys);

        /// <summary>
        /// 获取所有匹配的键
        /// </summary>
        /// <param name="pattern">匹配模式</param>
        /// <returns>键集合</returns>
        Task<IEnumerable<string>> GetKeysAsync(string pattern);

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <param name="key">Hash键</param>
        /// <param name="field">字段</param>
        /// <returns>值</returns>
        Task<string?> HashGetAsync(string key, string field);

        /// <summary>
        /// 设置Hash值
        /// </summary>
        /// <param name="key">Hash键</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        Task<bool> HashSetAsync(string key, string field, string value);

        /// <summary>
        /// 获取所有Hash值
        /// </summary>
        /// <param name="key">Hash键</param>
        /// <returns>字段值字典</returns>
        Task<Dictionary<string, string>> HashGetAllAsync(string key);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="message">消息</param>
        /// <returns>接收到消息的客户端数量</returns>
        Task<long> PublishAsync(string channel, string message);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="key">锁键</param>
        /// <param name="expiry">锁过期时间</param>
        /// <returns>是否成功获取锁</returns>
        Task<bool> LockAsync(string key, TimeSpan expiry);

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        /// <param name="key">锁键</param>
        /// <returns>是否成功释放锁</returns>
        Task<bool> UnlockAsync(string key);
    }
} 