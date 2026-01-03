-- 高中成绩管理系统数据库设计(简化解耦版)
-- 设计原则: 高度解耦,以查询为主,支持成绩趋势分析

-- ============================================
-- 1. 学生表(核心表)
-- ============================================
CREATE TABLE Students (
    StudentId INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentNumber TEXT UNIQUE,               -- 学号(唯一标识，可为空，用于学号变更时废弃旧学号)
    StudentName TEXT NOT NULL,              -- 姓名
    ClassName TEXT,                         -- 班级(可选,用于分组查询)
    Gender TEXT CHECK(Gender IN ('男', '女')), -- 性别
    EnrollmentDate TEXT,                     -- 入学日期
    IsActive INTEGER DEFAULT 1,              -- 是否在读
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime'))
);

-- ============================================
-- 2. 考试表
-- ============================================
CREATE TABLE Exams (
    ExamId INTEGER PRIMARY KEY AUTOINCREMENT,
    ExamName TEXT NOT NULL,                 -- 考试名称
    ExamType TEXT NOT NULL CHECK(ExamType IN ('月考', '期中考', '期末考', '模拟考', '联考')),
    ExamDate TEXT NOT NULL,                 -- 考试日期
    GradeName TEXT NOT NULL,                -- 年级
    Term TEXT CHECK(Term IN ('上学期', '下学期')), -- 学期
    AcademicYear TEXT,                      -- 学年(如: 2024-2025)
    IsPublished INTEGER DEFAULT 0,           -- 是否发布
    Description TEXT,                       -- 描述
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime'))
);

-- ============================================
-- 3. 科目表
-- ============================================
CREATE TABLE Subjects (
    SubjectId INTEGER PRIMARY KEY AUTOINCREMENT,
    SubjectName TEXT NOT NULL UNIQUE,       -- 科目名称
    SubjectCode TEXT NOT NULL UNIQUE,       -- 科目代码
    Category TEXT CHECK(Category IN ('文科', '理科', '通用')), -- 科目类别
    MaxScore REAL DEFAULT 100,             -- 满分
    SortOrder INTEGER DEFAULT 0,           -- 排序
    IsActive INTEGER DEFAULT 1
);

-- ============================================
-- 4. 成绩表(核心表)
-- ============================================
CREATE TABLE Scores (
    ScoreId INTEGER PRIMARY KEY AUTOINCREMENT,
    ExamId INTEGER NOT NULL,               -- 考试ID
    StudentId INTEGER NOT NULL,            -- 学生ID
    SubjectId INTEGER NOT NULL,            -- 科目ID (10表示总分)
    Score REAL NOT NULL,                   -- 得分
    ClassRank INTEGER,                      -- 班级排名
    GradeRank INTEGER,                      -- 年级排名
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ExamId) REFERENCES Exams(ExamId) ON DELETE CASCADE,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE,
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId) ON DELETE CASCADE,
    UNIQUE(ExamId, StudentId, SubjectId) -- 确保同一学生在同一考试同一科目只有一条成绩
);

-- ============================================
-- 索引创建(优化查询性能)
-- ============================================

-- 学生表索引
CREATE INDEX idx_students_number ON Students(StudentNumber);
CREATE INDEX idx_students_name ON Students(StudentName);
CREATE INDEX idx_students_class ON Students(ClassName);

-- 考试表索引
CREATE INDEX idx_exams_date ON Exams(ExamDate DESC);
CREATE INDEX idx_exams_grade ON Exams(GradeName);
CREATE INDEX idx_exams_type ON Exams(ExamType);

-- 成绩表索引(重点优化)
CREATE INDEX idx_scores_exam ON Scores(ExamId);
CREATE INDEX idx_scores_student ON Scores(StudentId);
CREATE INDEX idx_scores_subject ON Scores(SubjectId);
CREATE INDEX idx_scores_score ON Scores(Score DESC);  -- 成绩降序
CREATE INDEX idx_scores_exam_student ON Scores(ExamId, StudentId);  -- 考试+学生组合
CREATE INDEX idx_scores_student_exam ON Scores(StudentId, ExamId);  -- 学生+考试组合(趋势分析)

-- ============================================
-- 视图创建(简化查询)
-- ============================================

-- 视图1: 学生成绩完整详情
CREATE VIEW IF NOT EXISTS vw_StudentScoreDetail AS
SELECT
    s.ScoreId,
    e.ExamId,
    e.ExamName,
    e.ExamType,
    e.ExamDate,
    e.GradeName,
    st.StudentId,
    st.StudentNumber,
    st.StudentName,
    st.ClassName,
    sb.SubjectId,
    sb.SubjectName,
    sb.SubjectCode,
    s.Score,
    sb.MaxScore,
    s.ClassRank,
    s.GradeRank
FROM Scores s
JOIN Exams e ON s.ExamId = e.ExamId
JOIN Students st ON s.StudentId = st.StudentId
JOIN Subjects sb ON s.SubjectId = sb.SubjectId
ORDER BY e.ExamDate DESC, st.StudentNumber;

