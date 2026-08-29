using SP.Common.Nacos;

namespace SP.Common.ServiceDiscovery;

/// <summary>
/// Nacos服务发现
/// </summary>
public class NacosServiceDiscovery : IServiceDiscovery
{
    private readonly INacosClient _nacos;

    public NacosServiceDiscovery(INacosClient nacos) => _nacos = nacos;

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
        return await _nacos.ResolveAsync(serviceName, groupName, clusterName, scheme, ct);
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
        var instances = await _nacos.ListHealthyInstancesAsync(serviceName, groupName, clusterName, ct);
        var uris = new List<Uri>(instances.Count);

        foreach (var ins in instances)
        {
            if (!ins.Enabled || !ins.Healthy) continue;
            var finalScheme = ins.Metadata?.GetValueOrDefault("scheme", scheme) ?? scheme;
            uris.Add(new Uri($"{finalScheme}://{ins.Ip}:{ins.Port}"));
        }

        return uris;
    }
}