using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.SmS.Model;
using SP.Common.Redis;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SP.Common.Message.SmS.Services.Impl;

/// <summary>
/// 短信服务实现类（使用Twilio）
/// </summary>
public class TwilioSmSServiceImpl : ISmSService
{
    private readonly ILogger<TwilioSmSServiceImpl> _logger;
    private readonly IRedisService _redis;
    private readonly TwilioSmsOptions _options;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="redis"></param>
    /// <param name="options"></param>
    public TwilioSmSServiceImpl(ILogger<TwilioSmSServiceImpl> logger, IRedisService redis, TwilioSmsOptions options)
    {
        _logger = logger;
        _redis = redis;
        _options = options;
        if (string.IsNullOrWhiteSpace(_options.AccountSid) || string.IsNullOrWhiteSpace(_options.AuthToken)
                                                           || (string.IsNullOrWhiteSpace(_options.FromNumber) &&
                                                               string.IsNullOrWhiteSpace(_options.MessagingServiceSid)))
        {
            _logger.LogError(
                "Twilio 短信服务未配置完整，短信功能将不可用。请检查配置项：AccountSid, AuthToken, FromNumber 或 MessagingServiceSid");
            throw new BusinessException("Twilio短信服务未配置完整");
        }

        // 初始化Twilio客户端
        TwilioClient.Init(_options.AccountSid, _options.AuthToken);
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    /// <param name="toPhoneNumber">接收短信的电话号码</param>
    /// <param name="purpose">验证码用途</param>
    /// <returns>任务</returns>
    public async Task SendVerificationCodeAsync(string toPhoneNumber, SmSPurposeEnum purpose)
    {
        if (string.IsNullOrEmpty(toPhoneNumber))
        {
            _logger.LogError("发送短信失败，电话号码不能为空");
            throw new BusinessException("电话号码不能为空");
        }

        // 限流
        string limitKey = string.Format(SPRedisKey.SmsLimit, toPhoneNumber);
        await IsRateLimitedAsync(limitKey, toPhoneNumber);

        // 校验并记录当天发送次数（每日上限）
        string limitDayKey = string.Format(SPRedisKey.SmSLimitDay, toPhoneNumber);
        await IsCheckDailyLimitAsync(limitDayKey, toPhoneNumber);

        // 生成验证码
        string code = BuildCode();
        // 存储验证码到redis
        string codeKey = string.Format(SPRedisKey.SmSCode, toPhoneNumber, purpose);
        int ttl = _options.CodeTTLSeconds > 0 ? _options.CodeTTLSeconds : 300;
        await _redis.SetStringAsync(codeKey, code, ttl);
        // 设置发送间隔，防止频繁发送
        int interval = _options.SendIntervalSeconds > 0 ? _options.SendIntervalSeconds : 60;
        await _redis.SetStringAsync(limitKey, "1", interval);

        // 组装短信
        string messageBody =
            $"【{_options.Signature}】您的验证码是 {code}.有效期为{ttl / 60}分钟。如非本人操作，请忽略本短信。";
        // 发送短信
        await SendSmsAsync(toPhoneNumber, messageBody);
        _logger.LogInformation("发送短信验证码成功，电话号码：{PhoneNumber}, 用途：{Purpose}，验证码：{code}", toPhoneNumber, purpose, code);
    }

    ///<summary>
    /// 发送普通短信
    /// </summary>
    /// <param name="toPhoneNumber">接收短信的电话号码</param>
    /// <param name="message">短信内容</param>
    /// <returns>任务</returns>
    public async Task SendMessageAsync(string toPhoneNumber, string message)
    {
        if (string.IsNullOrEmpty(toPhoneNumber))
        {
            _logger.LogError("发送短信失败，电话号码不能为空");
            throw new BusinessException("电话号码不能为空");
        }

        if (string.IsNullOrEmpty(message))
        {
            _logger.LogError("发送短信失败，短信内容不能为空");
            throw new BusinessException("短信内容不能为空");
        }

        // 限流
        string limitKey = string.Format(SPRedisKey.SmsLimit, toPhoneNumber);
        await IsRateLimitedAsync(limitKey, toPhoneNumber);

        // 校验并记录当天发送次数（每日上限）
        string limitDayKey = string.Format(SPRedisKey.SmSLimitDay, toPhoneNumber);
        await IsCheckDailyLimitAsync(limitDayKey, toPhoneNumber);
        // 组装短信
        string messageBody =
            $"【{_options.Signature}】{message}";
        // 发送短信
        await SendSmsAsync(toPhoneNumber, messageBody);
        _logger.LogInformation("发送短信成功，电话号码：{PhoneNumber}，内容：{messageBody}", toPhoneNumber, messageBody);
    }

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    /// <param name="toPhoneNumber"></param>
    /// <param name="purpose"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<bool> VerifyCodeAsync(string toPhoneNumber, SmSPurposeEnum purpose, string code)
    {
        if (string.IsNullOrEmpty(toPhoneNumber) || string.IsNullOrEmpty(code))
        {
            throw new BusinessException("电话号码或验证码不能为空");
        }

        string codeKey = string.Format(SPRedisKey.SmSCode, toPhoneNumber, purpose);
        var storedCode = await _redis.GetStringAsync(codeKey);
        if (storedCode == code)
        {
            // 验证成功，删除验证码
            await _redis.RemoveAsync(codeKey);
            _logger.LogInformation("验证短信验证码成功，电话号码：{PhoneNumber}, 用途：{Purpose}", toPhoneNumber, purpose);
            return true;
        }
        else
        {
            _logger.LogWarning("验证短信验证码失败，电话号码：{PhoneNumber}, 用途：{Purpose}", toPhoneNumber, purpose);
            return false;
        }
    }

    /// <summary>
    /// 发送短信
    /// </summary>
    private async Task SendSmsAsync(string toPhoneNumber, string messageBody)
    {
        PhoneNumber to = new PhoneNumber(toPhoneNumber);
        CreateMessageOptions messageOptions = new CreateMessageOptions(to)
        {
            Body = messageBody
        };
        if (!string.IsNullOrWhiteSpace(_options.MessagingServiceSid))
        {
            messageOptions.MessagingServiceSid = _options.MessagingServiceSid;
        }
        else
        {
            messageOptions.From = new PhoneNumber(_options.FromNumber);
        }

        await MessageResource.CreateAsync(messageOptions);
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <returns></returns>
    private string BuildCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[6];
        rng.GetBytes(bytes);
        var code = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
        return code.ToString();
    }

    /// <summary>
    /// 限流
    /// </summary>
    /// <param name="limitKey">限流Key</param>
    /// <param name="toPhoneNumber">手机号</param>
    /// <returns></returns>
    private async Task IsRateLimitedAsync(string limitKey, string toPhoneNumber)
    {
        if (await _redis.ExistsAsync(limitKey))
        {
            _logger.LogWarning("发送短信失败，发送过于频繁，电话号码：{PhoneNumber}", toPhoneNumber);
            throw new BusinessException("发送过于频繁，请稍后再试");
        }
    }

    /// <summary>
    /// 每天发送上限
    /// </summary>
    /// <param name="limitDayKey">每日限流Key</param>
    /// <param name="toPhoneNumber">手机号</param>
    /// <returns></returns>
    private async Task IsCheckDailyLimitAsync(string limitDayKey, string toPhoneNumber)
    {
        int sendNumLimitPerDay = _options.SendNumLimitPerDay > 0 ? _options.SendNumLimitPerDay : 5;
        int daySeconds = (int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalSeconds;

        if (!await _redis.ExistsAsync(limitDayKey))
        {
            // 第一次发送：写入计数并设置当天过期时间
            await _redis.HashSetAsync(limitDayKey, "count", "1");
            await _redis.SetExpiryAsync(limitDayKey, daySeconds);
        }
        else
        {
            var countStr = await _redis.HashGetAsync(limitDayKey, "count");
            int currentCount = 0;
            _ = int.TryParse(countStr, out currentCount);
            if (currentCount >= sendNumLimitPerDay)
            {
                _logger.LogWarning("发送短信失败，超过每日发送上限，电话号码：{PhoneNumber}", toPhoneNumber);
                throw new BusinessException("今日发送次数已达上限");
            }

            await _redis.HashSetAsync(limitDayKey, "count", (currentCount + 1).ToString());
        }
    }
}