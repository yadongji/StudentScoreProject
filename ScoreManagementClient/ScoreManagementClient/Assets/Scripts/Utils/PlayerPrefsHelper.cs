using UnityEngine;

namespace Utils
{
    /// <summary>
    /// PlayerPrefs 帮助类
    /// </summary>
    public static class PlayerPrefsHelper
    {
        #region 设置值

        /// <summary>
        /// 设置整数值
        /// </summary>
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置浮点数值
        /// </summary>
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置字符串值
        /// </summary>
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置布尔值
        /// </summary>
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置长整型值
        /// </summary>
        public static void SetLong(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            PlayerPrefs.Save();
        }

        #endregion

        #region 获取值

        /// <summary>
        /// 获取整数值
        /// </summary>
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        /// <summary>
        /// 获取浮点数值
        /// </summary>
        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        /// <summary>
        /// 获取长整型值
        /// </summary>
        public static long GetLong(string key, long defaultValue = 0L)
        {
            string value = PlayerPrefs.GetString(key, defaultValue.ToString());
            if (long.TryParse(value, out long result))
                return result;
            return defaultValue;
        }

        #endregion

        #region 检查和删除

        /// <summary>
        /// 检查键是否存在
        /// </summary>
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// 删除指定键
        /// </summary>
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        #endregion

        #region 带前缀的键操作

        /// <summary>
        /// 获取带前缀的键
        /// </summary>
        public static string GetKeyWithPrefix(string prefix, string key)
        {
            return $"{prefix}_{key}";
        }

        /// <summary>
        /// 设置带前缀的值
        /// </summary>
        public static void SetStringWithPrefix(string prefix, string key, string value)
        {
            SetString(GetKeyWithPrefix(prefix, key), value);
        }

        /// <summary>
        /// 获取带前缀的值
        /// </summary>
        public static string GetStringWithPrefix(string prefix, string key, string defaultValue = "")
        {
            return GetString(GetKeyWithPrefix(prefix, key), defaultValue);
        }

        #endregion
    }
}
