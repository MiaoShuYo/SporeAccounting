using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 用户配置服务接口
/// </summary>
public interface IConfigServer
{
    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    Config? Query(string configId);

    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configTypeEnum"></param>
    /// <returns></returns>
    Config? Query(string userId, ConfigTypeEnum configTypeEnum);

    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configId"></param>
    /// <param name="value"></param>
    void Update(string userId, string configId, string value);

    /// <summary>
    /// 新增用户配置
    /// </summary>
    /// <param name="config"></param>
    void Add(Config config);

    /// <summary>
    /// 用户配置是否存在
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configTypeEnum"></param>
    /// <returns></returns>
    bool IsExist(string userId, ConfigTypeEnum configTypeEnum);

    /// <summary>
    /// 用户配置是否存在
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="configId"></param>
    /// <returns></returns>
    bool IsExist(string userId, string configId);
}