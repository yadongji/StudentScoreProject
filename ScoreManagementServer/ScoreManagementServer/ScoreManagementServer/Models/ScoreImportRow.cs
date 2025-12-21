// Models/ScoreImportModels.cs
namespace ScoreManagementServer.Models
{
    /// <summary>
    /// CSV 导入的单条成绩记录
    /// </summary>
    public class ScoreImportRow
    {
        public string ExamName { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public float Score { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
    }

    /// <summary>
    /// 导入结果
    /// </summary>
    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<ImportError> Errors { get; set; } = new();
    }

    /// <summary>
    /// 导入错误详情
    /// </summary>
    public class ImportError
    {
        public int RowNumber { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}