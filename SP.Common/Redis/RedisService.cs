using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace SP.Common.Redis
{
    /// <summary>
    /// Redis服务实现
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly ILogger<RedisService> _logger;
        private readonly RedisOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private readonly string _lockValuePrefix;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Redis配置选项</param>
        /// <param name="logger">日志器</param>
        public RedisService(IOptions<RedisOptions> options, ILogger<RedisService> logger)
        {
            _logger = logger;
            _options = options.Value;
            _lockValuePrefix = $"lock:{Environment.MachineName}:{Guid.NewGuid()}:";
            
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {
                var configOptions = ConfigurationOptions.Parse(_options.ConnectionString);
                configOptions.DefaultDatabase = _options.DefaultDatabase;
                configOptions.ConnectTimeout = _options.ConnectTimeout;
                configOptions.AbortOnConnectFail = false;
                
                return ConnectionMultiplexer.Connect(configOptions);
            });
        }

        /// <summary>
        /// 获取Redis连接
        /// </summary>
        private ConnectionMultiplexer Connection => _connectionMultiplexer.Value;

        /// <summary>
        /// 获取Redis数据库
        /// </summary>
        private IDatabase Database => Connection.GetDatabase();

        /// <summary>
        /// 获取字符串值
        /// </summary>
        public async Task<string?> GetStringAsync(string key)
        {
            try
            {
                var value = await Database.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取字符串值失败，Key: {Key}", key);
                return null;
            }
        }

        /// <summary>
        /// 设置字符串值
        /// </summary>
        public async Task<bool> SetStringAsync(string key, string value, int? expirySeconds = null)
        {
            try
            {
                var expiry = TimeSpan.FromSeconds(expirySeconds ?? _options.DefaultExpireSeconds);
                return await Database.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis设置字符串值失败，Key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var value = await GetStringAsync(key);
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取对象失败，Key: {Key}, Type: {Type}", key, typeof(T).Name);
                return null;
            }
        }

        /// <summary>
        /// 设置对象
        /// </summary>
        public async Task<bool> SetAsync<T>(string key, T value, int? expirySeconds = null) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            try
            {
                var json = JsonSerializer.Serialize(value);
                return await SetStringAsync(key, json, expirySeconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis设置对象失败，Key: {Key}, Type: {Type}", key, typeof(T).Name);
                return false;
            }
        }

        /// <summary>
        /// 删除键
        /// </summary>
        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await Database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis删除键失败，Key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// 键是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await Database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis检查键是否存在失败，Key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        public async Task<bool> SetExpiryAsync(string key, int expirySeconds)
        {
            try
            {
                return await Database.KeyExpireAsync(key, TimeSpan.FromSeconds(expirySeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis设置过期时间失败，Key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// 批量获取
        /// </summary>
        public async Task<Dictionary<string, string>> GetAllStringAsync(IEnumerable<string> keys)
        {
            try
            {
                var keyArray = keys.ToArray();
                var redisKeys = keyArray.Select(k => (RedisKey)k).ToArray();
                var values = await Database.StringGetAsync(redisKeys);

                var result = new Dictionary<string, string>();
                for (var i = 0; i < keyArray.Length; i++)
                {
                    if (values[i].HasValue)
                    {
                        result.Add(keyArray[i], values[i].ToString());
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis批量获取失败");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 获取所有匹配的键
        /// </summary>
        public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            try
            {
                var keys = new List<string>();
                var endpoints = Connection.GetEndPoints();
                
                foreach (var endpoint in endpoints)
                {
                    var server = Connection.GetServer(endpoint);
                    var serverKeys = server.Keys(pattern: pattern).Select(k => (string)k).ToList();
                    keys.AddRange(serverKeys);
                }

                return await Task.FromResult(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取匹配键失败，Pattern: {Pattern}", pattern);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// 获取Hash值
        /// </summary>
        public async Task<string?> HashGetAsync(string key, string field)
        {
            try
            {
                var value = await Database.HashGetAsync(key, field);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取Hash值失败，Key: {Key}, Field: {Field}", key, field);
                return null;
            }
        }

        /// <summary>
        /// 设置Hash值
        /// </summary>
        public async Task<bool> HashSetAsync(string key, string field, string value)
        {
            try
            {
                return await Database.HashSetAsync(key, field, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis设置Hash值失败，Key: {Key}, Field: {Field}", key, field);
                return false;
            }
        }

        /// <summary>
        /// 获取所有Hash值
        /// </summary>
        public async Task<Dictionary<string, string>> HashGetAllAsync(string key)
        {
            try
            {
                var entries = await Database.HashGetAllAsync(key);
                return entries.ToDictionary(
                    entry => entry.Name.ToString(),
                    entry => entry.Value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取所有Hash值失败，Key: {Key}", key);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        public async Task<long> PublishAsync(string channel, string message)
        {
            try
            {
                return await Connection.GetSubscriber().PublishAsync(channel, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis发布消息失败，Channel: {Channel}", channel);
                return 0;
            }
        }

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        public async Task<bool> LockAsync(string key, TimeSpan expiry)
        {
            try
            {
                var lockKey = $"lock:{key}";
                var lockValue = $"{_lockValuePrefix}{DateTime.UtcNow.Ticks}";
                
                // SET命令的NX选项确保键不存在时才设置值
                return await Database.StringSetAsync(lockKey, lockValue, expiry, When.NotExists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis获取分布式锁失败，Key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        public async Task<bool> UnlockAsync(string key)
        {
            try
            {
                var lockKey = $"lock:{key}";
                return await Database.KeyDeleteAsync(lockKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis释放分布式锁失败，Key: {Key}", key);
                return false;
            }
        }
    }
} 