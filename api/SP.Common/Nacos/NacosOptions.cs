namespace SP.Common.Nacos;

/// <summary>
/// Nacos OpenAPI 访问配置。
/// </summary>
public sealed class NacosOptions
{
    /// <summary>
    /// Nacos 服务地址列表，例如：["http://127.0.0.1:8848"]
    /// </summary>
    public string[]? ServerAddresses { get; set; }

    /// <summary>
    /// 命名空间（tenant），为空表示 public。
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// 服务名（用于服务注册）。
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// 分组名（用于服务注册/发现与配置中心）。
    /// </summary>
    public string GroupName { get; set; } = "DEFAULT_GROUP";

    /// <summary>
    /// 集群名（用于服务注册/发现）。
    /// </summary>
    public string ClusterName { get; set; } = "DEFAULT";

    /// <summary>
    /// 是否启用注册（与 RegisterEnabled 配置项对齐）。
    /// </summary>
    public bool RegisterEnabled { get; set; } = true;

    /// <summary>
    /// 实例是否启用（InstanceEnabled）。
    /// </summary>
    public bool InstanceEnabled { get; set; } = true;

    /// <summary>
    /// 是否临时实例（Ephemeral）。
    /// </summary>
    public bool Ephemeral { get; set; } = true;

    /// <summary>
    /// 权重（Weight）。
    /// </summary>
    public double Weight { get; set; } = 100;

    /// <summary>
    /// Nacos 用户名（开启鉴权时必填）。
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Nacos 密码（开启鉴权时必填）。
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 连接超时（毫秒），对齐原 nacos 配置的 ConnectionTimeOut。
    /// </summary>
    public int ConnectionTimeOut { get; set; } = 10_000;

    /// <summary>
    /// 配置拉取轮询间隔（毫秒）。
    /// </summary>
    public int ConfigPollIntervalMs { get; set; } = 5_000;

    /// <summary>
    /// 配置中心监听项（与 Listeners 配置项对齐）。
    /// </summary>
    public List<NacosListenerOptions> Listeners { get; set; } = new();
}

public sealed class NacosListenerOptions
{
    public bool Optional { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string Group { get; set; } = "DEFAULT_GROUP";
}
