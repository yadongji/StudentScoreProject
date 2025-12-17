using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    // 切换到指定场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 异步加载场景
    public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // 切换到成绩管理场景
    public void LoadScoreManagementScene()
    {
        LoadScene("ScoreManagement");
    }

    // 切换到3D游戏场景
    public void LoadGame3DScene()
    {
        LoadScene("Game3D");
    }
}