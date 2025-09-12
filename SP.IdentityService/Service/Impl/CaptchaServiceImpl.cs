using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SP.Common;
using SP.Common.ConfigService;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Redis;
using SP.IdentityService.Models.Response;
using SP.IdentityService.Services;

namespace SP.IdentityService.Service.Impl;

/// <summary>
/// 图形验证码服务实现
/// </summary>
public class CaptchaServiceImpl : ICaptchaService
{
    private readonly IRedisService _redis;
    private readonly ILogger<CaptchaServiceImpl> _logger;
    private readonly IConfiguration _configuration;

    private static readonly char[] CaptchaChars =
        "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

    /// <summary>
    /// 构建key
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static string BuildKey(string token) => $"captcha:{token}";

    public CaptchaServiceImpl(IRedisService redis, ILogger<CaptchaServiceImpl> logger, IConfiguration configuration)
    {
        _redis = redis;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 生成图形验证码（字母数字）并缓存到 Redis
    /// </summary>
    /// <param name="ip">请求方IP（可选用于限流）</param>
    /// <returns>验证码图片与令牌</returns>
    public async Task<CaptchaCreateResponse> CreateAsync(string? ip = null)
    {
        // 按 IP 限流（基于配置开关）
        bool rateLimitEnabled = _configuration.GetValue(SPConfigKey.CaptchaRateLimitEnabled, true);
        if (rateLimitEnabled && !string.IsNullOrWhiteSpace(ip))
        {
            int windowSeconds = _configuration.GetValue(SPConfigKey.CaptchaRateLimitWindowSeconds, 60);
            int maxRequests = _configuration.GetValue(SPConfigKey.CaptchaRateLimitMaxRequests, 10);
            string rateKey = string.Format(SPRedisKey.CaptchaRateLimit, ip);
            long count = await _redis.IncrementAsync(rateKey, windowSeconds);
            if (count > 0 && count > maxRequests)
            {
                throw new BusinessException(
                    $"请求过于频繁，请在{windowSeconds}秒后再试",
                    System.Net.HttpStatusCode.TooManyRequests);
            }
        }

        int width = _configuration.GetValue(SPConfigKey.CaptchaWidth, 120);
        int height = _configuration.GetValue(SPConfigKey.CaptchaHeight, 40);
        int length = _configuration.GetValue(SPConfigKey.CaptchaLength, 4);
        int expiresSeconds = _configuration.GetValue(SPConfigKey.CaptchaExpirySeconds, 60);

        string code = GenerateCode(length);
        string token = Snow.GetId().ToString();

        using var image = new Image<Rgba32>(width, height, Color.White);
        image.Mutate<Rgba32>(ctx =>
        {
            ctx.Fill(Color.White);
            var random = Random.Shared;

            // 干扰线（使用 Pen + PathBuilder 以提升兼容性）
            for (int i = 0; i < 8; i++)
            {
                var color = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var p1 = new PointF(random.Next(width), random.Next(height));
                var p2 = new PointF(random.Next(width), random.Next(height));
                var pen = Pens.Solid(color, 1);
                var pb = new PathBuilder();
                pb.AddLine(p1, p2);
                ctx.Draw(pen, pb.Build());
            }

            // 文本：使用默认系统字体（第一个可用的字体）
            var fontFamily = SystemFonts.Families.First();
            var font = new Font(fontFamily, height * 0.6f, FontStyle.Bold);
            var textOptions = new RichTextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new PointF(width / 2f, height / 2f)
            };
            ctx.DrawText(textOptions, code, Color.Black);

            // 轻微扭曲/噪点
            for (int i = 0; i < width * height / 30; i++)
            {
                image[Random.Shared.Next(width), Random.Shared.Next(height)] = Color.FromRgb(
                    (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            }
        });

        // 保存到 Redis（小写存储方便大小写不敏感）
        await _redis.SetStringAsync(BuildKey(token), code.ToLowerInvariant(), expiresSeconds);

        // 输出为Base64
        using var buffer = new MemoryStream();
        await image.SaveAsync(buffer, new PngEncoder());
        string base64 = Convert.ToBase64String(buffer.ToArray());
        return new CaptchaCreateResponse
        {
            Token = token,
            ImageBase64 = $"data:image/png;base64,{base64}",
            ExpiresInSeconds = expiresSeconds
        };
    }

    /// <summary>
    /// 校验图形验证码
    /// </summary>
    /// <param name="token">验证码令牌</param>
    /// <param name="code">用户输入验证码</param>
    /// <param name="removeOnSuccess">成功后是否删除</param>
    /// <returns>是否通过</returns>
    public async Task<bool> VerifyAsync(string token, string code, bool removeOnSuccess = true)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(code)) return false;
        string? cached = await _redis.GetStringAsync(BuildKey(token));
        if (string.IsNullOrEmpty(cached)) return false;
        bool ok = string.Equals(cached, code.Trim().ToLowerInvariant(), StringComparison.Ordinal);
        if (ok && removeOnSuccess)
        {
            await _redis.RemoveAsync(BuildKey(token));
        }

        return ok;
    }


    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static string GenerateCode(int length)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(CaptchaChars[Random.Shared.Next(CaptchaChars.Length)]);
        }

        return sb.ToString();
    }
}