using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class ScoreApiService : MonoBehaviour
{
    private const string API_BASE_URL = "http://localhost:5000/api";

    #region 上传CSV文件

    /// <summary>
    /// 上传CSV文件导入成绩
    /// </summary>
    public IEnumerator ImportScoresFromCsv(int examId, string filePath, Action<ImportResult> onSuccess,
        Action<string> onError)
    {
        // 读取文件
        if (!File.Exists(filePath))
        {
            onError?.Invoke("文件不存在：" + filePath);
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        // 创建表单
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", fileData, fileName, "text/csv")
        };

        string url = $"{API_BASE_URL}/score/import/{examId}";

        using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
        {
            // 发送请求
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ImportResult result = JsonUtility.FromJson<ImportResult>(jsonResponse);
                onSuccess?.Invoke(result);
            }
            else
            {
                onError?.Invoke($"上传失败：{request.error}\n{request.downloadHandler.text}");
            }
        }
    }

    /// <summary>
    /// 上传已加载的CSV数据
    /// </summary>
    public IEnumerator ImportScoresFromBytes(int examId, byte[] fileData, string fileName,
        Action<ImportResult> onSuccess, Action<string> onError)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", fileData, fileName, "text/csv")
        };

        string url = $"{API_BASE_URL}/score/import/{examId}";

        using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ImportResult result = JsonUtility.FromJson<ImportResult>(jsonResponse);
                onSuccess?.Invoke(result);
            }
            else
            {
                onError?.Invoke($"上传失败：{request.error}\n{request.downloadHandler.text}");
            }
        }
    }

    #endregion

    #region 下载模板

    /// <summary>
    /// 生成并保存CSV模板
    /// </summary>
    public void GenerateCsvTemplate(string savePath)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine(
            "StudentNumber,ChineseScore,MathScore,EnglishScore,PhysicsScore,ChemistryScore,BiologyScore,PoliticsScore");
        csv.AppendLine("202401001,130,142,135,88,92,85,");
        csv.AppendLine("202401002,125,138,128,92,85,90,");
        csv.AppendLine("202401003,135,145,140,95,90,88,");

        try
        {
            // 使用 UTF-8 with BOM 编码（支持中文）
            File.WriteAllText(savePath, csv.ToString(), new UTF8Encoding(true));
            Debug.Log($"模板已保存到：{savePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"保存模板失败：{ex.Message}");
        }
    }

    #endregion

    #region 获取考试列表

    /// <summary>
    /// 获取考试列表
    /// </summary>
    public IEnumerator GetExamList(int gradeId, Action<List<Exam>> onSuccess, Action<string> onError)
    {
        string url = $"{API_BASE_URL}/exam/grade/{gradeId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                // 处理 JSON 数组
                ExamListWrapper wrapper = JsonUtility.FromJson<ExamListWrapper>("{\"exams\":" + jsonResponse + "}");
                onSuccess?.Invoke(wrapper.exams);
            }
            else
            {
                onError?.Invoke($"获取考试列表失败：{request.error}");
            }
        }
    }

    [Serializable]
    private class ExamListWrapper
    {
        public List<Exam> exams;
    }

    #endregion

    #region CSV解析工具

    /// <summary>
    /// 解析CSV文件为成绩数据列表
    /// </summary>
    public List<ScoreImportDto> ParseCsvFile(string filePath)
    {
        List<ScoreImportDto> scores = new List<ScoreImportDto>();

        try
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // 跳过表头
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split(',');

                if (values.Length < 8) continue;

                ScoreImportDto score = new ScoreImportDto
                {
                    StudentNumber = values[0],
                    ChineseScore = ParseNullableFloat(values[1]),
                    MathScore = ParseNullableFloat(values[2]),
                    EnglishScore = ParseNullableFloat(values[3]),
                    PhysicsScore = ParseNullableFloat(values[4]),
                    ChemistryScore = ParseNullableFloat(values[5]),
                    BiologyScore = ParseNullableFloat(values[6]),
                    PoliticsScore = ParseNullableFloat(values[7])
                };

                scores.Add(score);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"解析CSV失败：{ex.Message}");
        }

        return scores;
    }

    private float? ParseNullableFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (float.TryParse(value, out float result))
            return result;

        return null;
    }

    #endregion
}