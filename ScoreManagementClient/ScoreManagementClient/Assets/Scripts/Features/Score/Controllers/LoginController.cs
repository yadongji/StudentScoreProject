using UnityEngine;


/// <summary>
/// 登录控制器 - 处理用户登录逻辑
/// </summary>
public class LoginController : MonoBehaviour
{
    [Header("视图引用")] [SerializeField] private LoginView _view;

    [Header("服务引用")] private NetworkService _networkService;

    private void Awake()
    {
        // 初始化网络服务
        _networkService = NetworkService.Instance;

        if (_networkService == null)
        {
            DebugHelper.LogError("❌ [LoginController] NetworkService 未找到！");
            return;
        }

        DebugHelper.Log("✅ [LoginController] 初始化完成");
    }

    private void Start()
    {
        // 绑定视图事件
        if (_view != null)
        {
            _view.OnLoginButtonClick += OnLoginButtonClicked;
            _view.OnTestConnectionClick += TestConnection;
            DebugHelper.Log("✅ [LoginController] 视图事件绑定成功");
        }
        else
        {
            DebugHelper.LogError("❌ [LoginController] LoginView 未分配！");
        }

        // 检查是否有保存的 Token
        CheckSavedToken();
    }

    private void OnDestroy()
    {
        // 解绑事件，防止内存泄漏
        if (_view != null)
        {
            _view.OnLoginButtonClick -= OnLoginButtonClicked;
        }
    }

    /// <summary>
    /// 检查是否有保存的 Token
    /// </summary>
    private void CheckSavedToken()
    {
        string savedToken = PlayerPrefs.GetString("AuthToken", "");

        if (!string.IsNullOrEmpty(savedToken))
        {
            DebugHelper.Log(
                $"🔑 [LoginController] 发现已保存的 Token: {savedToken.Substring(0, Mathf.Min(20, savedToken.Length))}...");
            // 可以选择自动跳转到主界面，或提示用户
            _view?.ShowInfoMessage("检测到已登录状态");
        }
        else
        {
            DebugHelper.Log("ℹ️ [LoginController] 未发现保存的 Token，需要登录");
        }
    }

    /// <summary>
    /// 登录按钮点击事件处理
    /// </summary>
    public void OnLoginButtonClicked()
    {
        DebugHelper.Log("==================== 🔐 开始登录流程 ====================");

        // 获取用户输入
        string username = _view.GetUsername();
        string password = _view.GetPassword();

        // 输入验证
        if (!ValidateInput(username, password))
        {
            return;
        }

        // 显示加载状态
        _view.SetLoadingState(true);
        _view.ShowInfoMessage("正在登录...");

        DebugHelper.Log($"📝 [LoginController] 用户名: {username}");
        DebugHelper.Log($"📝 [LoginController] 密码长度: {password.Length} 字符");
        DebugHelper.Log($"🌐 [LoginController] 请求地址: {_networkService.GetBaseUrl()}/auth/login");
        DebugHelper.Log($"⏰ [LoginController] 请求时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        try
        {
            // 发送登录请求
            NetworkService.Instance.Login(username, password, (success, message) =>
            {
                if (success)
                {
                    DebugHelper.Log("✅ 登录成功！");
                }
                else
                {
                    DebugHelper.LogError($"❌ 登录失败: {message}");
                }
            });
        }
        catch (System.Exception ex)
        {
            DebugHelper.LogError($"❌ [LoginController] 登录异常: {ex.Message}");
            DebugHelper.LogError($"   StackTrace: {ex.StackTrace}");

            _view.ShowErrorMessage($"登录失败: {ex.Message}");
        }
        finally
        {
            // 恢复UI状态
            _view.SetLoadingState(false);
            Debug.Log("==================== 🔐 登录流程结束 ====================\n");
        }
    }

    /// <summary>
    /// 验证用户输入
    /// </summary>
    private bool ValidateInput(string username, string password)
    {
        Debug.Log("🔍 [LoginController] 开始验证输入");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("⚠️ [LoginController] 用户名为空");
            _view.ShowErrorMessage("请输入用户名");
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("⚠️ [LoginController] 密码为空");
            _view.ShowErrorMessage("请输入密码");
            return false;
        }

        if (username.Length < 3)
        {
            Debug.LogWarning($"⚠️ [LoginController] 用户名过短: {username.Length} 字符");
            _view.ShowErrorMessage("用户名至少需要3个字符");
            return false;
        }

        if (password.Length < 6)
        {
            Debug.LogWarning($"⚠️ [LoginController] 密码过短: {password.Length} 字符");
            _view.ShowErrorMessage("密码至少需要6个字符");
            return false;
        }

        Debug.Log("✅ [LoginController] 输入验证通过");
        return true;
    }

