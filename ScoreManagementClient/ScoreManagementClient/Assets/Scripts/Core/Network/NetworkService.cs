using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// 网络服务 - 单例模式处理所有HTTP通信
/// </summary>
public class NetworkService : MonoBehaviour
{
    #region 单例模式

    private static NetworkService _instance;

    public static NetworkService Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中查找
                _instance = FindObjectOfType<NetworkService>();

                // 如果场景中没有，则创建一个
                if (_instance == null)
                {
                    GameObject go = new GameObject("NetworkService");
                    _instance = go.AddComponent<NetworkService>();
                    DontDestroyOnLoad(go);
                    DebugHelper.Log("✅ [NetworkService] 自动创建单例实例");
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // 确保只有一个实例
        if (_instance != null && _instance != this)
        {
            DebugHelper.LogWarning("⚠️ [NetworkService] 检测到重复实例，销毁当前实例");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        DebugHelper.Log("✅ [NetworkService] 初始化完成");
    }

    #endregion

    #region 配置

    [Header("服务器配置")] [SerializeField] private string _baseUrl = "http://localhost:5000";
    [SerializeField] private float _requestTimeout = 10f;

    [Header("认证")] private string _authToken;

    // 事件
    public event Action<string> OnError;
    public event Action<string> OnSuccess;

    #endregion

    #region 初始化和配置

    /// <summary>
    /// 设置服务器地址
    /// </summary>
    public void SetBaseUrl(string url)
    {
        _baseUrl = url.TrimEnd('/');
        DebugHelper.Log($"🌐 [NetworkService] 设置服务器地址: {_baseUrl}");
    }

    /// <summary>
    /// 获取服务器地址
    /// </summary>
    public string GetBaseUrl()
    {
        return _baseUrl;
    }

    /// <summary>
    /// 设置认证令牌
    /// </summary>
    public void SetAuthToken(string token)
    {
        _authToken = token;
        DebugHelper.Log($"🔑 [NetworkService] 设置认证令牌: {token?.Substring(0, Math.Min(10, token?.Length ?? 0))}...");
    }

    /// <summary>
    /// 清除认证令牌
    /// </summary>
    public void ClearAuthToken()
    {
        _authToken = null;
        DebugHelper.Log("🔑 [NetworkService] 清除认证令牌");
    }

    /// <summary>
    /// 检查是否已认证
    /// </summary>
    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(_authToken);
    }

    #endregion

    #region 测试连接

    /// <summary>
    /// 测试服务器连接
    /// </summary>
    public void TestConnection(Action<bool, string> callback)
    {
        DebugHelper.Log($"🔌 [NetworkService] 测试连接: {_baseUrl}");
        StartCoroutine(TestConnectionCoroutine(callback));
    }

