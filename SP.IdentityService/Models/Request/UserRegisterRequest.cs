using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SP.Common.Attributes;
using SP.IdentityService.Models.Enumeration;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 用户注册请求模型
/// </summary>
[ObjectRules(AnyOf = new[] { "UserName", "Email", "PhoneNumber" },
    RequireIfPresent = new[] { "UserName=>Password", "Email=>Code", "PhoneNumber=>Code" })]
public class UserRegisterRequest
{
    private RegisterTypeEnum _registerType;
    private const string LettersAndDigits = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string UserNameAllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
    private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialCharacters = "!@#$%^&*_-+";
    /// <summary>
    /// 用户名
    /// </summary>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "用户名只能包含字母、数字、下划线和连字符")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Phone(ErrorMessage = "手机号格式不正确")]
    [StringLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// 注册类型
    /// </summary>
    [Required(ErrorMessage = "注册类型不能为空")]
    public RegisterTypeEnum RegisterType
    {
        get => _registerType;
        set
        {
            _registerType = value;
            if (value == RegisterTypeEnum.Email)
            {
                if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    PhoneNumber = "13800000000";
                }
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    UserName = $"user_{GenerateRandomString(10, UserNameAllowedCharacters)}";
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    Password = GeneratePolicyCompliantPassword(12);
                }
            }
            else if (value == RegisterTypeEnum.PhoneNumber)
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    UserName =  $"user_{GenerateRandomString(10, UserNameAllowedCharacters)}";
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    Password = GeneratePolicyCompliantPassword(12);
                }
                if (string.IsNullOrWhiteSpace(Email))
                {
                    var localPart = GenerateRandomString(10, LettersAndDigits).ToLower();
                    Email = $"{localPart}@example.com";
                }
            }
        }
    }

    private static string GenerateRandomString(int length, string allowedCharacters)
    {
        if (length <= 0 || string.IsNullOrEmpty(allowedCharacters))
        {
            return string.Empty;
        }

        var result = new char[length];
        var buffer = new byte[length];
        RandomNumberGenerator.Fill(buffer);

        for (int i = 0; i < length; i++)
        {
            int idx = buffer[i] % allowedCharacters.Length;
            result[i] = allowedCharacters[idx];
        }

        return new string(result);
    }

    private static string GeneratePolicyCompliantPassword(int length)
    {
        int targetLength = Math.Max(length, 6);
        string all = LowercaseLetters + UppercaseLetters + Digits + SpecialCharacters;

        var characters = new List<char>(targetLength)
        {
            GetRandomChar(LowercaseLetters),
            GetRandomChar(UppercaseLetters),
            GetRandomChar(Digits),
            GetRandomChar(SpecialCharacters)
        };

        for (int i = characters.Count; i < targetLength; i++)
        {
            characters.Add(GetRandomChar(all));
        }

        Shuffle(characters);
        return new string(characters.ToArray());
    }

    private static char GetRandomChar(string allowed)
    {
        int idx = RandomNumberGenerator.GetInt32(allowed.Length);
        return allowed[idx];
    }

    private static void Shuffle(List<char> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}