using Nacos.V2;

namespace SP.FinanceService.RefitClient
{
    /// <summary>
    /// DelegatingHandler 基于 Nacos 动态解析服务实例并重写请求目标地址。
    /// 每次请求前从 Nacos 选择一个健康实例，组合请求完整 Uri。
    /// </summary>
    public sealed class NacosDiscoveryHandler : DelegatingHandler
    {
        private readonly INacosNamingService _nacosNamingService;
        private readonly string _serviceName;
        private readonly string _groupName;
        private readonly string? _clusterName;
        private readonly string _downstreamScheme;
        private readonly ILogger<NacosDiscoveryHandler> _logger;

        /// <summary>
        /// 构造函数，初始化 NacosDiscoveryHandler。
        /// </summary>
        /// <param name="nacosNamingService"></param>
        /// <param name="serviceName"></param>
        /// <param name="groupName"></param>
        /// <param name="clusterName"></param>
        /// <param name="downstreamScheme"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public NacosDiscoveryHandler(
            INacosNamingService nacosNamingService,
            string serviceName,
            string groupName,
            string? clusterName,
            string downstreamScheme,
            ILogger<NacosDiscoveryHandler> logger)
        {
            _nacosNamingService = nacosNamingService ?? throw new ArgumentNullException(nameof(nacosNamingService));
            _serviceName = !string.IsNullOrWhiteSpace(serviceName)
                ? serviceName
                : throw new ArgumentException("serviceName is required", nameof(serviceName));
            _groupName = string.IsNullOrWhiteSpace(groupName) ? "DEFAULT_GROUP" : groupName;
            _clusterName = string.IsNullOrWhiteSpace(clusterName) ? "DEFAULT" : clusterName;
            _downstreamScheme = string.IsNullOrWhiteSpace(downstreamScheme) ? "http" : downstreamScheme;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.RequestUri == null)
            {
                throw new InvalidOperationException("RequestUri cannot be null");
            }

            // 选择一个健康实例
            var clusters = new List<string> { _clusterName! };
            var instance = await _nacosNamingService.SelectOneHealthyInstance(_serviceName, _groupName, clusters)
                .ConfigureAwait(false);

            if (instance == null)
            {
                throw new InvalidOperationException($"No healthy instance found in Nacos for service '{_serviceName}'");
            }

            var baseUri = new Uri($"{_downstreamScheme}://{instance.Ip}:{instance.Port}");

            // 组合新的下游地址，仅保留原 PathAndQuery
            var newUri = new Uri(baseUri, request.RequestUri.PathAndQuery);
            request.RequestUri = newUri;

            _logger.LogDebug("Resolved {ServiceName} -> {Resolved}", _serviceName, newUri);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}