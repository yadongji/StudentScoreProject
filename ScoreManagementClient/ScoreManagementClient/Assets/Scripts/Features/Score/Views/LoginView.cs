using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


/// <summary>
/// 登录视图 - 处理登录界面的UI交互
/// </summary>
public class LoginView : MonoBehaviour
{
    [Header("输入框")] [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _passwordInput;

    [Header("按钮")] [SerializeField] private Button _loginButton;
    [SerializeField] private Button _testConnectionButton;

    [Header("消息提示")] [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private GameObject _messagePanel;

    [Header("加载指示器")] [SerializeField] private GameObject _loadingIndicator;
    [SerializeField] private TextMeshProUGUI _loadingText;

    [Header("UI设置")] [SerializeField] private float _messageDisplayDuration = 3f;
    [SerializeField] private Color _successColor = Color.green;
    [SerializeField] private Color _errorColor = Color.red;
    [SerializeField] private Color _infoColor = Color.blue;

    // 事件
    public event Action OnLoginButtonClick;
    public event Action OnTestConnectionClick;

    private void Awake()
    {
        Debug.Log("✅ [LoginView] 初始化");

        // 验证组件引用
        ValidateComponents();

        // 初始化UI状态
        InitializeUI();
    }

    private void Start()
    {
        // 绑定按钮事件
        BindEvents();

        // 设置输入框占位符
        SetupInputFields();

        Debug.Log("✅ [LoginView] 启动完成");
    }

    private void OnDestroy()
    {
        // 解绑事件
        UnbindEvents();
    }

    #region 初始化方法

    /// <summary>
    /// 验证必要组件是否已分配
    /// </summary>
    private void ValidateComponents()
    {
        if (_usernameInput == null)
            Debug.LogError("❌ [LoginView] Username Input 未分配！");

        if (_passwordInput == null)
            Debug.LogError("❌ [LoginView] Password Input 未分配！");

        if (_loginButton == null)
            Debug.LogError("❌ [LoginView] Login Button 未分配！");

        if (_messageText == null)
            Debug.LogWarning("⚠️ [LoginView] Message Text 未分配");

        if (_loadingIndicator == null)
            Debug.LogWarning("⚠️ [LoginView] Loading Indicator 未分配");
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    private void InitializeUI()
    {
        // 隐藏消息面板
        if (_messagePanel != null)
            _messagePanel.SetActive(false);

        // 隐藏加载指示器
        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(false);

        // 启用登录按钮
        if (_loginButton != null)
            _loginButton.interactable = true;
    }

    /// <summary>
    /// 设置输入框
    /// </summary>
    private void SetupInputFields()
    {
        if (_usernameInput != null)
        {
            _usernameInput.contentType = TMP_InputField.ContentType.Standard;
            _usernameInput.placeholder.GetComponent<TextMeshProUGUI>().text = "请输入用户名";

            // 添加输入监听
            _usernameInput.onValueChanged.AddListener(OnUsernameChanged);
        }

        if (_passwordInput != null)
        {
            _passwordInput.contentType = TMP_InputField.ContentType.Password;
            _passwordInput.placeholder.GetComponent<TextMeshProUGUI>().text = "请输入密码";

            // 添加输入监听
            _passwordInput.onValueChanged.AddListener(OnPasswordChanged);

            // 添加回车键监听
            _passwordInput.onSubmit.AddListener(OnPasswordSubmit);
        }
    }

    /// <summary>
    /// 绑定事件
    /// </summary>
    private void BindEvents()
    {
        if (_loginButton != null)
        {
            _loginButton.onClick.AddListener(OnLoginButtonPressed);
            Debug.Log("✅ [LoginView] 登录按钮事件已绑定");
        }

        if (_testConnectionButton != null)
        {
            _testConnectionButton.onClick.AddListener(OnTestConnectionButtonPressed);
            Debug.Log("✅ [LoginView] 测试连接按钮事件已绑定");
        }
    }

    /// <summary>
    /// 解绑事件
    /// </summary>
    private void UnbindEvents()
    {
        if (_loginButton != null)
            _loginButton.onClick.RemoveListener(OnLoginButtonPressed);

        if (_testConnectionButton != null)
            _testConnectionButton.onClick.RemoveListener(OnTestConnectionButtonPressed);

        if (_usernameInput != null)
            _usernameInput.onValueChanged.RemoveListener(OnUsernameChanged);

        if (_passwordInput != null)
        {
            _passwordInput.onValueChanged.RemoveListener(OnPasswordChanged);
            _passwordInput.onSubmit.RemoveListener(OnPasswordSubmit);
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 获取用户名
    /// </summary>
    public string GetUsername()
    {
        string username = _usernameInput?.text?.Trim() ?? "";
        Debug.Log($"📝 [LoginView] 获取用户名: {username}");
        return username;
    }

    /// <summary>
    /// 获取密码
    /// </summary>
    public string GetPassword()
    {
        string password = _passwordInput?.text ?? "";
        Debug.Log($"📝 [LoginView] 获取密码: {new string('*', password.Length)}");
        return password;
    }

    /// <summary>
    /// 设置加载状态
    /// </summary>
    public void SetLoadingState(bool isLoading)
    {
        Debug.Log($"🔄 [LoginView] 设置加载状态: {isLoading}");

        // 显示/隐藏加载指示器
        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(isLoading);

        // 禁用/启用登录按钮
        if (_loginButton != null)
            _loginButton.interactable = !isLoading;

        // 禁用/启用输入框
        if (_usernameInput != null)
            _usernameInput.interactable = !isLoading;

        if (_passwordInput != null)
            _passwordInput.interactable = !isLoading;

        // 更新加载文本
        if (_loadingText != null)
            _loadingText.text = isLoading ? "正在登录..." : "";
    }

    /// <summary>
    /// 显示成功消息
    /// </summary>
    public void ShowSuccessMessage(string message)
    {
        Debug.Log($"✅ [LoginView] 显示成功消息: {message}");
        ShowMessage(message, _successColor);
    }

    /// <summary>
    /// 显示错误消息
    /// </summary>
    public void ShowErrorMessage(string message)
    {
        Debug.LogWarning($"❌ [LoginView] 显示错误消息: {message}");
        ShowMessage(message, _errorColor);
    }

    /// <summary>
    /// 显示信息消息
    /// </summary>
    public void ShowInfoMessage(string message)
    {
        Debug.Log($"ℹ️ [LoginView] 显示信息消息: {message}");
        ShowMessage(message, _infoColor);
    }

    /// <summary>
    /// 清空输入框
    /// </summary>
    public void ClearInputs()
    {
        Debug.Log("🧹 [LoginView] 清空输入框");

        if (_usernameInput != null)
            _usernameInput.text = "";

        if (_passwordInput != null)
            _passwordInput.text = "";
    }

    /// <summary>
    /// 清空密码框
    /// </summary>
    public void ClearPassword()
    {
        Debug.Log("🧹 [LoginView] 清空密码框");

        if (_passwordInput != null)
            _passwordInput.text = "";
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 显示消息
    /// </summary>
    private void ShowMessage(string message, Color color)
    {
        if (_messagePanel == null || _messageText == null)
        {
            Debug.LogWarning("⚠️ [LoginView] 消息面板或文本未分配，无法显示消息");
            return;
        }

        // 取消之前的隐藏任务
        CancelInvoke(nameof(HideMessage));

        // 设置消息内容和颜色
        _messageText.text = message;
        _messageText.color = color;

        // 显示消息面板
        _messagePanel.SetActive(true);

        // 延迟隐藏
        Invoke(nameof(HideMessage), _messageDisplayDuration);
    }

    /// <summary>
    /// 隐藏消息
    /// </summary>
    private void HideMessage()
    {
        if (_messagePanel != null)
            _messagePanel.SetActive(false);
    }

    #endregion

    #region 事件处理

    /// <summary>
    /// 登录按钮点击
    /// </summary>
    private void OnLoginButtonPressed()
    {
        Debug.Log("🖱️ [LoginView] 登录按钮被点击");

        // 触发登录事件
        OnLoginButtonClick?.Invoke();
    }

    /// <summary>
    /// 测试连接按钮点击
    /// </summary>
    private void OnTestConnectionButtonPressed()
    {
        Debug.Log("🖱️ [LoginView] 测试连接按钮被点击");

        // 触发测试连接事件
        OnTestConnectionClick?.Invoke();
    }

    /// <summary>
    /// 用户名输入变化
    /// </summary>
    private void OnUsernameChanged(string value)
    {
        // 可以在这里添加实时验证
        // Debug.Log($"📝 [LoginView] 用户名输入: {value}");
    }

    /// <summary>
    /// 密码输入变化
    /// </summary>
    private void OnPasswordChanged(string value)
    {
        // 可以在这里添加实时验证
        // Debug.Log($"📝 [LoginView] 密码输入: {new string('*', value.Length)}");
    }

    /// <summary>
    /// 密码框回车提交
    /// </summary>
    private void OnPasswordSubmit(string value)
    {
        Debug.Log("⌨️ [LoginView] 密码框回车提交");

        // 触发登录
        OnLoginButtonPressed();
    }

    #endregion

    #region 调试方法

    /// <summary>
    /// 填充测试数据（仅用于调试）
    /// </summary>
    [ContextMenu("填充测试数据")]
    public void FillTestData()
    {
        if (_usernameInput != null)
            _usernameInput.text = "admin";

        if (_passwordInput != null)
            _passwordInput.text = "admin123";

        Debug.Log("🧪 [LoginView] 已填充测试数据");
    }

    /// <summary>
    /// 测试成功消息
    /// </summary>
    [ContextMenu("测试成功消息")]
    public void TestSuccessMessage()
    {
        ShowSuccessMessage("这是一条成功消息！");
    }

    /// <summary>
    /// 测试错误消息
    /// </summary>
    [ContextMenu("测试错误消息")]
    public void TestErrorMessage()
    {
        ShowErrorMessage("这是一条错误消息！");
    }

    /// <summary>
    /// 测试信息消息
    /// </summary>
    [ContextMenu("测试信息消息")]
    public void TestInfoMessage()
    {
        ShowInfoMessage("这是一条信息消息！");
    }

    /// <summary>
    /// 测试加载状态
    /// </summary>
    [ContextMenu("测试加载状态")]
    public void TestLoadingState()
    {
        SetLoadingState(true);
        Invoke(nameof(StopLoading), 3f);
    }

    private void StopLoading()
    {
        SetLoadingState(false);
    }

    #endregion
}