namespace SP.Common.Message.Model;

/// <summary>
/// 消息类型
/// </summary>
public static class MessageType
{
    /// <summary>
    /// 验证邮箱
    /// </summary>
    public const string VerifyEmail = "VerifyEmail";
    
    ///<summary>
    /// 找回密码
    ///</summary>
    public const string ResetPassword = "ResetPassword";

    /// <summary>
    /// 预算扣除
    /// </summary>
    public const string BudgetDeduct = "BudgetDeduct";

    /// <summary>
    /// 预算增加
    /// </summary>
    public const string BudgetAdd = "BudgetAdd";
    
    /// <summary>
    /// 预算更新
    /// </summary>
    public const string BudgetUpdate = "BudgetUpdate";
    
    /// <summary>
    /// 用户配置默认币种
    /// </summary>
    public const string UserConfigDefaultCurrency = "UserConfigDefaultCurrency";
    
    /// <summary>
    /// 短信验证码
    /// </summary>
    public const string SmSVerificationCode = "SmSVerificationCode";
    
    /// <summary>
    /// 普通短信
    /// </summary>
    public const string SmSGeneral = "SmSGeneral";
    
    /// <summary>
    /// 预算预警通知
    /// </summary>
    public const string BudgetWarning = "BudgetWarning";
    
    /// <summary>
    /// 预算耗尽通知
    /// </summary>
    public const string BudgetExhausted = "BudgetExhausted";
    
    /// <summary>
    /// 预算超额通知
    /// </summary>
    public const string BudgetOverrun = "BudgetOverrun";

    /// <summary>
    /// 账本分享
    /// </summary>
    public const string AccountBookShare = "AccountBookShare";

    /// <summary>
    /// 账本分享撤回
    /// </summary>
    public const string AccountBookShareRevoke = "AccountBookShareRevoke";

    /// <summary>
    /// 账本分享权限修改
    /// </summary>
    public const string AccountBookSharePermissionUpdate = "AccountBookSharePermissionUpdate";
}