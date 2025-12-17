using System;
using System.Collections.Generic;


/// <summary>
/// 成绩数据模型
/// </summary>
[Serializable]
public class ScoreRecord
{
    public string PlayerName;
    public int Score;
    public string CreatedAt;

    public ScoreRecord(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}

[Serializable]
public class ScoreRequest
{
    public string PlayerName;
    public int Score;
}

[Serializable]
public class ServerResponse
{
    public string message;
}

[Serializable]
public class ScoreListWrapper
{
    public List<ScoreRecord> scores;
}