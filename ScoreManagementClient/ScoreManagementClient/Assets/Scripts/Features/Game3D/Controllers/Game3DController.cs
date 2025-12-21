using UnityEngine;


/// <summary>
/// 3D 游戏控制器
/// </summary>
public class Game3DController : MonoBehaviour, IGame3DController
{
    private int _gameScore;
    private bool _isGamePaused;

    public int GameScore => _gameScore;

    void Start()
    {
        _gameScore = 0;
        _isGamePaused = false;
    }

    /// <summary>
    /// 启动游戏
    /// </summary>
    public void StartGame()
    {
        DebugHelper.Log("🎮 游戏开始！");
        _gameScore = 0; // 重置分数
        // 其他游戏初始化代码
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        if (!_isGamePaused)
        {
            DebugHelper.Log("⏸️ 游戏暂停");
            _isGamePaused = true;
            Time.timeScale = 0f; // 暂停游戏时间流动
        }
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        if (_isGamePaused)
        {
            DebugHelper.Log("▶️ 游戏继续");
            _isGamePaused = false;
            Time.timeScale = 1f; // 恢复游戏时间流动
        }
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void EndGame()
    {
        DebugHelper.Log($"🎮 游戏结束，最终得分：{_gameScore}");
        // 这里可以提交分数到服务器
    }

    /// <summary>
    /// 获取游戏分数
    /// </summary>
    public void GetGameScore()
    {
        DebugHelper.Log($"🎮 当前分数：{_gameScore}");
    }
}