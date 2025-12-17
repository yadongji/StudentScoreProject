using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 网络服务接口 - 定义所有网络请求的标准
/// </summary>
public interface INetworkService
{
    IEnumerator Get<T>(string endpoint, Action<T> onSuccess, Action<string> onError = null);

    IEnumerator Post<TRequest, TResponse>(string endpoint, TRequest data, Action<TResponse> onSuccess,
        Action<string> onError = null);

    IEnumerator Put<TRequest, TResponse>(string endpoint, TRequest data, Action<TResponse> onSuccess,
        Action<string> onError = null);

    IEnumerator Delete(string endpoint, Action onSuccess, Action<string> onError = null);
}