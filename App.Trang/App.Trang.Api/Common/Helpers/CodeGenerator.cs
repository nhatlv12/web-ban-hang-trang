using System.Security.Cryptography;

namespace App.Trang.Api.Common.Helpers;

/// <summary>
/// Sinh mã tự động 6 ký tự (chữ + số), đảm bảo không trùng.
/// </summary>
public static class CodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CodeLength = 6;

    /// <summary>
    /// Sinh mã ngẫu nhiên 6 ký tự. Kiểm tra trùng bằng hàm existsCheck.
    /// </summary>
    public static string Generate(Func<string, bool> existsCheck, string prefix = "")
    {
        string code;
        int attempts = 0;
        do
        {
            code = prefix + GenerateRandom();
            attempts++;
            if (attempts > 100)
                throw new InvalidOperationException("Không thể sinh mã duy nhất sau 100 lần thử.");
        }
        while (existsCheck(code));

        return code;
    }

    private static string GenerateRandom()
    {
        return string.Create(CodeLength, 0, (span, _) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
            }
        });
    }
}
