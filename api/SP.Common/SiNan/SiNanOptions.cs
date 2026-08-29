namespace SP.Common.SiNan;

/// <summary>
/// SiNan 客户端配置（对应 appsettings.json 中的 sinan 节点）。
/// </summary>
public sealed class SiNanOptions
{
    /// <summary>
    /// SiNan Server 地址，例如 "http://127.0.0.1:5043"。
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5043";

    /// <summary>
    /// API Key（服务端开启鉴权时必填），对应请求头 X-SiNan-Token。
    /// 生产环境建议通过环境变量 SINAN_APIKEY 覆盖，避免明文提交到仓库。
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// 命名空间，默认 "default"。
    /// </summary>
    public string Namespace { get; set; } = "default";

    /// <summary>
    /// 分组名，默认 "DEFAULT_GROUP"。
    /// </summary>
    public string Group { get; set; } = "DEFAULT_GROUP";

    /// <summary>
    /// 服务名（用于服务注册）。
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// 是否启用服务注册，默认 true。
    /// </summary>
    public bool RegisterEnabled { get; set; } = true;

    /// <summary>
    /// 实例权重，默认 100。
    /// </summary>
    public int Weight { get; set; } = 100;

    /// <summary>
    /// 实例心跳 TTL（秒），默认 30。SiNan Server 超过此时间未收到心跳则标记实例为不健康。
    /// </summary>
    public int TtlSeconds { get; set; } = 30;

    /// <summary>
    /// 心跳发送间隔（秒），默认取 TtlSeconds / 3（至少 5 秒）。
    /// </summary>
    public int HeartbeatIntervalSeconds { get; set; } = 0;

    /// <summary>
    /// HTTP 连接超时（毫秒），默认 10000。
    /// </summary>
    public int ConnectionTimeOut { get; set; } = 10_000;

    /// <summary>
    /// SDK 请求失败最大重试次数，默认 2。
    /// </summary>
    public int RetryCount { get; set; } = 2;

    /// <summary>
    /// 初始重试延迟（毫秒），默认 200。
    /// </summary>
    public int RetryDelayMs { get; set; } = 200;

    /// <summary>
    /// 最大重试延迟（毫秒），默认 2000。
    /// </summary>
    public int RetryMaxDelayMs { get; set; } = 2_000;

    /// <summary>
    /// 配置中心轮询间隔（毫秒），默认 5000。
    /// </summary>
    public int ConfigPollIntervalMs { get; set; } = 5_000;

    /// <summary>
    /// 配置中心监听项列表。
    /// </summary>
    public List<SiNanListenerOptions> Listeners { get; set; } = new();

    /// <summary>
    /// 计算实际心跳间隔（秒）。
    /// </summary>
    internal int ResolvedHeartbeatIntervalSeconds =>
        HeartbeatIntervalSeconds > 0
            ? HeartbeatIntervalSeconds
            : Math.Max(5, TtlSeconds / 3);
}

/// <summary>
/// 单个配置中心监听项。
/// </summary>
public sealed class SiNanListenerOptions
{
    /// <summary>
    /// 配置 Key。
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 分组（为空时使用 SiNanOptions.Group）。
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// 为 true 时配置不存在不抛出异常。
    /// </summary>
    public bool Optional { get; set; }
}
