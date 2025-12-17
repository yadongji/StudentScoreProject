using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// 排行榜视图 - 显示排行榜数据
/// </summary>
public class LeaderboardView : BaseView
{
    [Header("排行榜")] [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject scoreItemPrefab;
    [SerializeField] private Button refreshButton;

    // 事件
    public event Action OnRefresh;

    protected override void Awake()
    {
        base.Awake();
        refreshButton.onClick.AddListener(() => OnRefresh?.Invoke());
    }

    /// <summary>
    /// 更新排行榜显示
    /// </summary>
    public void UpdateLeaderboard(List<ScoreRecord> scores)
    {
        // 清空现有内容
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // 创建排行榜项
        for (int i = 0; i < scores.Count; i++)
        {
            var score = scores[i];
            GameObject item = Instantiate(scoreItemPrefab, leaderboardContent);

            var rankText = item.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            var nameText = item.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var scoreText = item.transform.Find("Score").GetComponent<TextMeshProUGUI>();

            rankText.text = $"#{i + 1}";
            nameText.text = score.PlayerName;
            scoreText.text = score.Score.ToString();
        }

        Debug.Log($"✅ 排行榜已更新，共 {scores.Count} 条记录");
    }
}