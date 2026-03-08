namespace SP.Common.ServiceDiscovery;

/// <summary>
/// 服务发现接口
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// 解析服务地址
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="groupName">组名称</param>
    /// <param name="clusterName">集群名称</param>
    /// <param name="scheme">协议</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    Task<Uri> ResolveAsync(string serviceName, string groupName, string clusterName, string scheme,
        CancellationToken ct = default);

    /// <summary>
    /// 列出服务地址
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="groupName">组名称</param>
    /// <param name="clusterName">集群名称</param>
    /// <param name="scheme">协议</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    Task<IReadOnlyList<Uri>> ListAsync(string serviceName, string groupName, string clusterName, string scheme,
        CancellationToken ct = default);
}