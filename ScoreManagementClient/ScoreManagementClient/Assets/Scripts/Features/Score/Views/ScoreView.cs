using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 成绩视图 - 负责成绩提交和查询的 UI
/// </summary>
public class ScoreView : BaseView
{
    [Header("提交成绩")] [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField scoreInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI submitResultText;

    [Header("查询玩家")] [SerializeField] private TMP_InputField searchPlayerInput;
    [SerializeField] private Button searchButton;
    [SerializeField] private TextMeshProUGUI playerScoreText;

    // 事件
    public event Action<string, int> OnSubmitScore;
    public event Action<string> OnSearchPlayer;

    protected override void Awake()
    {
        base.Awake();

        // 绑定按钮事件
        submitButton.onClick.AddListener(HandleSubmitClick);
        searchButton.onClick.AddListener(HandleSearchClick);
    }

    private void HandleSubmitClick()
    {
        string playerName = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            ShowSubmitResult(false, "请输入玩家名称");
            return;
        }

        if (!int.TryParse(scoreInput.text, out int score))
        {
            ShowSubmitResult(false, "请输入有效分数");
            return;
        }

        submitResultText.text = "⏳ 提交中...";
        OnSubmitScore?.Invoke(playerName, score);
    }

    private void HandleSearchClick()
    {
        string playerName = searchPlayerInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            playerScoreText.text = "❌ 请输入玩家名称";
            return;
        }

        playerScoreText.text = "⏳ 查询中...";
        OnSearchPlayer?.Invoke(playerName);
    }

    // 显示提交结果
    public void ShowSubmitResult(bool success, string message)
    {
        submitResultText.text = success ? $"✅ {message}" : $"❌ {message}";

        if (success)
        {
            scoreInput.text = "";
        }
    }

    // 显示玩家成绩
    public void ShowPlayerScore(ScoreRecord score)
    {
        if (score != null)
        {
            playerScoreText.text = $"🏆 {score.PlayerName} 最高分: {score.Score}";
        }
        else
        {
            playerScoreText.text = "❌ 未找到该玩家记录";
        }
    }
}