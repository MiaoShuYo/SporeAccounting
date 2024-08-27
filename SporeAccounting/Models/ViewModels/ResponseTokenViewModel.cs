namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// token
/// </summary>
public class ResponseTokenViewModel
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 用于刷新token的RefresToken
    /// </summary>
    public string RefresToken { get; set; }
}