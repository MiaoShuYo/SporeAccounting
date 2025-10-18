using Refit;
using SP.Common.Model.Enumeration;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.RefitClient;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigServiceApi
{
    /// <summary>
    /// 根据类型获取配置
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Get("/api/configs/by-type/{type}")]
    Task<ApiResponse<ConfigResponse>> QueryByType(ConfigTypeEnum type);

    /// <summary>
    /// 根据类型和userId获取配置
    /// </summary>
    /// <param name="type"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [Get("/api/configs/by-type-and-user/{type}/{userId}")]
    Task<ApiResponse<ConfigResponse>> QueryByTypeAndUserId(ConfigTypeEnum type, long userId);
}