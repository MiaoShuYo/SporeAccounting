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
    List<ConfigResponse> GetConfig();
    
    /// <summary>
    /// 更新用户配置
    /// </summary>
    /// <param name="config">配置更新请求</param>
    void UpdateConfig(ConfigResponse config);
}