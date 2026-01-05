using UnityEngine;
using Core.Base;
using Models;
using Services;
using Utils;

namespace Features.Auth
{
    /// <summary>
    /// 登录控制器 - 负责登录逻辑控制
    /// </summary>
    public class LoginController : BaseController
    {
        private LoginService _loginService;
        private LoginState _loginState = LoginState.NotLoggedIn;

        public LoginState LoginState => _loginState;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _loginService = new LoginService();
            _loginService.Initialize();

            _loginService.OnLoginSuccess += OnLoginSuccess;
            _loginService.OnLoginFailed += OnLoginFailed;

            DebugHelper.Log("✅ [LoginController] 初始化完成");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventSystem.Subscribe<string>("LoginRequest", HandleLoginRequest);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventSystem.Unsubscribe<string>("LoginRequest", HandleLoginRequest);
        }

        /// <summary>
        /// 处理登录请求
        /// </summary>
        private void HandleLoginRequest(string jsonData)
        {
            var request = JsonUtility.FromJson<LoginRequest>(jsonData);
            if (request != null)
            {
                Login(request.phonenumber, request.password);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login(string username, string password)
        {
            if (_loginState == LoginState.LoggingIn)
            {
                DebugHelper.LogWarning("⚠️ [LoginController] 正在登录中，请勿重复操作");
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DebugHelper.LogError("❌ [LoginController] 用户名或密码不能为空");
                EventSystem.Publish("LoginFailed", "用户名或密码不能为空");
                return;
            }

            _loginState = LoginState.LoggingIn;
            EventSystem.Publish<LoginState>("LoginStateChanged", _loginState);

            _loginService.Login(username, password, (success, message, response) =>
            {
                if (success)
                {
                    _loginState = LoginState.LoggedIn;
                    EventSystem.Publish<LoginState>("LoginStateChanged", _loginState);
                    EventSystem.Publish<LoginResponse>("LoginSuccess", response);
                }
                else
                {
                    _loginState = LoginState.Failed;
                    EventSystem.Publish<LoginState>("LoginStateChanged", _loginState);
                }
            });
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout()
        {
            _loginService.ClearAuthToken();
            _loginState = LoginState.NotLoggedIn;
            EventSystem.Publish<LoginState>("LoginStateChanged", _loginState);
            EventSystem.Publish("Logout", true);
            DebugHelper.Log("📤 [LoginController] 用户已登出");
        }

        /// <summary>
        /// 测试服务器连接
        /// </summary>
        public void TestConnection()
        {
            _loginService.TestConnection((success, message) =>
            {
                EventSystem.Publish<bool>("ConnectionTestResult", success);
            });
        }

        /// <summary>
        /// 设置服务器地址
        /// </summary>
        public void SetServerUrl(string url)
        {
            _loginService.SetBaseUrl(url);
        }

        /// <summary>
        /// 检查是否已登录
        /// </summary>
        public bool IsLoggedIn()
        {
            return _loginService.IsAuthenticated;
        }

        private void OnLoginSuccess(string message)
        {
            DebugHelper.Log($"✅ [LoginController] 登录成功: {message}");
        }

        private void OnLoginFailed(string message)
        {
            DebugHelper.LogError($"❌ [LoginController] 登录失败: {message}");
        }

        protected override void OnDispose()
        {
            if (_loginService != null)
            {
                _loginService.OnLoginSuccess -= OnLoginSuccess;
                _loginService.OnLoginFailed -= OnLoginFailed;
                _loginService.Dispose();
                _loginService = null;
            }

            base.OnDispose();
            DebugHelper.Log("🗑️ [LoginController] 已释放");
        }
    }
}
