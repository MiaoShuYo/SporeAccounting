using SP.Common.Message.SmS.Model;

namespace SP.Common.Message.SmS.Services;

/// <summary>
/// 短信发送接口
/// </summary>
public interface ISmSService
{
    /// <summary>
    /// 发送短信验证码
    /// </summary>
    /// <param name="toPhoneNumber">接收短信的电话号码</param>
    /// <param name="purpose">验证码用途</param>
    /// <returns>任务</returns>
    Task SendVerificationCodeAsync(string toPhoneNumber, SmSPurposeEnum purpose);
    
    ///<summary>
    /// 发送普通短信
    /// </summary>
    /// <param name="toPhoneNumber">接收短信的电话号码</param>
    /// <param name="message">短信内容</param>
    /// <returns>任务</returns>
    Task SendMessageAsync(string toPhoneNumber, string message);

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    /// <param name="toPhoneNumber">接收短信的电话号码</param>
    /// <param name="purpose">验证码用途</param>
    /// <param name="code">验证码</param>
    /// <returns>是否验证成功</returns>
    Task<bool> VerifyCodeAsync(string toPhoneNumber, SmSPurposeEnum purpose, string code);
}