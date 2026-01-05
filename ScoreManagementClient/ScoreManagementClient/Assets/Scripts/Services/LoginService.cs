using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Models;

namespace Services
{
    /// <summary>
    /// 登录服务 - 处理与服务器之间的登录相关交互
    /// </summary>
    public class LoginService : MonoBehaviour
    {
        private bool _isInitialized = false;
        private string _baseUrl = "http://localhost:5000";
        private string _authToken;
        private float _requestTimeout = 10f;

        public string AuthToken => _authToken;
        public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

        public event Action<string> OnLoginSuccess;
        public event Action<string> OnLoginFailed;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            DebugHelper.Log("✅ [LoginService] 初始化完成");
        }

        public void Dispose()
        {
            if (!_isInitialized) return;
            ClearAuthToken();
            _isInitialized = false;
        }

        /// <summary>
        /// 设置服务器地址
        /// </summary>
        public void SetBaseUrl(string url)
        {
            _baseUrl = url.TrimEnd('/');
            DebugHelper.Log($"🌐 [LoginService] 设置服务器地址: {_baseUrl}");
        }

        /// <summary>
        /// 设置认证令牌
        /// </summary>
        public void SetAuthToken(string token)
        {
            _authToken = token;
            DebugHelper.Log($"🔑 [LoginService] 设置认证令牌");
        }

        /// <summary>
        /// 清除认证令牌
        /// </summary>
        public void ClearAuthToken()
        {
            _authToken = null;
            DebugHelper.Log("🔑 [LoginService] 清除认证令牌");
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        public void Login(string username, string password, Action<bool, string, LoginResponse> callback)
        {
            DebugHelper.Log($"🔐 [LoginService] 发起登录请求");
            StartCoroutine(LoginCoroutine(username, password, callback));
        }

        private System.Collections.IEnumerator LoginCoroutine(string username, string password, Action<bool, string, LoginResponse> callback)
        {
            string url = $"{_baseUrl}/api/auth/login";

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

                DebugHelper.Log($"📤 [LoginService] 发送登录请求: POST {url}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    DebugHelper.Log($"✅ [LoginService] 登录成功: {responseText}");

                    try
                    {
                        var response = JsonUtility.FromJson<LoginResponse>(responseText);

                        if (response != null && !string.IsNullOrEmpty(response.token))
                        {
                            SetAuthToken(response.token);
                            callback?.Invoke(true, "登录成功", response);
                            OnLoginSuccess?.Invoke("登录成功");
                        }
                        else
                        {
                            string error = "登录失败: 无效的响应格式";
                            DebugHelper.LogError($"❌ [LoginService] {error}");
                            callback?.Invoke(false, error, null);
                            OnLoginFailed?.Invoke(error);
                        }
                    }
                    catch (Exception e)
                    {
                        string error = $"解析响应失败: {e.Message}";
                        DebugHelper.LogError($"❌ [LoginService] {error}");
                        callback?.Invoke(false, error, null);
                        OnLoginFailed?.Invoke(error);
                    }
                }
                else
                {
                    string error = GetErrorMessage(request);
                    DebugHelper.LogError($"❌ [LoginService] 登录失败: {error}");
                    callback?.Invoke(false, error, null);
                    OnLoginFailed?.Invoke(error);
                }
            }
        }

        /// <summary>
        /// 测试服务器连接
        /// </summary>
        public void TestConnection(Action<bool, string> callback)
        {
            DebugHelper.Log($"🔌 [LoginService] 测试连接");
            StartCoroutine(TestConnectionCoroutine(callback));
        }

        private System.Collections.IEnumerator TestConnectionCoroutine(Action<bool, string> callback)
        {
            string url = $"{_baseUrl}/api/health";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = (int)_requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    DebugHelper.Log($"✅ [LoginService] 连接成功");
                    callback?.Invoke(true, "连接成功");
                }
                else
                {
                    string error = $"连接失败: {request.error}";
                    DebugHelper.LogError($"❌ [LoginService] {error}");
                    callback?.Invoke(false, error);
                }
            }
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        private string GetErrorMessage(UnityWebRequest request)
        {
            string error = request.error;

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

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
