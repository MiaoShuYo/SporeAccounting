using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SP.Common;
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

    private static readonly char[] CaptchaChars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

    public CaptchaServiceImpl(IRedisService redis, ILogger<CaptchaServiceImpl> logger, IConfiguration configuration)
    {
        _redis = redis;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<CaptchaCreateResponse> CreateAsync(string? ip = null)
    {
        int width = _configuration.GetValue("Captcha:Width", 120);
        int height = _configuration.GetValue("Captcha:Height", 40);
        int length = _configuration.GetValue("Captcha:Length", 4);
        int expiresSeconds = _configuration.GetValue("Captcha:ExpiresSeconds", 120);

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

    private static string BuildKey(string token) => $"captcha:{token}";

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