    /// <summary>
    /// 处理登录成功
    /// </summary>
    private void HandleLoginSuccess(string token)
    {
        Debug.Log("✅ [LoginController] 登录成功！");
        Debug.Log($"🔑 [LoginController] Token: {token.Substring(0, Mathf.Min(30, token.Length))}...");
        Debug.Log($"📏 [LoginController] Token 长度: {token.Length} 字符");

        // 保存 Token
        PlayerPrefs.SetString("AuthToken", token);
        PlayerPrefs.SetString("Username", _view.GetUsername());
        PlayerPrefs.SetString("LoginTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        PlayerPrefs.Save();

        Debug.Log("💾 [LoginController] Token 已保存到 PlayerPrefs");

        // 显示成功消息
        _view.ShowSuccessMessage("登录成功！");

        // 延迟跳转到成绩查询场景
        Invoke(nameof(NavigateToScoreQuery), 1.5f);
    }

    /// <summary>
    /// 处理登录失败
    /// </summary>
    private void HandleLoginFailure(string message, int statusCode)
    {
        Debug.LogError("❌ [LoginController] 登录失败！");
        Debug.LogError($"   - 错误消息: {message}");
        Debug.LogError($"   - 状态码: {statusCode}");

        string errorMessage = "登录失败";

        // 根据状态码提供更友好的错误提示
        switch (statusCode)
        {
            case 400:
                errorMessage = "用户名或密码错误";
                break;
            case 401:
                errorMessage = "认证失败，请检查用户名和密码";
                break;
            case 404:
                errorMessage = "用户不存在";
                break;
            case 500:
                errorMessage = "服务器错误，请稍后重试";
                break;
            case 0:
                errorMessage = "无法连接到服务器，请检查网络";
                break;
            default:
                errorMessage = $"登录失败: {message}";
                break;
        }

        _view.ShowErrorMessage(errorMessage);
    }

    /// <summary>
    /// 跳转到成绩查询场景
    /// </summary>
    private void NavigateToScoreQuery()
    {
        Debug.Log("🚀 [LoginController] 准备跳转到成绩查询场景");

        // 使用场景管理器跳转
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreQueryScene");
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    public void Logout()
    {
        Debug.Log("👋 [LoginController] 用户退出登录");

        // 清除保存的数据
        PlayerPrefs.DeleteKey("AuthToken");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("LoginTime");
        PlayerPrefs.Save();

        Debug.Log("🗑️ [LoginController] 已清除本地登录信息");

        _view.ShowInfoMessage("已退出登录");
    }

    /// <summary>
    /// 测试连接（用于调试）
    /// </summary>
    public async void TestConnection()
    {
        Debug.Log("🔍 [LoginController] 测试服务器连接...");

        try
        {
            // 这里可以添加一个简单的 ping 接口测试
            _view.ShowInfoMessage("正在测试连接...");
            
            NetworkService.Instance.Login("test", "test123", (success, message) =>
            {
                if (success)
                {
                    Debug.Log("✅ 登录成功！");
                }
                else
                {
                    Debug.LogError($"❌ 登录失败: {message}");
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ [LoginController] 连接测试失败: {ex.Message}");
            _view.ShowErrorMessage("连接测试失败");
        }
    }
}