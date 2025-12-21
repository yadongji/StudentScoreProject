using ScoreManagementServer.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreManagementServer.Services
{
    public class ScoreService
    {
        // 模拟数据库 - 实际项目中应该使用真实数据库
        private static List<StudentScore> _scores = new()
        {
            new StudentScore 
            { 
                ScoreId = "sc001",
                CourseId = "c001",
                CourseName = "高等数学",
                Semester = "2024-1",
                Score = 85.5f,
                Level = "良好",
                Credits = 4,
                ExamDate = DateTime.Now.AddDays(-30)
            },
            // ... 更多模拟数据
        };

        private static Dictionary<string, List<string>> _teacherClasses = new()
        {
            { "teacher001", new List<string> { "class001", "class002" } },
            { "teacher002", new List<string> { "class003" } }
        };

        #region 原有方法

        public async Task SaveScoreAsync(string playerName, int score)
        {
            // 原有实现
            await Task.CompletedTask;
        }

        public async Task<List<object>> GetTopScoresAsync(int limit)
        {
            // 原有实现
            return await Task.FromResult(new List<object>());
        }

        public async Task<object?> GetPlayerBestScoreAsync(string playerName)
        {
            // 原有实现
            return await Task.FromResult<object?>(null);
        }

        #endregion

        #region 学生查询

        public async Task<List<StudentScore>> GetStudentScoresAsync(
            string studentId, 
            string? semester = null, 
            string? courseId = null)
        {
            var query = _scores.Where(s => s.ScoreId.StartsWith(studentId));

            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(s => s.Semester == semester);
            }

            if (!string.IsNullOrEmpty(courseId))
            {
                query = query.Where(s => s.CourseId == courseId);
            }

            return await Task.FromResult(
                query.OrderByDescending(s => s.ExamDate).ToList()
            );
        }

        public async Task<StudentStatistics> GetStudentStatisticsAsync(
            string studentId, 
            string? semester = null)
        {
            var scores = await GetStudentScoresAsync(studentId, semester);

            if (!scores.Any())
            {
                return new StudentStatistics();
            }

            return new StudentStatistics
            {
                AverageScore = scores.Average(s => s.Score),
                GPA = CalculateGPA(scores),
                TotalCredits = scores.Sum(s => s.Credits),
                PassedCourses = scores.Count(s => s.Score >= 60),
                FailedCourses = scores.Count(s => s.Score < 60),
                LevelDistribution = scores
                    .GroupBy(s => s.Level)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        #endregion

        #region 教师查询

        public async Task<bool> ValidateTeacherClassPermissionAsync(
            string teacherId, 
            string classId)
        {
            return await Task.FromResult(
                _teacherClasses.ContainsKey(teacherId) && 
                _teacherClasses[teacherId].Contains(classId)
            );
        }

        public async Task<List<ClassScoreItem>> GetClassScoresAsync(
            string classId, 
            string? semester = null, 
            string? courseId = null)
        {
            // 模拟获取班级学生成绩
            // 实际应该从数据库查询
            var classScores = new List<ClassScoreItem>();
            
            return await Task.FromResult(classScores);
        }

        public async Task<ClassStatistics> GetClassCourseStatisticsAsync(
            string classId, 
            string courseId, 
            string? semester = null)
        {
            var scores = await GetClassScoresAsync(classId, semester, courseId);
            
            var allScores = scores
                .SelectMany(s => s.Scores)
                .Where(s => s.CourseId == courseId)
                .ToList();

            if (!allScores.Any())
            {
                return new ClassStatistics();
            }

            return new ClassStatistics
            {
                CourseName = allScores.First().CourseName,
                TotalStudents = scores.Count,
                AverageScore = allScores.Average(s => s.Score),
                HighestScore = allScores.Max(s => s.Score),
                LowestScore = allScores.Min(s => s.Score),
                PassCount = allScores.Count(s => s.Score >= 60),
                PassRate = allScores.Count(s => s.Score >= 60) * 100f / allScores.Count,
                LevelDistribution = allScores
                    .GroupBy(s => s.Level)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TopStudents = scores
                    .OrderByDescending(s => s.AverageScore)
                    .Take(5)
                    .Select((s, index) => new ScoreRanking
                    {
                        Rank = index + 1,
                        StudentName = s.StudentName,
                        Score = s.AverageScore
                    })
                    .ToList()
            };
        }

        public async Task<List<ClassInfo>> GetTeacherClassesAsync(string teacherId)
        {
            if (!_teacherClasses.ContainsKey(teacherId))
            {
                return new List<ClassInfo>();
            }

            // 模拟返回班级信息
            var classes = _teacherClasses[teacherId]
                .Select(classId => new ClassInfo
                {
                    ClassId = classId,
                    ClassName = $"班级{classId}",
                    Grade = "2024",
                    StudentCount = 30
                })
                .ToList();

            return await Task.FromResult(classes);
        }

        #endregion

        #region 通用方法

        public async Task<List<string>> GetAvailableSemestersAsync()
        {
            var semesters = _scores
                .Select(s => s.Semester)
                .Distinct()
                .OrderByDescending(s => s)
                .ToList();

            return await Task.FromResult(semesters);
        }

        public async Task<List<CourseInfo>> GetAvailableCoursesAsync(string? semester = null)
        {
            var query = _scores.AsEnumerable();

            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(s => s.Semester == semester);
            }

            var courses = query
                .Select(s => new CourseInfo
                {
                    CourseId = s.CourseId,
                    CourseName = s.CourseName,
                    Credits = s.Credits
                })
                .DistinctBy(c => c.CourseId)
                .ToList();

            return await Task.FromResult(courses);
        }

        private float CalculateGPA(List<StudentScore> scores)
        {
            if (!scores.Any()) return 0;

            float totalPoints = 0;
            int totalCredits = 0;

            foreach (var score in scores)
            {
                float gradePoint = score.Score >= 90 ? 4.0f :
                                  score.Score >= 80 ? 3.0f :
                                  score.Score >= 70 ? 2.0f :
                                  score.Score >= 60 ? 1.0f : 0f;

                totalPoints += gradePoint * score.Credits;
                totalCredits += score.Credits;
            }

            return totalCredits > 0 ? totalPoints / totalCredits : 0;
        }

        #endregion
    }
}
