using System;
using System.Collections.Generic;

[Serializable]
public class ImportResult
{
    public bool success;
    public string message;
    public int totalRows;
    public int successRows;
    public int failedRows;
    public List<string> errors;
}

[Serializable]
public class ScoreImportDto
{
    public string StudentNumber;
    public float? ChineseScore;
    public float? MathScore;
    public float? EnglishScore;
    public float? PhysicsScore;
    public float? ChemistryScore;
    public float? BiologyScore;
    public float? PoliticsScore;
}

[Serializable]
public class Exam
{
    public int examId;
    public string examName;
    public string examType;
    public string examDate;
    public int gradeId;
}