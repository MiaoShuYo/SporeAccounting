using SP.ConfigService.Models.Request;
using SP.ConfigService.Models.Response;

namespace SP.ConfigService.Service;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigServer
{
    /// <summary>
    /// 查询用户配置
    /// </summary>
    /// <returns>用户配置</returns>
    Task<List<ConfigResponse>> GetConfig();
    
    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="config">配置更新请求</param>
    Task UpdateConfig(ConfigEditRequest config);
    
    /// <summary>
    /// 设置用户默认货币
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="defaultCurrencyId"></param>
    /// <returns></returns>
    Task SetUserDefaultCurrencyAsync(long userId, string defaultCurrencyId);
}