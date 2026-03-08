using Microsoft.Extensions.Logging;
using SP.Common.ServiceDiscovery;

namespace SP.Common.Refit;

/// <summary>
/// DelegatingHandler 基于 Nacos 动态解析服务实例并重写请求目标地址。
/// </summary>
public sealed class NacosDiscoveryHandler : DelegatingHandler
{
    /// <summary>
    /// 服务发现
    /// </summary>
    private readonly IServiceDiscovery _discovery;

    /// <summary>
    /// 服务名
    /// </summary>
    private readonly string _serviceName;

    /// <summary>
    /// 组名
    /// </summary>
    private readonly string _groupName;

    /// <summary>
    /// 集群名
    /// </summary>
    private readonly string _clusterName;

    /// <summary>
    /// 下游服务协议
    /// </summary>
    private readonly string _scheme;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<NacosDiscoveryHandler> _logger;

    /// <summary>
    /// 构造函数，初始化 NacosDiscoveryHandler。
    /// </summary>
    /// <param name="discovery">服务发现</param>
    /// <param name="serviceName">服务名</param>
    /// <param name="groupName">组名</param>
    /// <param name="clusterName">集群名</param>
    /// <param name="downstreamScheme">下游服务协议</param>
    /// <param name="logger">日志</param>
    /// <exception cref="ArgumentException"></exception>
    public NacosDiscoveryHandler(
        IServiceDiscovery discovery,
        string serviceName,
        string groupName,
        string clusterName,
        string downstreamScheme,
        ILogger<NacosDiscoveryHandler> logger)
    {
        _discovery = discovery;
        _serviceName = string.IsNullOrWhiteSpace(serviceName)
            ? throw new ArgumentException(nameof(serviceName))
            : serviceName;
        _groupName = string.IsNullOrWhiteSpace(groupName) ? "DEFAULT_GROUP" : groupName;
        _clusterName = string.IsNullOrWhiteSpace(clusterName) ? "DEFAULT" : clusterName;
        _scheme = string.IsNullOrWhiteSpace(downstreamScheme) ? "http" : downstreamScheme;
        _logger = logger;
    }
    
    /// <summary>
    /// 重写 SendAsync 方法，在发送请求前解析服务地址并重写 RequestUri。
    /// </summary>
    /// <param name="request">请求消息</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>响应消息</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.RequestUri == null) throw new InvalidOperationException("RequestUri cannot be null");

        var baseUri = await _discovery.ResolveAsync(_serviceName, _groupName, _clusterName, _scheme, ct);
        var newUri = new Uri(baseUri, request.RequestUri.PathAndQuery);
        request.RequestUri = newUri;

        _logger.LogDebug("Resolved {Service} -> {Uri}", _serviceName, newUri);
        return await base.SendAsync(request, ct);
    }
}