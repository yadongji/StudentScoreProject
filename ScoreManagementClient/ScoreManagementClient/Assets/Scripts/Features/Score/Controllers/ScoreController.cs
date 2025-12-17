using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 成绩控制器 - 协调 Model 和 View
/// </summary>
public class ScoreController : MonoBehaviour
{
    [Header("服务")] [SerializeField] private ScoreService scoreService;

    [Header("视图")] [SerializeField] private ScoreView scoreView;
    [SerializeField] private LeaderboardView leaderboardView;

    private void Start()
    {
        InitializeViews();
    }

    private void InitializeViews()
    {
        // 绑定视图事件
        if (scoreView != null)
        {
            scoreView.OnSubmitScore += HandleSubmitScore;
            scoreView.OnSearchPlayer += HandleSearchPlayer;
        }

        if (leaderboardView != null)
        {
            leaderboardView.OnRefresh += HandleRefreshLeaderboard;
        }

        // 初始加载排行榜
        RefreshLeaderboard();
    }

    // 处理提交成绩
    private void HandleSubmitScore(string playerName, int score)
    {
        StartCoroutine(scoreService.SubmitScore(playerName, score, (success, message) =>
        {
            scoreView?.ShowSubmitResult(success, message);

            if (success)
            {
                RefreshLeaderboard();
            }
        }));
    }

    // 处理查询玩家
    private void HandleSearchPlayer(string playerName)
    {
        StartCoroutine(scoreService.GetPlayerBestScore(playerName, (score) => { scoreView?.ShowPlayerScore(score); }));
    }

    // 处理刷新排行榜
    private void HandleRefreshLeaderboard()
    {
        RefreshLeaderboard();
    }

    // 刷新排行榜
    private void RefreshLeaderboard()
    {
        StartCoroutine(scoreService.GetTopScores(10, (scores) => { leaderboardView?.UpdateLeaderboard(scores); }));
    }

    private void OnDestroy()
    {
        // 解绑事件
        if (scoreView != null)
        {
            scoreView.OnSubmitScore -= HandleSubmitScore;
            scoreView.OnSearchPlayer -= HandleSearchPlayer;
        }

        if (leaderboardView != null)
        {
            leaderboardView.OnRefresh -= HandleRefreshLeaderboard;
        }
    }
}