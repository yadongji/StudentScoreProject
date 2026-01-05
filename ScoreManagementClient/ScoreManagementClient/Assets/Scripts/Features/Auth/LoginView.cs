using UnityEngine;
using UnityEngine.UI;
using Core.Base;
using Models;

namespace Features.Auth
{
    /// <summary>
    /// 登录视图 - 负责登录界面的展示和交互
    /// </summary>
    public class LoginView : BaseView
    {
        [Header("UI组件")]
        [SerializeField] private InputField _usernameInput;
        [SerializeField] private InputField _passwordInput;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _testConnectionButton;
        [SerializeField] private Text _statusText;
        [SerializeField] private GameObject _loadingPanel;

        [Header("服务器配置")]
        [SerializeField] private string _serverUrl = "http://localhost:5000";

        private LoginController _controller;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 获取或创建登录控制器
            _controller = FindObjectOfType<LoginController>();
            if (_controller == null)
            {
                GameObject controllerGO = new GameObject("LoginController");
                _controller = controllerGO.AddComponent<LoginController>();
            }

            _controller.Initialize();
            _controller.SetServerUrl(_serverUrl);

            // 绑定按钮事件
            if (_loginButton != null)
            {
                _loginButton.onClick.AddListener(OnLoginButtonClick);
            }

            if (_testConnectionButton != null)
            {
                _testConnectionButton.onClick.AddListener(OnTestConnectionButtonClick);
            }

            // 订阅事件
            EventSystem.Subscribe<LoginState>("LoginStateChanged", OnLoginStateChanged);
            EventSystem.Subscribe<LoginResponse>("LoginSuccess", OnLoginSuccess);
            EventSystem.Subscribe<string>("LoginFailed", OnLoginFailed);
            EventSystem.Subscribe<bool>("ConnectionTestResult", OnConnectionTestResult);

            // 初始化UI
            UpdateStatus("请输入用户名和密码登录");

            DebugHelper.Log("✅ [LoginView] 初始化完成");
        }

        private void OnLoginButtonClick()
        {
            if (_usernameInput == null || _passwordInput == null)
            {
                DebugHelper.LogError("❌ [LoginView] 用户名或密码输入框未设置");
                return;
            }

            string username = _usernameInput.text.Trim();
            string password = _passwordInput.text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                UpdateStatus("请输入用户名");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                UpdateStatus("请输入密码");
                return;
            }

            // 发送登录请求
            var request = new LoginRequest
            {
                phonenumber = username,
                password = password
            };

            string jsonData = JsonUtility.ToJson(request);
            EventSystem.Publish<string>("LoginRequest", jsonData);

            ShowLoading(true);
            UpdateStatus("正在登录...");
        }

        private void OnTestConnectionButtonClick()
        {
            _controller.TestConnection();
            UpdateStatus("正在测试服务器连接...");
        }

        private void OnLoginStateChanged(LoginState state)
        {
            switch (state)
            {
                case LoginState.NotLoggedIn:
                    UpdateStatus("未登录");
                    break;
                case LoginState.LoggingIn:
                    ShowLoading(true);
                    UpdateStatus("正在登录...");
                    break;
                case LoginState.LoggedIn:
                    ShowLoading(false);
                    UpdateStatus("登录成功");
                    break;
                case LoginState.Failed:
                    ShowLoading(false);
                    break;
            }
        }

        private void OnLoginSuccess(LoginResponse response)
        {
            ShowLoading(false);
            UpdateStatus($"登录成功! Token: {response.token?.Substring(0, Mathf.Min(10, response.token?.Length ?? 0))}...");
        }

        private void OnLoginFailed(string message)
        {
            ShowLoading(false);
            UpdateStatus($"登录失败: {message}");
        }

        private void OnConnectionTestResult(bool success)
        {
            if (success)
            {
                UpdateStatus("服务器连接成功");
            }
            else
            {
                UpdateStatus("服务器连接失败");
            }
        }

        private void UpdateStatus(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
        }

        private void ShowLoading(bool show)
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(show);
            }

            if (_loginButton != null)
            {
                _loginButton.interactable = !show;
            }

            if (_testConnectionButton != null)
            {
                _testConnectionButton.interactable = !show;
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            _usernameInput.text = "";
            _passwordInput.text = "";
            UpdateStatus("请输入用户名和密码登录");
        }

        protected override void OnDispose()
        {
            // 取消订阅事件
            EventSystem.Unsubscribe<LoginState>("LoginStateChanged", OnLoginStateChanged);
            EventSystem.Unsubscribe<LoginResponse>("LoginSuccess", OnLoginSuccess);
            EventSystem.Unsubscribe<string>("LoginFailed", OnLoginFailed);
            EventSystem.Unsubscribe<bool>("ConnectionTestResult", OnConnectionTestResult);

            // 解绑按钮事件
            if (_loginButton != null)
            {
                _loginButton.onClick.RemoveListener(OnLoginButtonClick);
            }

            if (_testConnectionButton != null)
            {
                _testConnectionButton.onClick.RemoveListener(OnTestConnectionButtonClick);
            }

            base.OnDispose();
        }
    }
}
