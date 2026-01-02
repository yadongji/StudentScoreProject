# 限时练成绩表设计说明

## 问题描述

原设计存在ExamId冲突问题：
- `Exams`表使用`ExamId`作为主键
- `TimeLimitExams`表也使用`ExamId`作为主键
- 两个表的`ExamId`会产生冲突（因为都是自增ID）

## 解决方案

创建独立的`TimeLimitScores`表，专门存储限时练成绩数据。

## 数据库表结构

### TimeLimitScores表

| 字段名 | 类型 | 说明 |
|--------|------|------|
| ScoreId | INTEGER | 主键ID（独立自增，不与Scores表冲突） |
| TimeLimitExamId | INTEGER | 关联TimeLimitExams表的ExamId |
| StudentId | INTEGER | 关联Students表的学生ID |
| SubjectId | INTEGER | 关联Subjects表的科目ID |
| Score | REAL | 成绩 |
| ClassRank | INTEGER | 班级排名 |
| GradeRank | INTEGER | 年级排名 |
| CreatedAt | TEXT | 创建时间 |
| UpdatedAt | TEXT | 更新时间 |

### 外键关系

- `TimeLimitExamId` → `TimeLimitExams(ExamId)`
- `StudentId` → `Students(StudentId)`
- `SubjectId` → `Subjects(SubjectId)`

### 唯一约束

`(TimeLimitExamId, StudentId, SubjectId)` - 确保同一学生在同一限时练考试的同一科目只有一条记录

## 使用步骤

### 1. 创建表

运行 `创建限时练成绩表.bat` 或执行：
```bash
python create_time_limit_scores_table.py
```

### 2. 导入数据

使用 `拖拽导入限时练成绩.bat` 导入Excel文件，数据将自动保存到`TimeLimitScores`表。

### 3. 查询数据

使用 `查询限时练成绩.bat` 查询成绩数据，系统自动从`TimeLimitScores`表读取。

## 优势

1. **完全隔离**：限时练数据与普通考试成绩完全分离
2. **无ID冲突**：使用独立的ScoreId，不会与Scores表冲突
3. **清晰结构**：专门的表结构，便于管理和查询
4. **易于扩展**：可以根据需要添加限时练特有的字段

## 数据迁移

如果之前已经导入了限时练数据到Scores表，需要：

1. 备份现有数据
2. 将Scores表中的限时练相关数据迁移到TimeLimitScores表
3. 删除Scores表中的限时练数据（可选）

迁移SQL示例：
```sql
INSERT INTO TimeLimitScores (TimeLimitExamId, StudentId, SubjectId, Score, ClassRank, GradeRank, CreatedAt, UpdatedAt)
SELECT ExamId, StudentId, SubjectId, Score, ClassRank, GradeRank, CreatedAt, UpdatedAt
FROM Scores
WHERE ExamId IN (SELECT ExamId FROM TimeLimitExams);
```
