namespace SP.Common;

/// <summary>
/// 生成验证码类
/// </summary>
public static class CodeGenerator
{
    /// <summary>
    /// 生成{length}位数字验证码
    /// </summary>
    /// <returns></returns>
    public static string GenerateVerificationCode(int length = 6)
    {
        if (length < 1 || length > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "数字验证码长度必须在1位到10位之间");
        }

        // 使用随机数生成器生成指定长度的验证码
        Random random = new Random();
        string code = string.Empty;
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }

        return code;
    }

    /// <summary>
    /// 生成{length}位字母验证码
    /// </summary>
    /// <returns></returns>
    public static string GenerateLetterCode(int length = 6)
    {
        if (length < 1 || length > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "字母验证码长度必须在1位到10位之间");
        }

        // 使用随机数生成器生成指定长度的验证码（包括大写和小写字母）
        Random random = new Random();
        string code = string.Empty;
        for (int i = 0; i < length; i++)
        {
            char letter = random.Next(0, 2) == 0
                ? (char)random.Next('A', 'Z' + 1)
                : (char)random.Next('a', 'z' + 1);
            code += letter.ToString();
        }

        return code;
    }

    /// <summary>
    /// 生成{length}位字母数字验证码
    /// </summary>
    /// <returns></returns>
    public static string GenerateAlphanumericCode(int length = 6)
    {
        if (length < 1 || length > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "字母数字验证码长度必须在1位到10位之间");
        }

        // 使用随机数生成器生成指定长度的验证码
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string code = string.Empty;
        for (int i = 0; i < length; i++)
        {
            code += chars[random.Next(chars.Length)];
        }

        return code;
    }
}