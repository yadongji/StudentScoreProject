using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoreManagementServer.Models;
using ScoreManagementServer.Services;
using System.Security.Claims;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Controllers/StudentScoreController.cs
    public class StudentScoreController : ControllerBase
    {
        private readonly CsvImportService _csvImportService;
        private readonly ScoreService _studentScoreService;

        public StudentScoreController(CsvImportService csvImportService, ScoreService studentScoreService)
        {
            _csvImportService = csvImportService;
            _studentScoreService = studentScoreService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("请上传文件");

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest("只支持CSV格式文件");

            using var stream = file.OpenReadStream();
            var result = await _csvImportService.ImportFromCsvAsync(stream);

            return Ok(new
            {
                success = result.FailedCount == 0,
                successCount = result.SuccessCount,
                failedCount = result.FailedCount,
                errors = result.Errors
            });
        }

        [HttpGet("student/{studentName}")]
        public async Task<IActionResult> GetStudentScore(string studentName)
        {
            var score = await _studentScoreService.GetStudentScoreAsync(studentName);
        
            if (score == null)
                return NotFound($"未找到学生: {studentName}");

            return Ok(score);
        }

        [HttpGet("class/{className}")]
        public async Task<IActionResult> GetClassScores(string className)
        {
            var scores = await _studentScoreService.GetClassScoresAsync(className);
            return Ok(scores);
        }
    }


    #region 数据模型

    // 原有模型
    public class ScoreRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    // 通用响应模型
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    // 学生成绩模型
    // Models/StudentScore.cs
    public class StudentScore
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
    
        // 语文
        public decimal? ChineseScore { get; set; }
        public int? ChineseClassRank { get; set; }
        public int? ChineseGradeRank { get; set; }
    
        // 数学
        public decimal? MathScore { get; set; }
        public int? MathClassRank { get; set; }
        public int? MathGradeRank { get; set; }
    
        // 英语
        public decimal? EnglishScore { get; set; }
        public int? EnglishClassRank { get; set; }
        public int? EnglishGradeRank { get; set; }
    
        // 物理
        public decimal? PhysicsScore { get; set; }
        public int? PhysicsClassRank { get; set; }
        public int? PhysicsGradeRank { get; set; }
    
        // 化学
        public decimal? ChemistryScore { get; set; }
        public int? ChemistryClassRank { get; set; }
        public int? ChemistryGradeRank { get; set; }
    
        // 生物
        public decimal? BiologyScore { get; set; }
        public int? BiologyClassRank { get; set; }
        public int? BiologyGradeRank { get; set; }
    
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

// DTO for API responses
    public class StudentScoreDto
    {
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public List<SubjectScore> Subjects { get; set; }
        public decimal AverageScore { get; set; }
    }

    public class SubjectScore
    {
        public string SubjectName { get; set; }
        public decimal? Score { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
    }


    // 学生统计信息
    public class StudentStatistics
    {
        public float AverageScore { get; set; }
        public float GPA { get; set; }
        public int TotalCredits { get; set; }
        public int PassedCourses { get; set; }
        public int FailedCourses { get; set; }
        public Dictionary<string, int> LevelDistribution { get; set; } = new();
    }

    // 班级成绩项
    public class ClassScoreItem
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public List<StudentScore> Scores { get; set; } = new();
        public float AverageScore { get; set; }
    }

    // 班级统计信息
    public class ClassStatistics
    {
        public string CourseName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public float AverageScore { get; set; }
        public float HighestScore { get; set; }
        public float LowestScore { get; set; }
        public int PassCount { get; set; }
        public float PassRate { get; set; }
        public Dictionary<string, int> LevelDistribution { get; set; } = new();
        public List<ScoreRanking> TopStudents { get; set; } = new();
    }

    // 成绩排名
    public class ScoreRanking
    {
        public int Rank { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public float Score { get; set; }
    }

    // 班级信息
    public class ClassInfo
    {
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty; // 年级
        public int StudentCount { get; set; }
    }

    // 课程信息
    public class CourseInfo
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
    }

    #endregion
}
