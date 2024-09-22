namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// token
/// </summary>
public class TokenViewModel
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 用于刷新token的RefreshToken
    /// </summary>
    public string RefreshToken { get; set; }
}