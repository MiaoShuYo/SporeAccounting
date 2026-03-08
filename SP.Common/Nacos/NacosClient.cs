using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SP.Common.Nacos.Models;

namespace SP.Common.Nacos;

/// <summary>
/// 基于 Nacos OpenAPI 的最小封装：服务发现/注册 + 配置读取/轮询拉取。
/// </summary>
public sealed class NacosClient : INacosClient
{
    private readonly HttpClient _http;
    private readonly NacosOptions _options;

    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _tokenExpiresAt;

    public NacosClient(HttpClient http, IOptions<NacosOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<NacosInstance>> ListHealthyInstancesAsync(
        string serviceName,
        string? groupName = null,
        string? clusterName = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException(nameof(serviceName));

        var query = new Dictionary<string, string?>
        {
            ["serviceName"] = serviceName,
            ["groupName"] = string.IsNullOrWhiteSpace(groupName) ? _options.GroupName : groupName,
            ["clusters"] = string.IsNullOrWhiteSpace(clusterName) ? _options.ClusterName : clusterName,
            ["healthyOnly"] = "true",
            ["namespaceId"] = _options.Namespace
        };
        await EnsureAuthAsync(query, ct);

        var url = "/nacos/v1/ns/instance/list" + BuildQueryString(query);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        var parsed = JsonSerializer.Deserialize<NacosInstanceListResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var hosts = parsed?.Hosts ?? new List<NacosInstance>();
        return hosts.Where(h => h is { Enabled: true, Healthy: true }).ToList();
    }

    public async Task<Uri> ResolveAsync(
        string serviceName,
        string? groupName = null,
        string? clusterName = null,
        string scheme = "http",
        CancellationToken ct = default)
    {
        var instances = await ListHealthyInstancesAsync(serviceName, groupName, clusterName, ct);
        if (instances.Count == 0)
            throw new InvalidOperationException($"No healthy instance for {serviceName}");

        // 简单随机：避免所有请求打到同一个实例。
        var ins = instances[Random.Shared.Next(instances.Count)];
        var finalScheme = ins.Metadata?.GetValueOrDefault("scheme", scheme) ?? scheme;
        return new Uri($"{finalScheme}://{ins.Ip}:{ins.Port}");
    }

    public async Task RegisterInstanceAsync(
        string serviceName,
        string ip,
        int port,
        string? groupName = null,
        string? clusterName = null,
        double? weight = null,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException(nameof(serviceName));
        if (string.IsNullOrWhiteSpace(ip)) throw new ArgumentException(nameof(ip));
        if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));

        var form = new Dictionary<string, string?>
        {
            ["serviceName"] = serviceName,
            ["ip"] = ip,
            ["port"] = port.ToString(),
            ["groupName"] = string.IsNullOrWhiteSpace(groupName) ? _options.GroupName : groupName,
            ["clusterName"] = string.IsNullOrWhiteSpace(clusterName) ? _options.ClusterName : clusterName,
            ["namespaceId"] = _options.Namespace,
            ["enabled"] = _options.InstanceEnabled ? "true" : "false",
            ["ephemeral"] = _options.Ephemeral ? "true" : "false"
        };
        if (weight is not null) form["weight"] = weight.Value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        if (metadata is { Count: > 0 })
            form["metadata"] = JsonSerializer.Serialize(metadata);

        await EnsureAuthAsync(form, ct);

        using var req = new HttpRequestMessage(HttpMethod.Post, "/nacos/v1/ns/instance")
        {
            Content = new FormUrlEncodedContent(form.Where(kv => kv.Value is not null)
                .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value!)))
        };

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeregisterInstanceAsync(
        string serviceName,
        string ip,
        int port,
        string? groupName = null,
        string? clusterName = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException(nameof(serviceName));
        if (string.IsNullOrWhiteSpace(ip)) throw new ArgumentException(nameof(ip));
        if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));

        var query = new Dictionary<string, string?>
        {
            ["serviceName"] = serviceName,
            ["ip"] = ip,
            ["port"] = port.ToString(),
            ["groupName"] = string.IsNullOrWhiteSpace(groupName) ? _options.GroupName : groupName,
            ["clusterName"] = string.IsNullOrWhiteSpace(clusterName) ? _options.ClusterName : clusterName,
            ["namespaceId"] = _options.Namespace
        };
        await EnsureAuthAsync(query, ct);

        var url = "/nacos/v1/ns/instance" + BuildQueryString(query);
        using var req = new HttpRequestMessage(HttpMethod.Delete, url);
        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetConfigAsync(
        string dataId,
        string? group = null,
        string? tenant = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dataId)) throw new ArgumentException(nameof(dataId));

        var query = new Dictionary<string, string?>
        {
            ["dataId"] = dataId,
            ["group"] = string.IsNullOrWhiteSpace(group) ? _options.GroupName : group,
            ["tenant"] = tenant ?? _options.Namespace
        };
        await EnsureAuthAsync(query, ct);

        var url = "/nacos/v1/cs/configs" + BuildQueryString(query);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await _http.SendAsync(req, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(ct);
    }

    public async IAsyncEnumerable<string> WatchConfigAsync(
        string dataId,
        string? group = null,
        string? tenant = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var pollMs = _options.ConfigPollIntervalMs <= 0 ? 5_000 : _options.ConfigPollIntervalMs;

        string? last = await GetConfigAsync(dataId, group, tenant, ct);
        if (last is not null)
            yield return last;

        var lastHash = ComputeHash(last);

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(pollMs, ct);

            var current = await GetConfigAsync(dataId, group, tenant, ct);
            var currentHash = ComputeHash(current);

            if (!string.Equals(lastHash, currentHash, StringComparison.Ordinal))
            {
                last = current;
                lastHash = currentHash;
                if (current is not null)
                    yield return current;
            }
        }
    }

    private static string ComputeHash(string? content)
    {
        if (content is null) return "__null__";
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    private async Task EnsureAuthAsync(IDictionary<string, string?> queryOrForm, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.Username) || string.IsNullOrWhiteSpace(_options.Password))
            return;

        if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            queryOrForm["accessToken"] = _accessToken;
            return;
        }

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _tokenExpiresAt)
            {
                queryOrForm["accessToken"] = _accessToken;
                return;
            }

            var form = new Dictionary<string, string?>
            {
                ["username"] = _options.Username,
                ["password"] = _options.Password
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "/nacos/v1/auth/login")
            {
                Content = new FormUrlEncodedContent(form.Where(kv => kv.Value is not null)
                    .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value!)))
            };

            using var resp = await _http.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync(ct);
            var login = JsonSerializer.Deserialize<NacosLoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (string.IsNullOrWhiteSpace(login?.AccessToken))
                throw new InvalidOperationException("Nacos login succeeded but accessToken is missing.");

            _accessToken = login.AccessToken;
            // 给 token 预留一点缓冲
            var ttl = login.TokenTtlSeconds <= 0 ? 60 : login.TokenTtlSeconds;
            _tokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(10, ttl - 10));

            queryOrForm["accessToken"] = _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string?> query)
    {
        var pairs = query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}")
            .ToArray();

        return pairs.Length == 0 ? string.Empty : "?" + string.Join("&", pairs);
    }
}

