using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 用户配置服务实现
/// </summary>
public class ConfigImp : IConfigServer
{
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    public ConfigImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    public Config? Query(string configId)
    {
        try
        {
            return _sporeAccountingDbContext.Configs.FirstOrDefault(c =>
                c.Id == configId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configTypeEnum"></param>
    /// <returns></returns>
    public Config? Query(string userId, ConfigTypeEnum configTypeEnum)
    {
        try
        {
            return _sporeAccountingDbContext.Configs.FirstOrDefault(c =>
                c.UserId == userId && c.ConfigTypeEnum == configTypeEnum);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configId"></param>
    /// <param name="value"></param>
    public void Update(string userId, string configId, string value)
    {
        try
        {
            var config = _sporeAccountingDbContext.Configs.FirstOrDefault(c =>
                c.UserId == userId && c.Id == configId);
            if (config != null)
            {
                config.Value = value;
                _sporeAccountingDbContext.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 新增用户配置
    /// </summary>
    /// <param name="config"></param>
    public void Add(Config config)
    {
        try
        {
            _sporeAccountingDbContext.Configs.Add(config);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 用户配置是否存在
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configTypeEnum"></param>
    /// <returns></returns>
    public bool IsExist(string userId, ConfigTypeEnum configTypeEnum)
    {
        try
        {
            return _sporeAccountingDbContext.Configs.Any(c =>
                c.UserId == userId && c.ConfigTypeEnum == configTypeEnum);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 用户配置是否存在
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configId"></param>
    /// <returns></returns>
    public bool IsExist(string userId, string configId)
    {
        try
        {
            return _sporeAccountingDbContext.Configs.Any(c =>
                c.UserId == userId && c.Id == configId);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}