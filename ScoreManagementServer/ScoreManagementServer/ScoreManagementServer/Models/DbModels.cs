using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoreManagementServer.Models
{
    // 用户表
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string RealName { get; set; }
        public string CreatedAt { get; set; }
    }

    // 年级表
    [Table("Grades")]
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }
        [Required]
        public string GradeName { get; set; }
        public int GradeYear { get; set; }
    }

    // 班级表
    [Table("Classes")]
    public class Class
    {
        [Key]
        public int ClassId { get; set; }
        public int GradeId { get; set; }
        [Required]
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
    }

    // 学生信息表
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public int UserId { get; set; }
        [Required]
        public string StudentNumber { get; set; }
        public int ClassId { get; set; }
    }

    // 教师信息表
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        public int UserId { get; set; }
    }

    // 年级主任表
    [Table("GradeDirectors")]
    public class GradeDirector
    {
        [Key]
        public int DirectorId { get; set; }
        public int UserId { get; set; }
        public int GradeId { get; set; }
    }

    // 考试表
    [Table("Exams")]
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }
        [Required]
        public string ExamName { get; set; }
        [Required]
        public string ExamDate { get; set; }
        public int GradeId { get; set; }
        public string CreatedAt { get; set; }
    }

    // 科目表
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        [Required]
        public string SubjectName { get; set; }
        [Required]
        public string SubjectCategory { get; set; }
        public double MaxScore { get; set; }
    }

    // 成绩表
    [Table("Scores")]
    public class Score
    {
        [Key]
        public int ScoreId { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public double score { get; set; }
        public int? ClassRank { get; set; }
        public int? GradeRank { get; set; }
        public int SubmittedBy { get; set; }
        public string SubmittedAt { get; set; }
    }
}
