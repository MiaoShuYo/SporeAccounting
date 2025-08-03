using AutoMapper;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.Common.Redis;
using SP.ConfigService.DB;
using SP.ConfigService.Models.Entity;
using SP.ConfigService.Models.Enumeration;
using SP.ConfigService.Models.Request;
using SP.ConfigService.Models.Response;

namespace SP.ConfigService.Service.Impl;

/// <summary>
/// 配置服务实现类
/// </summary>
public class ConfigServerImpl : IConfigServer
{
    /// <summary>
    /// 配置服务上下文
    /// </summary>
    private readonly ConfigServiceDbContext _context;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _autoMapper;

    /// <summary>
    /// Redis服务
    /// </summary>
    private readonly IRedisService _redisService;

    /// <summary>
    /// 用户配置key
    /// </summary>
    private readonly string _redisUserConfigKey;

    /// <summary>
    /// 用户id
    /// </summary>
    private readonly long _userId;

    /// <summary>
    /// 配置服务实现类构造函数
    /// </summary>
    /// <param name="context">配置服务数据库上下文</param>
    /// <param name="contextSession">上下文会话</param>
    /// <param name="mapper">自动映射器</param>
    /// <param name="redisService">Redis服务</param>
    public ConfigServerImpl(ConfigServiceDbContext context, ContextSession contextSession, IMapper mapper,
        IRedisService redisService)
    {
        _redisService = redisService;
        _contextSession = contextSession;
        _context = context;
        _autoMapper = mapper;
        _userId = _contextSession.UserId;
        _redisUserConfigKey = ConfigRedisKey.UserConfig;
        _redisUserConfigKey = string.Format(_redisUserConfigKey, _userId);
    }

    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <returns>用户配置</returns>
    public async Task<List<ConfigResponse>> GetConfig()
    {
        List<ConfigResponse>? config = await _redisService.GetAsync<List<ConfigResponse>>(_redisUserConfigKey);
        if (config != null)
        {
            // 如果redis中存在配置，则直接返回
            return config;
        }

        // 查询用户配置
        List<Config> configs = _context.Configs
            .Where(c => c.UserId == _userId)
            .ToList();
        if (configs == null)
        {
            // 如果没有查询到配置，则抛出异常
            throw new NotFoundException("用户配置不存在");
        }

        // 将配置实体转换为响应模型
        List<ConfigResponse> configResponses = _autoMapper.Map<List<ConfigResponse>>(configs);
        // 将查询出来的数据缓存到Redis中，存储时间为24小时
        await _redisService.SetAsync(_redisUserConfigKey, configResponses, 60 * 60 * 24);

        return configResponses;
    }

    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="config">配置更新请求</param>
    public async Task UpdateConfig(ConfigEditRequest config)
    {
        // 查询用户配置
        Config? existingConfig = _context.Configs
            .FirstOrDefault(c => c.Id == config.Id);
        if (existingConfig == null)
        {
            throw new NotFoundException("配置项不存在");
        }

        existingConfig.Value = config.Value;
        existingConfig.ConfigType = config.ConfigType;
        SettingCommProperty.Edit(existingConfig);
        _context.Configs.Update(existingConfig);
        // 保存更改到数据库
        await _context.SaveChangesAsync();
        // 删除Redis缓存
        await _redisService.RemoveAsync(_redisUserConfigKey);
    }

    /// <summary>
    /// 设置用户默认货币
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="defaultCurrencyId"></param>
    /// <returns></returns>
    public async Task SetUserDefaultCurrencyAsync(long userId, string defaultCurrencyId)
    {
        Config userConfig = new Config();
        // 更新默认货币ID
        userConfig.Value = defaultCurrencyId;
        userConfig.UserId = userId;
        userConfig.ConfigType = ConfigTypeEnum.Currency;
        userConfig.Id = Snow.GetId();
        userConfig.CreateDateTime = DateTime.Now;
        userConfig.CreateUserId = userId;
        _context.Configs.Add(userConfig);

        // 保存到数据库
        await _context.SaveChangesAsync();
        // 删除Redis缓存
        await _redisService.RemoveAsync(_redisUserConfigKey);
    }
}