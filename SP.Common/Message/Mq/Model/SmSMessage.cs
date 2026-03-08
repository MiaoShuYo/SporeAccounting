using SP.Common.Message.SmS.Model;

namespace SP.Common.Message.Mq.Model;

/// <summary>
/// 短信消息类
/// </summary>
public class SmSMessage
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; }
    
    /// <summary>
    /// 短信用途
    /// </summary>
    public SmSPurposeEnum Purpose { get; set; }
    
    /// <summary>
    /// 短信内容
    /// </summary>
    public string Message { get; set; }
}