-- 视图2: 学生成绩汇总(按考试,排除总分)
CREATE VIEW IF NOT EXISTS vw_StudentExamSummary AS
SELECT
    e.ExamId,
    e.ExamName,
    e.ExamDate,
    e.ExamType,
    st.StudentId,
    st.StudentNumber,
    st.StudentName,
    st.ClassName,
    COUNT(s.ScoreId) as SubjectCount,
    SUM(s.Score) as TotalScore,
    AVG(s.Score) as AvgScore,
    MAX(s.Score) as MaxScore,
    MIN(s.Score) as MinScore
FROM Exams e
JOIN Scores s ON e.ExamId = s.ExamId AND s.SubjectId != 10  -- 排除总分科目
JOIN Students st ON s.StudentId = st.StudentId
GROUP BY e.ExamId, st.StudentId
ORDER BY e.ExamDate DESC, TotalScore DESC;

-- 视图3: 科目平均分统计(班级/年级)
CREATE VIEW IF NOT EXISTS vw_SubjectStats AS
SELECT
    e.ExamId,
    e.ExamName,
    e.ExamDate,
    sb.SubjectName,
    st.ClassName,
    COUNT(s.ScoreId) as StudentCount,
    AVG(s.Score) as AvgScore,
    MAX(s.Score) as MaxScore,
    MIN(s.Score) as MinScore,
    ROUND(AVG(s.Score), 2) as AvgScoreRounded,
    ROUND(MAX(s.Score), 2) as MaxScoreRounded,
    ROUND(MIN(s.Score), 2) as MinScoreRounded
FROM Exams e
JOIN Scores s ON e.ExamId = s.ExamId
JOIN Students st ON s.StudentId = st.StudentId
JOIN Subjects sb ON s.SubjectId = sb.SubjectId
GROUP BY e.ExamId, sb.SubjectId, st.ClassName
ORDER BY e.ExamDate DESC, sb.SubjectName;

-- 视图4: 学生成绩趋势分析(跨考试对比)
CREATE VIEW IF NOT EXISTS vw_StudentScoreTrend AS
WITH RankedScores AS (
    SELECT
        st.StudentId,
        st.StudentNumber,
        st.StudentName,
        st.ClassName,
        sb.SubjectName,
        e.ExamId,
        e.ExamName,
        e.ExamDate,
        e.ExamType,
        s.Score,
        s.ClassRank,
        s.GradeRank,
        -- 考试排名(按日期)
        ROW_NUMBER() OVER (PARTITION BY st.StudentId, sb.SubjectId ORDER BY e.ExamDate) as ExamSeq
    FROM Students st
    JOIN Scores s ON st.StudentId = s.StudentId
    JOIN Exams e ON s.ExamId = e.ExamId
    JOIN Subjects sb ON s.SubjectId = sb.SubjectId
)
SELECT
    StudentId,
    StudentNumber,
    StudentName,
    ClassName,
    SubjectName,
    ExamId,
    ExamName,
    ExamDate,
    ExamType,
    Score,
    ClassRank,
    GradeRank,
    ExamSeq,
    -- 与上次考试对比
    LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) as PrevScore,
    LAG(ClassRank, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) as PrevClassRank,
    LAG(GradeRank, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) as PrevGradeRank,
    -- 进步/退步计算
    CASE
        WHEN LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) IS NOT NULL
        THEN Score - LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate)
    END as ScoreChange,
    CASE
        WHEN LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) IS NOT NULL
        THEN LAG(ClassRank, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) - ClassRank
    END as ClassRankChange,
    CASE
        WHEN LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) IS NOT NULL
        THEN LAG(GradeRank, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) - GradeRank
    END as GradeRankChange,
    -- 趋势标记
    CASE
        WHEN LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) IS NOT NULL
        THEN
            CASE
                WHEN Score > LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) THEN '进步'
                WHEN Score < LAG(Score, 1) OVER (PARTITION BY StudentId, SubjectName ORDER BY ExamDate) THEN '退步'
                ELSE '持平'
            END
    END as Trend
FROM RankedScores
ORDER BY StudentName, SubjectName, ExamDate;

-- ============================================
-- 触发器(自动维护)
-- ============================================

-- 自动更新学生表时间戳
CREATE TRIGGER IF NOT EXISTS trg_students_update
AFTER UPDATE ON Students
BEGIN
    UPDATE Students SET UpdatedAt = datetime('now', 'localtime') WHERE StudentId = NEW.StudentId;
END;

-- 自动更新成绩表时间戳
CREATE TRIGGER IF NOT EXISTS trg_scores_update
AFTER UPDATE ON Scores
BEGIN
    UPDATE Scores SET UpdatedAt = datetime('now', 'localtime') WHERE ScoreId = NEW.ScoreId;
END;

-- ============================================
-- 初始化数据
-- ============================================

-- 初始化科目
INSERT INTO Subjects (SubjectName, SubjectCode, Category, MaxScore, SortOrder) VALUES
('语文', 'CHI', '通用', 150, 1),
('数学', 'MATH', '通用', 150, 2),
('英语', 'ENG', '通用', 150, 3),
('物理', 'PHY', '理科', 100, 4),
('化学', 'CHE', '理科', 100, 5),
('生物', 'BIO', '理科', 100, 6),
('政治', 'POL', '文科', 100, 7),
('历史', 'HIS', '文科', 100, 8),
('地理', 'GEO', '文科', 100, 9),
('总分', 'TOTAL', '总分', 0, 10);  -- SubjectId=10,总分作为特殊科目
