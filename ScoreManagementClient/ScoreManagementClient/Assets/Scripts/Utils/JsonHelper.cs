using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// JSON 工具类 - 处理 Unity JsonUtility 不支持的功能
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 解析 JSON 数组
    /// </summary>
    public static List<T> ParseArray<T>(string json)
    {
        string wrappedJson = $"{{\"items\":{json}}}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.items ?? new List<T>();
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}