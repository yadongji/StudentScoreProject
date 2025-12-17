using System.Collections.Generic;

namespace ScoreManagementServer.DTOs
{
    // 通用响应
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "成功")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }
    }

    // 登录请求
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // 登录响应
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public RoleSpecificData RoleData { get; set; }
    }

    // 角色特定数据
    public class RoleSpecificData
    {
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? GradeId { get; set; }
        public List<int> ManagedClassIds { get; set; }
    }

    // 学生成绩详情
    public class StudentScoreDetail
    {
        public int ScoreId { get; set; }
        public string ExamName { get; set; }
        public string ExamDate { get; set; }
        public string SubjectName { get; set; }
        public double Score { get; set; }
        public double MaxScore { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
    }

    // 班级成绩汇总
    public class ClassScoreSummary
    {
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public List<SubjectScore> Subjects { get; set; }
    }

    public class SubjectScore
    {
        public string SubjectName { get; set; }
        public double Score { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
    }

    // 成绩提交请求（CSV批量导入）
    public class ScoreSubmitRequest
    {
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public List<ScoreEntry> Scores { get; set; }
    }

    public class ScoreEntry
    {
        public string StudentNumber { get; set; }
        public double Score { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
    }

    // 考试列表项
    public class ExamItem
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamDate { get; set; }
        public string GradeName { get; set; }
    }
}
