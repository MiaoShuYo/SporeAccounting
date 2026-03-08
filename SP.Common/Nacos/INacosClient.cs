using SP.Common.Nacos.Models;

namespace SP.Common.Nacos;

/// <summary>
/// Nacos 调用封装（OpenAPI）。
/// </summary>
public interface INacosClient
{
    /// <summary>
    /// 查询指定服务的健康实例列表。
    /// </summary>
    Task<IReadOnlyList<NacosInstance>> ListHealthyInstancesAsync(
        string serviceName,
        string? groupName = null,
        string? clusterName = null,
        CancellationToken ct = default);

    /// <summary>
    /// 选择一个健康实例并拼出 Uri。
    /// </summary>
    Task<Uri> ResolveAsync(
        string serviceName,
        string? groupName = null,
        string? clusterName = null,
        string scheme = "http",
        CancellationToken ct = default);

    /// <summary>
    /// 注册服务实例。
    /// </summary>
    Task RegisterInstanceAsync(
        string serviceName,
        string ip,
        int port,
        string? groupName = null,
        string? clusterName = null,
        double? weight = null,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default);

    /// <summary>
    /// 注销服务实例。
    /// </summary>
    Task DeregisterInstanceAsync(
        string serviceName,
        string ip,
        int port,
        string? groupName = null,
        string? clusterName = null,
        CancellationToken ct = default);

    /// <summary>
    /// 获取配置内容（Config Center）。
    /// </summary>
    Task<string?> GetConfigAsync(
        string dataId,
        string? group = null,
        string? tenant = null,
        CancellationToken ct = default);

    /// <summary>
    /// 轮询拉取配置变化（pull）。首次会先返回当前配置（若存在）。
    /// </summary>
    IAsyncEnumerable<string> WatchConfigAsync(
        string dataId,
        string? group = null,
        string? tenant = null,
        CancellationToken ct = default);
}
