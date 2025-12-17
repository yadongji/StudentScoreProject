/// <summary>
/// 3D 游戏控制器接口
/// </summary>
public interface IGame3DController
{
    void StartGame();
    void PauseGame();
    void ResumeGame();
    void EndGame();
    void GetGameScore();
}