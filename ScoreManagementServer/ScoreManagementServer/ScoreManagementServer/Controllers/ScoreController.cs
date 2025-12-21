using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScoreManagementServer.Models;
using ScoreManagementServer.Services;
using System.Security.Claims;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly ScoreService _scoreService;

        public ScoreController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        #region 原有功能

        // POST api/score
        [HttpPost]
        public async Task<IActionResult> SaveScore([FromBody] ScoreRequest request)
        {
            await _scoreService.SaveScoreAsync(request.PlayerName, request.Score);
            return Ok(new { message = "Score saved successfully" });
        }

        // GET api/score/top?limit=10
        [HttpGet("top")]
        public async Task<IActionResult> GetTopScores([FromQuery] int limit = 10)
        {
            var scores = await _scoreService.GetTopScoresAsync(limit);
            return Ok(scores);
        }

        // GET api/score/player/{playerName}
        [HttpGet("player/{playerName}")]
        public async Task<IActionResult> GetPlayerBestScore(string playerName)
        {
            var score = await _scoreService.GetPlayerBestScoreAsync(playerName);
            return score != null ? Ok(score) : NotFound();
        }

        #endregion

        #region 学生查询成绩

        /// <summary>
        /// 学生查询自己的所有成绩
        /// GET api/score/student/my-scores?semester=2024-1&courseId=c001
        /// </summary>
        [HttpGet("student/my-scores")]
        [Authorize(Roles = "Student")] // 需要学生身份
        public async Task<IActionResult> GetMyScores(
            [FromQuery] string? semester = null,
            [FromQuery] string? courseId = null)
        {
            try
            {
                // 从 JWT Token 中获取学生ID
                var studentId = User.FindFirst("userId")?.Value 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(studentId))
                {
                    return Unauthorized(new { message = "无法获取学生身份信息" });
                }

                var scores = await _scoreService.GetStudentScoresAsync(
                    studentId, 
                    semester, 
                    courseId);

                return Ok(new ApiResponse<List<StudentScore>>
                {
                    Success = true,
                    Message = "获取成绩成功",
                    Data = scores
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取成绩失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 学生查询成绩统计信息（平均分、GPA等）
        /// GET api/score/student/statistics?semester=2024-1
        /// </summary>
        [HttpGet("student/statistics")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyStatistics([FromQuery] string? semester = null)
        {
            try
            {
                var studentId = User.FindFirst("userId")?.Value;
                
                if (string.IsNullOrEmpty(studentId))
                {
                    return Unauthorized(new { message = "无法获取学生身份信息" });
                }

                var statistics = await _scoreService.GetStudentStatisticsAsync(studentId, semester);

                return Ok(new ApiResponse<StudentStatistics>
                {
                    Success = true,
                    Message = "获取统计信息成功",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取统计信息失败: {ex.Message}"
                });
            }
        }

        #endregion

        #region 教师查询班级成绩

        /// <summary>
        /// 教师查询班级所有学生成绩
        /// GET api/score/teacher/class/{classId}?semester=2024-1&courseId=c001
        /// </summary>
        [HttpGet("teacher/class/{classId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetClassScores(
            string classId,
            [FromQuery] string? semester = null,
            [FromQuery] string? courseId = null)
        {
            try
            {
                // 从 JWT Token 中获取教师ID
                var teacherId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(teacherId))
                {
                    return Unauthorized(new { message = "无法获取教师身份信息" });
                }

                // 验证教师是否有权限查看该班级
                var hasPermission = await _scoreService.ValidateTeacherClassPermissionAsync(
                    teacherId, 
                    classId);

                if (!hasPermission)
                {
                    return Forbid("您没有权限查看该班级的成绩");
                }

                var scores = await _scoreService.GetClassScoresAsync(
                    classId, 
                    semester, 
                    courseId);

                return Ok(new ApiResponse<List<ClassScoreItem>>
                {
                    Success = true,
                    Message = "获取班级成绩成功",
                    Data = scores
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取班级成绩失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 教师查询班级成绩统计
        /// GET api/score/teacher/class/{classId}/statistics?courseId=c001&semester=2024-1
        /// </summary>
        [HttpGet("teacher/class/{classId}/statistics")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetClassStatistics(
            string classId,
            [FromQuery] string courseId,
            [FromQuery] string? semester = null)
        {
            try
            {
                var teacherId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(teacherId))
                {
                    return Unauthorized(new { message = "无法获取教师身份信息" });
                }

                // 验证权限
                var hasPermission = await _scoreService.ValidateTeacherClassPermissionAsync(
                    teacherId, 
                    classId);

                if (!hasPermission)
                {
                    return Forbid("您没有权限查看该班级的成绩");
                }

                if (string.IsNullOrEmpty(courseId))
                {
                    return BadRequest(new { message = "请指定课程ID" });
                }

                var statistics = await _scoreService.GetClassCourseStatisticsAsync(
                    classId, 
                    courseId, 
                    semester);

                return Ok(new ApiResponse<ClassStatistics>
                {
                    Success = true,
                    Message = "获取统计信息成功",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取统计信息失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 教师获取所管辖的班级列表
        /// GET api/score/teacher/my-classes
        /// </summary>
        [HttpGet("teacher/my-classes")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyClasses()
        {
            try
            {
                var teacherId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(teacherId))
                {
                    return Unauthorized(new { message = "无法获取教师身份信息" });
                }

                var classes = await _scoreService.GetTeacherClassesAsync(teacherId);

                return Ok(new ApiResponse<List<ClassInfo>>
                {
                    Success = true,
                    Message = "获取班级列表成功",
                    Data = classes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取班级列表失败: {ex.Message}"
                });
            }
        }

        #endregion

        #region 通用查询

        /// <summary>
        /// 获取可用的学期列表
        /// GET api/score/semesters
        /// </summary>
        [HttpGet("semesters")]
        [Authorize]
        public async Task<IActionResult> GetSemesters()
        {
            try
            {
                var semesters = await _scoreService.GetAvailableSemestersAsync();
                
                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Data = semesters
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取学期列表失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 获取课程列表
        /// GET api/score/courses?semester=2024-1
        /// </summary>
        [HttpGet("courses")]
        [Authorize]
        public async Task<IActionResult> GetCourses([FromQuery] string? semester = null)
        {
            try
            {
                var courses = await _scoreService.GetAvailableCoursesAsync(semester);
                
                return Ok(new ApiResponse<List<CourseInfo>>
                {
                    Success = true,
                    Data = courses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取课程列表失败: {ex.Message}"
                });
            }
        }

        #endregion
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
    public class StudentScore
    {
        public string ScoreId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public float Score { get; set; }
        public string Level { get; set; } = string.Empty; // 优秀/良好/及格/不及格
        public int Credits { get; set; } // 学分
        public DateTime ExamDate { get; set; }
        public DateTime CreatedAt { get; set; }
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
