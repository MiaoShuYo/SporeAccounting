using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SP.Common.SiNan.Configuration;

/// <summary>
/// 从 SiNan 配置中心拉取配置，并支持轮询刷新。
/// 直接使用 HttpClient（不依赖 DI），因为 IConfigurationBuilder 在 DI 容器构建之前运行。
/// </summary>
internal sealed class SpSiNanConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly SiNanOptions _options;
    private readonly HttpClient _http;
    private readonly CancellationTokenSource _cts = new();
    private readonly Dictionary<string, string> _contentHashByKey = new();
    private Task? _pollingTask;

    public SpSiNanConfigurationProvider(SiNanOptions options)
    {
        _options = options;

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException("sinan:BaseUrl is required for SiNan configuration provider.");

        _http = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute),
            Timeout = TimeSpan.FromMilliseconds(options.ConnectionTimeOut <= 0 ? 10_000 : options.ConnectionTimeOut)
        };

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-SiNan-Token", options.ApiKey);
    }

    public override void Load()
    {
        LoadAsync(_cts.Token).GetAwaiter().GetResult();

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
                    // 正常退出
                }
                catch
                {
                    // 配置 Provider 不应让应用崩溃，吞掉异常
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
            if (string.IsNullOrWhiteSpace(listener.Key))
                continue;

            var group = string.IsNullOrWhiteSpace(listener.Group) ? _options.Group : listener.Group;
            var content = await FetchConfigAsync(listener.Key, group, _options.Namespace, ct);

            if (content is null)
            {
                if (!listener.Optional)
                    throw new InvalidOperationException(
                        $"SiNan config not found: key={listener.Key}, group={group}, namespace={_options.Namespace}");
                continue;
            }

            var cacheKey = $"{_options.Namespace}@@{group}@@{listener.Key}";
            var hash = ComputeHash(content);
            if (!_contentHashByKey.TryGetValue(cacheKey, out var oldHash)
                || !string.Equals(oldHash, hash, StringComparison.Ordinal))
            {
                _contentHashByKey[cacheKey] = hash;
                anyChanged = true;
            }

            if (reloadOnlyIfChanged && !anyChanged)
                continue;

            MergeContentInto(content, newData);
        }

        if (reloadOnlyIfChanged && !anyChanged)
            return false;

        Data = newData
            .Where(kv => kv.Value is not null)
            .ToDictionary(kv => kv.Key, kv => kv.Value!, StringComparer.OrdinalIgnoreCase);

        return anyChanged;
    }

    /// <summary>
    /// 调用 GET /api/v1/configs?namespace=&amp;group=&amp;key=
    /// </summary>
    private async Task<string?> FetchConfigAsync(
        string key, string group, string @namespace, CancellationToken ct)
    {
        var url = $"/api/v1/configs" +
                  $"?namespace={Uri.EscapeDataString(@namespace)}" +
                  $"&group={Uri.EscapeDataString(group)}" +
                  $"&key={Uri.EscapeDataString(key)}";

        using var resp = await _http.GetAsync(url, ct);

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();

        // SiNan 返回 JSON 对象 { namespace, group, key, content, ... }，取 content 字段
        var json = await resp.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("content", out var contentProp))
            return contentProp.GetString();

        // 若直接返回纯文本内容则原样使用
        return json;
    }

    /// <summary>
    /// 将配置内容合并到目标字典（支持 JSON 展开 和 key=value 格式）。
    /// </summary>
    private static void MergeContentInto(string content, IDictionary<string, string?> target)
    {
        var trimmed = content.TrimStart();
        if (trimmed.StartsWith('{') || trimmed.StartsWith('['))
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                FlattenJson(doc.RootElement, parentPath: null, target);
                return;
            }
            catch (JsonException)
            {
                // 不是合法 JSON，回退到 key=value 解析
            }
        }

        // key=value 行格式（# 开头为注释）
        using var reader = new StringReader(content);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var idx = line.IndexOf('=');
            if (idx <= 0) continue;
            var k = line[..idx].Trim();
            var v = line[(idx + 1)..].Trim();
            if (k.Length > 0)
                target[k] = v;
        }
    }

    private static void FlattenJson(
        JsonElement element, string? parentPath, IDictionary<string, string?> target)
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

    private static string ComputeHash(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    public void Dispose()
    {
        _cts.Cancel();
        try { _pollingTask?.Wait(TimeSpan.FromSeconds(2)); } catch { /* ignore */ }
        _http.Dispose();
        _cts.Dispose();
    }
}