    private IEnumerator TestConnectionCoroutine(Action<bool, string> callback)
    {
        string url = $"{_baseUrl}/api/health";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = (int)_requestTimeout;

            DebugHelper.Log($"📤 [NetworkService] 发送请求: GET {url}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DebugHelper.Log($"✅ [NetworkService] 连接成功: {request.downloadHandler.text}");
                callback?.Invoke(true, "连接成功");
                OnSuccess?.Invoke("连接成功");
            }
            else
            {
                string error = $"连接失败: {request.error}";
                DebugHelper.LogError($"❌ [NetworkService] {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region 登录

    /// <summary>
    /// 用户登录
    /// </summary>
    public void Login(string username, string password, Action<bool, string> callback)
    {
        DebugHelper.Log($"🔐 [NetworkService] 登录请求: 手机号={username}");
        StartCoroutine(LoginCoroutine(username, password, callback));
    }

    private IEnumerator LoginCoroutine(string username, string password, Action<bool, string> callback)
    {
        string url = $"{_baseUrl}/api/auth/login";

        // 构建登录数据
        var loginData = new LoginRequest
        {
            phonenumber = username,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = (int)_requestTimeout;

            DebugHelper.Log($"📤 [NetworkService] 发送登录请求: POST {url}");
            DebugHelper.Log($"📦 [NetworkService] 请求数据: {jsonData}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ [NetworkService] 登录成功: {responseText}");

                try
                {
                    // 解析响应
                    var response = JsonUtility.FromJson<LoginResponse>(responseText);

                    if (response != null && !string.IsNullOrEmpty(response.token))
                    {
                        // 保存令牌
                        SetAuthToken(response.token);

                        callback?.Invoke(true, "登录成功");
                        OnSuccess?.Invoke("登录成功");
                    }
                    else
                    {
                        string error = "登录失败: 无效的响应格式";
                        Debug.LogError($"❌ [NetworkService] {error}");
                        callback?.Invoke(false, error);
                        OnError?.Invoke(error);
                    }
                }
                catch (Exception e)
                {
                    string error = $"解析响应失败: {e.Message}";
                    Debug.LogError($"❌ [NetworkService] {error}");
                    callback?.Invoke(false, error);
                    OnError?.Invoke(error);
                }
            }
            else
            {
                string error = GetErrorMessage(request);
                Debug.LogError($"❌ [NetworkService] 登录失败: {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region GET 请求

    /// <summary>
    /// 发送GET请求
    /// </summary>
    public void Get(string endpoint, Action<bool, string> callback)
    {
        Debug.Log($"📥 [NetworkService] GET请求: {endpoint}");
        StartCoroutine(GetCoroutine(endpoint, callback));
    }

    private IEnumerator GetCoroutine(string endpoint, Action<bool, string> callback)
    {
        string url = $"{_baseUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 添加认证头
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.timeout = (int)_requestTimeout;

            Debug.Log($"📤 [NetworkService] 发送请求: GET {url}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ [NetworkService] GET成功: {responseText}");
                callback?.Invoke(true, responseText);
            }
            else
            {
                string error = GetErrorMessage(request);
                Debug.LogError($"❌ [NetworkService] GET失败: {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region POST 请求

    /// <summary>
    /// 发送POST请求
    /// </summary>
    public void Post(string endpoint, string jsonData, Action<bool, string> callback)
    {
        Debug.Log($"📤 [NetworkService] POST请求: {endpoint}");
        StartCoroutine(PostCoroutine(endpoint, jsonData, callback));
    }

    private IEnumerator PostCoroutine(string endpoint, string jsonData, Action<bool, string> callback)
    {
        string url = $"{_baseUrl}{endpoint}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 添加认证头
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.timeout = (int)_requestTimeout;

            Debug.Log($"📤 [NetworkService] 发送请求: POST {url}");
            Debug.Log($"📦 [NetworkService] 请求数据: {jsonData}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ [NetworkService] POST成功: {responseText}");
                callback?.Invoke(true, responseText);
            }
            else
            {
                string error = GetErrorMessage(request);
                Debug.LogError($"❌ [NetworkService] POST失败: {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region PUT 请求

    /// <summary>
    /// 发送PUT请求
    /// </summary>
    public void Put(string endpoint, string jsonData, Action<bool, string> callback)
    {
        Debug.Log($"📝 [NetworkService] PUT请求: {endpoint}");
        StartCoroutine(PutCoroutine(endpoint, jsonData, callback));
    }

    private IEnumerator PutCoroutine(string endpoint, string jsonData, Action<bool, string> callback)
    {
        string url = $"{_baseUrl}{endpoint}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 添加认证头
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.timeout = (int)_requestTimeout;

            Debug.Log($"📤 [NetworkService] 发送请求: PUT {url}");
            Debug.Log($"📦 [NetworkService] 请求数据: {jsonData}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ [NetworkService] PUT成功: {responseText}");
                callback?.Invoke(true, responseText);
            }
            else
            {
                string error = GetErrorMessage(request);
                Debug.LogError($"❌ [NetworkService] PUT失败: {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region DELETE 请求

    /// <summary>
    /// 发送DELETE请求
    /// </summary>
    public void Delete(string endpoint, Action<bool, string> callback)
    {
        Debug.Log($"🗑️ [NetworkService] DELETE请求: {endpoint}");
        StartCoroutine(DeleteCoroutine(endpoint, callback));
    }

    private IEnumerator DeleteCoroutine(string endpoint, Action<bool, string> callback)
    {
        string url = $"{_baseUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            // 添加认证头
            if (!string.IsNullOrEmpty(_authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
            }

            request.timeout = (int)_requestTimeout;

            Debug.Log($"📤 [NetworkService] 发送请求: DELETE {url}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ [NetworkService] DELETE成功: {responseText}");
                callback?.Invoke(true, responseText);
            }
            else
            {
                string error = GetErrorMessage(request);
                Debug.LogError($"❌ [NetworkService] DELETE失败: {error}");
                callback?.Invoke(false, error);
                OnError?.Invoke(error);
            }
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取错误消息
    /// </summary>
    private string GetErrorMessage(UnityWebRequest request)
    {
        string error = request.error;

        // 尝试解析服务器返回的错误消息
        if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
        {
            try
            {
                var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
                if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.message))
                {
                    error = errorResponse.message;
                }
            }
            catch
            {
                // 如果解析失败,使用原始错误消息
            }
        }

        return error;
    }

    #endregion

    #region 数据模型

    [Serializable]
    private class LoginRequest
    {
        public string phonenumber;
        public string password;
    }

    [Serializable]
    private class LoginResponse
    {
        public string token;
        public string message;
    }

    [Serializable]
    private class ErrorResponse
    {
        public string message;
        public string error;
    }

    #endregion
}