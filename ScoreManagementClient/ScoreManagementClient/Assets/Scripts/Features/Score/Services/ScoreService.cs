using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 成绩服务 - 处理成绩相关的业务逻辑
/// </summary>
public class ScoreService : HttpService
{
    private const string SCORE_ENDPOINT = "/score";

    /// <summary>
    /// 提交成绩
    /// </summary>
    public IEnumerator SubmitScore(string playerName, int score, Action<bool, string> callback)
    {
        var request = new ScoreRequest
        {
            PlayerName = playerName,
            Score = score
        };

        yield return Post<ScoreRequest, ServerResponse>(
            SCORE_ENDPOINT,
            request,
            response =>
            {
                Debug.Log($"✅ 成绩提交成功: {response.message}");
                callback?.Invoke(true, response.message);
            },
            error =>
            {
                Debug.LogError($"❌ 成绩提交失败: {error}");
                callback?.Invoke(false, $"提交失败: {error}");
            }
        );
    }

    /// <summary>
    /// 获取排行榜
    /// </summary>
    public IEnumerator GetTopScores(int limit, Action<List<ScoreRecord>> callback)
    {
        string endpoint = $"{SCORE_ENDPOINT}/top?limit={limit}";

        using (UnityEngine.Networking.UnityWebRequest www =
               UnityEngine.Networking.UnityWebRequest.Get($"{BaseUrl}{endpoint}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var scores = JsonHelper.ParseArray<ScoreRecord>(www.downloadHandler.text);
                Debug.Log($"✅ 获取到 {scores.Count} 条排行榜记录");
                callback?.Invoke(scores);
            }
            else
            {
                Debug.LogError($"❌ 获取排行榜失败: {www.error}");
                callback?.Invoke(new List<ScoreRecord>());
            }
        }
    }

    /// <summary>
    /// 获取玩家最高分
    /// </summary>
    public IEnumerator GetPlayerBestScore(string playerName, Action<ScoreRecord> callback)
    {
        string endpoint = $"{SCORE_ENDPOINT}/player/{UnityEngine.Networking.UnityWebRequest.EscapeURL(playerName)}";

        yield return Get<ScoreRecord>(
            endpoint,
            score =>
            {
                Debug.Log($"✅ 获取玩家最高分: {score.PlayerName} - {score.Score}");
                callback?.Invoke(score);
            },
            error =>
            {
                Debug.LogWarning($"⚠️ 未找到玩家记录: {playerName}");
                callback?.Invoke(null);
            }
        );
    }
}