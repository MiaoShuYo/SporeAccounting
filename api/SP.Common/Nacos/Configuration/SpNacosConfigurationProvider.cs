using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SP.Common.Nacos.Configuration;

internal sealed class SpNacosConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly NacosOptions _options;
    private readonly HttpClient _http;
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _tokenExpiresAt;
    private readonly Dictionary<string, string> _contentHashByKey = new();
    private Task? _pollingTask;

    public SpNacosConfigurationProvider(NacosOptions options)
    {
        _options = options;
        var baseUrl = _options.ServerAddresses?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("nacos:ServerAddresses is required");
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeOut <= 0 ? 10_000 : _options.ConnectionTimeOut)
        };
    }

    public override void Load()
    {
        // initial load
        LoadAsync(_cts.Token).GetAwaiter().GetResult();

        // start polling reload
        var pollMs = _options.ConfigPollIntervalMs <= 0 ? 5_000 : _options.ConfigPollIntervalMs;
        _pollingTask = Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(pollMs, _cts.Token);
                    var changed = await LoadAsync(_cts.Token, reloadOnlyIfChanged: true);
                    if (changed) OnReload();
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
                catch
                {
                    // swallow: config provider should not crash app
                }
            }
        }, _cts.Token);
    }

    private async Task<bool> LoadAsync(CancellationToken ct, bool reloadOnlyIfChanged = false)
    {
        if (_options.Listeners.Count == 0)
            return false;

        var newData = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var anyChanged = false;

        foreach (var listener in _options.Listeners)
        {
            if (string.IsNullOrWhiteSpace(listener.DataId))
                continue;

            var group = string.IsNullOrWhiteSpace(listener.Group) ? _options.GroupName : listener.Group;
            var content = await GetConfigAsync(listener.DataId, group, _options.Namespace, ct);

            if (content is null)
            {
                if (!listener.Optional)
                    throw new InvalidOperationException($"Nacos config not found: dataId={listener.DataId}, group={group}");
                continue;
            }

            var key = $"{listener.DataId}@@{group}";
            var hash = ComputeHash(content);
            if (!_contentHashByKey.TryGetValue(key, out var oldHash) || !string.Equals(oldHash, hash, StringComparison.Ordinal))
            {
                _contentHashByKey[key] = hash;
                anyChanged = true;
            }

            if (reloadOnlyIfChanged && !anyChanged)
                continue;

            MergeContentInto(content, newData);
        }

        if (reloadOnlyIfChanged && !anyChanged)
            return false;

        Data = newData.Where(kv => kv.Value is not null).ToDictionary(kv => kv.Key, kv => kv.Value!, StringComparer.OrdinalIgnoreCase);
        return anyChanged;
    }

    private void MergeContentInto(string content, IDictionary<string, string?> target)
    {
        var trimmed = content.TrimStart();
        if (trimmed.StartsWith("{", StringComparison.Ordinal) || trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            using var doc = JsonDocument.Parse(content);
            FlattenJson(doc.RootElement, parentPath: null, target);
            return;
        }

        // simple key=value lines
        using var reader = new StringReader(content);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var idx = line.IndexOf('=');
            if (idx <= 0) continue;
            var key = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim();
            if (key.Length == 0) continue;
            target[key] = value;
        }
    }

    private static void FlattenJson(JsonElement element, string? parentPath, IDictionary<string, string?> target)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in element.EnumerateObject())
                {
                    var path = parentPath is null ? prop.Name : $"{parentPath}:{prop.Name}";
                    FlattenJson(prop.Value, path, target);
                }
                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var path = parentPath is null ? index.ToString() : $"{parentPath}:{index}";
                    FlattenJson(item, path, target);
                    index++;
                }
                break;
            case JsonValueKind.String:
                target[parentPath ?? string.Empty] = element.GetString();
                break;
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                target[parentPath ?? string.Empty] = element.ToString();
                break;
        }
    }

    private async Task<string?> GetConfigAsync(string dataId, string group, string? tenant, CancellationToken ct)
    {
        var query = new Dictionary<string, string?>
        {
            ["dataId"] = dataId,
            ["group"] = group,
            ["tenant"] = tenant
        };
        await EnsureAuthAsync(query, ct);
        var url = "/nacos/v1/cs/configs" + BuildQueryString(query);

        using var resp = await _http.GetAsync(url, ct);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(ct);
    }

    private async Task EnsureAuthAsync(IDictionary<string, string?> query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.Username) || string.IsNullOrWhiteSpace(_options.Password))
            return;

        if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            query["accessToken"] = _accessToken;
            return;
        }

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _tokenExpiresAt)
            {
                query["accessToken"] = _accessToken;
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
            using var doc = JsonDocument.Parse(json);
            _accessToken = doc.RootElement.TryGetProperty("accessToken", out var tokenProp) ? tokenProp.GetString() : null;
            var ttl = doc.RootElement.TryGetProperty("tokenTtl", out var ttlProp) ? ttlProp.GetInt32() : 60;
            if (string.IsNullOrWhiteSpace(_accessToken))
                throw new InvalidOperationException("Nacos login succeeded but accessToken is missing.");
            _tokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(10, ttl - 10));
            query["accessToken"] = _accessToken;
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

    private static string ComputeHash(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public void Dispose()
    {
        _cts.Cancel();
        try { _pollingTask?.Wait(TimeSpan.FromSeconds(1)); } catch { /* ignore */ }
        _http.Dispose();
        _cts.Dispose();
        _tokenLock.Dispose();
    }
}
