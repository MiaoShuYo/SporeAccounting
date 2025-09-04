namespace SP.Common.Message.SmS.Model;

/// <summary>
/// 短信验证码用途枚举
/// </summary>
public enum SmSPurposeEnum
{
    // 1-注册，2-登录，3-修改密码，4-更换手机号
    /// <summary>
    /// 注册
    /// </summary>
    Register = 1,

    /// <summary>
    /// 登录
    /// </summary>
    Login = 2,

    /// <summary>
    /// 修改密码
    /// </summary>
    ChangePassword = 3,

    /// <summary>
    /// 更换手机号
    /// </summary>
    ChangePhoneNumber = 4
}