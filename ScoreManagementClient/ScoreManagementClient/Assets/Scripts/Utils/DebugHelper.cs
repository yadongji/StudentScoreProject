using UnityEngine;


/// <summary>
/// 调试帮助类 - 提供统一的日志输出接口
/// </summary>
public static class DebugHelper
{
    /// <summary>
    /// 输出普通日志
    /// </summary>
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// 输出错误日志
    /// </summary>
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    /// <summary>
    /// 输出警告日志
    /// </summary>
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    /// <summary>
    /// 输出带格式的日志
    /// </summary>
    public static void LogFormat(string format, params object[] args)
    {
        Debug.LogFormat(format, args);
    }

    /// <summary>
    /// 输出带颜色的日志
    /// </summary>
    public static void LogColor(string message, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        Debug.Log($"<color=#{hexColor}>{message}</color>");
    }
}