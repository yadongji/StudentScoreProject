// Services/CsvImportService.cs
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;
using CsvHelper.Configuration.Attributes;

public class CsvImportService
{
    private readonly string _connectionString;

    public CsvImportService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<ImportResult> ImportFromCsvAsync(Stream fileStream)
    {
        var result = new ImportResult();
        
        try
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            });

            var records = csv.GetRecords<CsvStudentRecord>().ToList();
            
            using var connection = new SqliteConnection(_connectionString);  // 改用 SqliteConnection
            await connection.OpenAsync();

            foreach (var record in records)
            {
                try
                {
                    // SQLite 使用 INSERT OR REPLACE 代替 MERGE
                    var sql = @"
                        INSERT OR REPLACE INTO StudentScores 
                        (Id, StudentName, ClassName, ChineseScore, ChineseClassRank, ChineseGradeRank,
                         MathScore, MathClassRank, MathGradeRank, EnglishScore, EnglishClassRank, 
                         EnglishGradeRank, PhysicsScore, PhysicsClassRank, PhysicsGradeRank,
                         ChemistryScore, ChemistryClassRank, ChemistryGradeRank, BiologyScore, 
                         BiologyClassRank, BiologyGradeRank, UpdatedAt)
                        VALUES 
                        ((SELECT Id FROM StudentScores WHERE StudentName = @StudentName AND ClassName = @ClassName),
                         @StudentName, @ClassName, @ChineseScore, @ChineseClassRank, @ChineseGradeRank,
                         @MathScore, @MathClassRank, @MathGradeRank, @EnglishScore, @EnglishClassRank,
                         @EnglishGradeRank, @PhysicsScore, @PhysicsClassRank, @PhysicsGradeRank,
                         @ChemistryScore, @ChemistryClassRank, @ChemistryGradeRank, @BiologyScore,
                         @BiologyClassRank, @BiologyGradeRank, CURRENT_TIMESTAMP)";

                    await connection.ExecuteAsync(sql, record);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"学生 {record.StudentName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"导入失败: {ex.Message}");
        }

        return result;
    }
}

// CSV记录映射类
public class CsvStudentRecord
{
    [Name("姓名")]
    public string StudentName { get; set; }
    
    [Name("班级")]
    public string ClassName { get; set; }
    
    [Name("语文成绩")]
    public decimal? ChineseScore { get; set; }
    
    [Name("语文班级名次")]
    public int? ChineseClassRank { get; set; }
    
    [Name("语文年级名次")]
    public int? ChineseGradeRank { get; set; }
    
    [Name("数学成绩")]
    public decimal? MathScore { get; set; }
    
    [Name("数学班级名次")]
    public int? MathClassRank { get; set; }
    
    [Name("数学年级名次")]
    public int? MathGradeRank { get; set; }
    
    [Name("英语成绩")]
    public decimal? EnglishScore { get; set; }
    
    [Name("英语班级名次")]
    public int? EnglishClassRank { get; set; }
    
    [Name("英语年级名次")]
    public int? EnglishGradeRank { get; set; }
    
    [Name("物理成绩")]
    public decimal? PhysicsScore { get; set; }
    
    [Name("物理班级名次")]
    public int? PhysicsClassRank { get; set; }
    
    [Name("物理年级名次")]
    public int? PhysicsGradeRank { get; set; }
    
    [Name("化学成绩")]
    public decimal? ChemistryScore { get; set; }
    
    [Name("化学班级名次")]
    public int? ChemistryClassRank { get; set; }
    
    [Name("化学年级名次")]
    public int? ChemistryGradeRank { get; set; }
    
    [Name("生物成绩")]
    public decimal? BiologyScore { get; set; }
    
    [Name("生物班级名次")]
    public int? BiologyClassRank { get; set; }
    
    [Name("生物年级名次")]
    public int? BiologyGradeRank { get; set; }
}

public class ImportResult
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
