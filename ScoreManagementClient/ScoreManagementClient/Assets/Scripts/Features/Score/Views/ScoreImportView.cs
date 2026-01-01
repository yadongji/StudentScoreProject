using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SFB; // StandaloneFileBrowser（需要安装插件）

public class ScoreImportPanel : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private TMP_Dropdown examDropdown;

    [SerializeField] private Button selectFileButton;
    [SerializeField] private Button uploadButton;
    [SerializeField] private Button downloadTemplateButton;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Result Display")] [SerializeField]
    private GameObject resultPanel;

    [SerializeField] private TextMeshProUGUI resultMessageText;
    [SerializeField] private TextMeshProUGUI detailText;
    [SerializeField] private ScrollRect errorScrollRect;
    [SerializeField] private TextMeshProUGUI errorListText;

    private ScoreApiService apiService;
    private List<Exam> exams = new List<Exam>();
    private string selectedFilePath;
    private int selectedExamId = -1;

    void Start()
    {
        // 获取API服务
        apiService = GetComponent<ScoreApiService>();
        if (apiService == null)
        {
            apiService = gameObject.AddComponent<ScoreApiService>();
        }

        // 绑定事件
        selectFileButton.onClick.AddListener(OnSelectFile);
        uploadButton.onClick.AddListener(OnUpload);
        downloadTemplateButton.onClick.AddListener(OnDownloadTemplate);
        examDropdown.onValueChanged.AddListener(OnExamChanged);

        // 初始化UI
        uploadButton.interactable = false;
        progressPanel.SetActive(false);
        resultPanel.SetActive(false);
        fileNameText.text = "未选择文件";
        statusText.text = "请先选择考试和CSV文件";

        // 加载考试列表
        LoadExamList();
    }

    #region 加载考试列表

    void LoadExamList()
    {
        int gradeId = 1; // TODO: 从用户信息获取
        StartCoroutine(apiService.GetExamList(gradeId, OnExamsLoaded, OnLoadExamsFailed));
    }

    void OnExamsLoaded(List<Exam> loadedExams)
    {
        exams = loadedExams;
        examDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var exam in exams)
        {
            options.Add($"{exam.examName} ({exam.examDate})");
        }

        examDropdown.AddOptions(options);

        if (exams.Count > 0)
        {
            selectedExamId = exams[0].examId;
            UpdateUploadButton();
        }
    }

    void OnLoadExamsFailed(string error)
    {
        statusText.text = $"加载考试列表失败：{error}";
        statusText.color = Color.red;
    }

    void OnExamChanged(int index)
    {
        if (index >= 0 && index < exams.Count)
        {
            selectedExamId = exams[index].examId;
            UpdateUploadButton();
        }
    }

    #endregion

    #region 文件选择

    void OnSelectFile()
    {
        // 使用 StandaloneFileBrowser 打开文件选择对话框
        var extensions = new[]
        {
            new ExtensionFilter("CSV Files", "csv"),
            new ExtensionFilter("All Files", "*")
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "选择成绩CSV文件",
            "",
            extensions,
            false
        );

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            selectedFilePath = paths[0];
            fileNameText.text = System.IO.Path.GetFileName(selectedFilePath);
            fileNameText.color = Color.white;
            statusText.text = "文件已选择，可以开始上传";
            statusText.color = Color.green;
            UpdateUploadButton();
        }
    }

    void UpdateUploadButton()
    {
        uploadButton.interactable = selectedExamId > 0 && !string.IsNullOrEmpty(selectedFilePath);
    }

    #endregion

    #region 上传

    void OnUpload()
    {
        if (selectedExamId <= 0 || string.IsNullOrEmpty(selectedFilePath))
        {
            ShowError("请先选择考试和文件");
            return;
        }

        // 显示进度
        progressPanel.SetActive(true);
        resultPanel.SetActive(false);
        uploadButton.interactable = false;
        selectFileButton.interactable = false;
        progressText.text = "正在上传...";
        progressBar.value = 0.5f;

        StartCoroutine(apiService.ImportScoresFromCsv(
            selectedExamId,
            selectedFilePath,
            OnUploadSuccess,
            OnUploadFailed
        ));
    }

    void OnUploadSuccess(ImportResult result)
    {
        progressPanel.SetActive(false);
        uploadButton.interactable = true;
        selectFileButton.interactable = true;

        // 显示结果
        resultPanel.SetActive(true);

        if (result.success)
        {
            resultMessageText.text = "✓ 导入成功";
            resultMessageText.color = Color.green;
        }
        else
        {
            resultMessageText.text = "⚠ 部分导入失败";
            resultMessageText.color = Color.yellow;
        }

        detailText.text = $"总计：{result.totalRows} 条\n成功：{result.successRows} 条\n失败：{result.failedRows} 条";

        // 显示错误信息
        if (result.errors != null && result.errors.Count > 0)
        {
            errorListText.text = string.Join("\n", result.errors);
            errorScrollRect.gameObject.SetActive(true);
        }
        else
        {
            errorScrollRect.gameObject.SetActive(false);
        }

        statusText.text = result.message;
        statusText.color = result.success ? Color.green : Color.yellow;
    }

    void OnUploadFailed(string error)
    {
        progressPanel.SetActive(false);
        uploadButton.interactable = true;
        selectFileButton.interactable = true;

        ShowError(error);
    }

    void ShowError(string message)
    {
        resultPanel.SetActive(true);
        resultMessageText.text = "✗ 导入失败";
        resultMessageText.color = Color.red;
        detailText.text = message;
        errorScrollRect.gameObject.SetActive(false);
        statusText.text = "上传失败";
        statusText.color = Color.red;
    }

    #endregion

    #region 下载模板

    void OnDownloadTemplate()
    {
        string savePath = StandaloneFileBrowser.SaveFilePanel(
            "保存CSV模板",
            "",
            "scores_template.csv",
            "csv"
        );

        if (!string.IsNullOrEmpty(savePath))
        {
            apiService.GenerateCsvTemplate(savePath);
            statusText.text = $"模板已保存到：{savePath}";
            statusText.color = Color.green;
        }
    }

    #endregion
}