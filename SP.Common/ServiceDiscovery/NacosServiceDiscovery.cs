using Nacos.V2;

namespace SP.Common.ServiceDiscovery;

/// <summary>
/// Nacos服务发现
/// </summary>
public class NacosServiceDiscovery : IServiceDiscovery
{
    /// <summary>
    /// Nacos命名服务
    /// </summary>
    private readonly INacosNamingService _naming;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="naming">Nacos命名服务</param>
    public NacosServiceDiscovery(INacosNamingService naming) => _naming = naming;

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="groupName">分组名称</param>
    /// <param name="clusterName">集群名称</param>
    /// <param name="scheme">协议</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Uri> ResolveAsync(string serviceName, string groupName, string clusterName, string scheme,
        CancellationToken ct = default)
    {
        var instance = await _naming.SelectOneHealthyInstance(
            serviceName,
            string.IsNullOrWhiteSpace(groupName) ? "DEFAULT_GROUP" : groupName,
            new List<string> { string.IsNullOrWhiteSpace(clusterName) ? "DEFAULT" : clusterName });
        if (instance == null) throw new InvalidOperationException($"No healthy instance for {serviceName}");
        var finalScheme = instance.Metadata?.GetValueOrDefault("scheme", scheme) ?? scheme;
        return new Uri($"{finalScheme}://{instance.Ip}:{instance.Port}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="groupName">分组名称</param>
    /// <param name="clusterName">集群名称</param>
    /// <param name="scheme">协议</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    /// <exception cref="NacosException"></exception>
    public async Task<IReadOnlyList<Uri>> ListAsync(string serviceName, string groupName, string clusterName,
        string scheme, CancellationToken ct = default)
    {
        var instances = await _naming.SelectInstances(
            serviceName,
            string.IsNullOrWhiteSpace(groupName) ? "DEFAULT_GROUP" : groupName,
            new List<string> { string.IsNullOrWhiteSpace(clusterName) ? "DEFAULT" : clusterName },
            true);
        var uris = new List<Uri>();
        if (instances != null)
        {
            foreach (var ins in instances)
            {
                if (ins.Enabled && ins.Healthy)
                {
                    var finalScheme = ins.Metadata?.GetValueOrDefault("scheme", scheme) ?? scheme;
                    uris.Add(new Uri($"{finalScheme}://{ins.Ip}:{ins.Port}"));
                }
            }
        }

        return uris;
    }
}