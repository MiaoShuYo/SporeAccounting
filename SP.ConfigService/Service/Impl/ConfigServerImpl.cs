using System.Configuration;
using AutoMapper;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.ConfigService.DB;
using SP.ConfigService.Models.Entity;
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
    /// 配置服务实现类构造函数
    /// </summary>
    /// <param name="context">配置服务数据库上下文</param>
    /// <param name="contextSession">上下文会话</param>
    /// <param name="mapper">自动映射器</param>
    public ConfigServerImpl(ConfigServiceDbContext context, ContextSession contextSession, IMapper mapper)
    {
        _contextSession = contextSession;
        _context = context;
        _autoMapper = mapper;
    }

    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <returns>用户配置</returns>
    public List<ConfigResponse> GetConfig()
    {
        long userId = _contextSession.UserId;
        // 查询用户配置
        List<Config> configs = _context.Configs
            .Where(c => c.UserId == userId)
            .ToList();
        // 将配置实体转换为响应模型
        List<ConfigResponse> configResponses = _autoMapper.Map<List<ConfigResponse>>(configs);
        return configResponses;
    }

    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="config">配置更新请求</param>
    public void UpdateConfig(ConfigResponse config)
    {
        long userId = _contextSession.UserId;
        // 查询用户配置
        Config? existingConfig = _context.Configs
            .FirstOrDefault(c => c.Id == config.Id);
        if (existingConfig == null)
        {
            throw new NotFoundException("配置项不存在");
        }

        existingConfig.Value = config.Value;
        SettingCommProperty.Edit(existingConfig);
        _context.Configs.Update(existingConfig);
        // 保存更改到数据库
        _context.SaveChanges();
    }
}