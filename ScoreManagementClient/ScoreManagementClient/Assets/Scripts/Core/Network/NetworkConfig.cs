using UnityEngine;


/// <summary>
/// 网络配置 - 可在 Inspector 中配置
/// </summary>
[CreateAssetMenu(fileName = "NetworkConfig", menuName = "Config/Network Config")]
public class NetworkConfig : ScriptableObject
{
    [Header("服务器配置")] [Tooltip("服务器基础URL")]
    public string BaseUrl = "http://localhost:5000/api";

    [Header("请求配置")] [Tooltip("请求超时时间（秒）")]
    public float RequestTimeout = 10f;

    [Tooltip("重试次数")] public int RetryCount = 3;

    [Header("调试")] [Tooltip("是否打印网络日志")] public bool EnableDebugLog = true;
}