using System.ComponentModel.DataAnnotations;

namespace SP.Common.Message.SmS.Model;

/// <summary>
/// 短信发送通用类
/// </summary>
public class SmSRequest
{
    /// <summary>
    /// 接收短信的电话号码
    /// </summary>
    [Required(ErrorMessage = "电话号码不能为空")]
    public List<string> PhoneNumbers { get; set; }

    /// <summary>
    /// 短信用途
    /// </summary>
    [Required(ErrorMessage = "短信用途不能为空")]
    public SmSPurposeEnum Purpose { get; set; }
    
    /// <summary>
    /// 短信内容（用于发送普通短信）
    /// </summary>
    public string Message { get; set; }
}