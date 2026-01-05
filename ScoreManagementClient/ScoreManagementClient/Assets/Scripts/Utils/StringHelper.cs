using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        public static byte[] ToByteArray(this string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        public static string FromByteArray(byte[] bytes, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 判断是否为邮箱格式
        /// </summary>
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否为手机号格式（中国大陆）
        /// </summary>
        public static bool IsPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return Regex.IsMatch(phone, @"^1[3-9]\d{9}$");
        }

        /// <summary>
        /// 移除字符串中的特殊字符
        /// </summary>
        public static string RemoveSpecialCharacters(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// 驼峰命名转下划线命名
        /// </summary>
        public static string CamelToSnake(string camelCase)
        {
            if (string.IsNullOrEmpty(camelCase))
                return camelCase;

            return Regex.Replace(camelCase, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        /// <summary>
        /// 下划线命名转驼峰命名
        /// </summary>
        public static string SnakeToCamel(string snakeCase)
        {
            if (string.IsNullOrEmpty(snakeCase))
                return snakeCase;

            var words = snakeCase.Split('_');
            var result = new StringBuilder();
            result.Append(words[0].ToLower());

            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    result.Append(char.ToUpper(words[i][0]));
                    if (words[i].Length > 1)
                    {
                        result.Append(words[i].Substring(1).ToLower());
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string MD5Encrypt(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 字符串截断（带省略号）
        /// </summary>
        public static string TruncateWithEllipsis(string str, int maxLength, string ellipsis = "...")
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            if (maxLength <= ellipsis.Length)
                return ellipsis.Substring(0, maxLength);

            return str.Substring(0, maxLength - ellipsis.Length) + ellipsis;
        }

        /// <summary>
        /// 分割字符串到多个行
        /// </summary>
        public static string[] SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new string[0];

            return text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
    }
}
