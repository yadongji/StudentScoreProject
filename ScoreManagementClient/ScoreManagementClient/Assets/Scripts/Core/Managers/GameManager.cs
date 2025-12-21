using UnityEngine;


/// <summary>
/// 游戏总控制器 - 管理整个应用的生命周期
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("管理器引用")] public UIManager UIManager;
    public SceneTransitionManager SceneManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        // 初始化各个管理器
        if (UIManager == null) UIManager = GetComponentInChildren<UIManager>();
        if (SceneManager == null) SceneManager = GetComponentInChildren<SceneTransitionManager>();

        DebugHelper.Log("✅ GameManager 初始化完成");
    }

    /// <summary>
    /// 获取特定功能的控制器
    /// </summary>
    public T GetController<T>() where T : Component
    {
        return GetComponentInChildren<T>();
    }
}