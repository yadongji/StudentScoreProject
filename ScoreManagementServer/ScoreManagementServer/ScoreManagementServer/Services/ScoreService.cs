// Services/StudentScoreService.cs
using Dapper;
using Microsoft.Data.Sqlite;
using ScoreManagementServer.Controllers;

public class ScoreService
{
    private readonly string _connectionString;

    public ScoreService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<StudentScoreDto> GetStudentScoreAsync(string studentName)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = "SELECT * FROM StudentScores WHERE StudentName = @StudentName";
        var score = await connection.QueryFirstOrDefaultAsync<StudentScore>(sql, new { StudentName = studentName });
        
        if (score == null)
            return null;

        var dto = new StudentScoreDto
        {
            StudentName = score.StudentName,
            ClassName = score.ClassName,
            Subjects = new List<SubjectScore>
            {
                new SubjectScore { SubjectName = "语文", Score = score.ChineseScore, ClassRank = score.ChineseClassRank, GradeRank = score.ChineseGradeRank },
                new SubjectScore { SubjectName = "数学", Score = score.MathScore, ClassRank = score.MathClassRank, GradeRank = score.MathGradeRank },
                new SubjectScore { SubjectName = "英语", Score = score.EnglishScore, ClassRank = score.EnglishClassRank, GradeRank = score.EnglishGradeRank },
                new SubjectScore { SubjectName = "物理", Score = score.PhysicsScore, ClassRank = score.PhysicsClassRank, GradeRank = score.PhysicsGradeRank },
                new SubjectScore { SubjectName = "化学", Score = score.ChemistryScore, ClassRank = score.ChemistryClassRank, GradeRank = score.ChemistryGradeRank },
                new SubjectScore { SubjectName = "生物", Score = score.BiologyScore, ClassRank = score.BiologyClassRank, GradeRank = score.BiologyGradeRank }
            }
        };

        var scores = dto.Subjects.Where(s => s.Score.HasValue).Select(s => s.Score.Value).ToList();
        dto.AverageScore = scores.Any() ? scores.Average() : 0;

        return dto;
    }

    public async Task<List<StudentScoreDto>> GetClassScoresAsync(string className)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = "SELECT * FROM StudentScores WHERE ClassName = @ClassName ORDER BY StudentName";
        var scores = await connection.QueryAsync<StudentScore>(sql, new { ClassName = className });
        
        return scores.Select(score => new StudentScoreDto
        {
            StudentName = score.StudentName,
            ClassName = score.ClassName,
            Subjects = new List<SubjectScore>
            {
                new SubjectScore { SubjectName = "语文", Score = score.ChineseScore, ClassRank = score.ChineseClassRank, GradeRank = score.ChineseGradeRank },
                new SubjectScore { SubjectName = "数学", Score = score.MathScore, ClassRank = score.MathClassRank, GradeRank = score.MathGradeRank },
                new SubjectScore { SubjectName = "英语", Score = score.EnglishScore, ClassRank = score.EnglishClassRank, GradeRank = score.EnglishGradeRank },
                new SubjectScore { SubjectName = "物理", Score = score.PhysicsScore, ClassRank = score.PhysicsClassRank, GradeRank = score.PhysicsGradeRank },
                new SubjectScore { SubjectName = "化学", Score = score.ChemistryScore, ClassRank = score.ChemistryClassRank, GradeRank = score.ChemistryGradeRank },
                new SubjectScore { SubjectName = "生物", Score = score.BiologyScore, ClassRank = score.BiologyClassRank, GradeRank = score.BiologyGradeRank }
            },
            AverageScore = new[] { score.ChineseScore, score.MathScore, score.EnglishScore, score.PhysicsScore, score.ChemistryScore, score.BiologyScore }
                .Where(s => s.HasValue).Select(s => s.Value).DefaultIfEmpty(0).Average()
        }).ToList();
    }
}
