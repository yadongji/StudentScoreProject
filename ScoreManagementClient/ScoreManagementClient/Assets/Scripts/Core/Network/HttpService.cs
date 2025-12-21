using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// HTTP 服务基类 - 封装所有网络请求逻辑
/// </summary>
public class HttpService : MonoBehaviour, INetworkService
{
    [SerializeField] private NetworkConfig config;

    protected string BaseUrl => config.BaseUrl;
    protected float RequestTimeout => config.RequestTimeout;

    // GET 请求
    public IEnumerator Get<T>(string endpoint, Action<T> onSuccess, Action<string> onError = null)
    {
        string url = $"{BaseUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = (int)RequestTimeout;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                DebugHelper.LogError($"❌ GET 请求失败: {url} - {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    // POST 请求
    public IEnumerator Post<TRequest, TResponse>(string endpoint, TRequest data, Action<TResponse> onSuccess,
        Action<string> onError = null)
    {
        string url = $"{BaseUrl}{endpoint}";
        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)RequestTimeout;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                TResponse response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                DebugHelper.LogError($"❌ POST 请求失败: {url} - {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    // PUT 请求
    public IEnumerator Put<TRequest, TResponse>(string endpoint, TRequest data, Action<TResponse> onSuccess,
        Action<string> onError = null)
    {
        string url = $"{BaseUrl}{endpoint}";
        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)RequestTimeout;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                TResponse response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                DebugHelper.LogError($"❌ PUT 请求失败: {url} - {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    // DELETE 请求
    public IEnumerator Delete(string endpoint, Action onSuccess, Action<string> onError = null)
    {
        string url = $"{BaseUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            request.timeout = (int)RequestTimeout;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                DebugHelper.LogError($"❌ DELETE 请求失败: {url} - {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }
}