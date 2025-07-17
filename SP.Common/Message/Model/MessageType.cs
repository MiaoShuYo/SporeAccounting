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
}