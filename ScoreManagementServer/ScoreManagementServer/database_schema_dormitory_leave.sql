-- ============================================
-- 宿舍管理与请假管理系统数据库设计
-- 功能：宿舍查寝管理、学生请假管理、出勤统计
-- ============================================

-- ============================================
-- 1. 宿舍楼表
-- ============================================
CREATE TABLE Dormitories (
    DormitoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    DormitoryName TEXT NOT NULL UNIQUE,     -- 宿舍楼名称（如"5号楼"、"6号楼"）
    DormitoryCode TEXT NOT NULL UNIQUE,     -- 宿舍楼代码（如"D5"、"D6"）
    GenderType TEXT NOT NULL CHECK(GenderType IN ('男', '女')), -- 宿舍类型
    FloorCount INTEGER DEFAULT 6,           -- 楼层数
    Address TEXT,                          -- 地址
    IsActive INTEGER DEFAULT 1,             -- 是否启用
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime'))
);

-- ============================================
-- 2. 宿舍房间表
-- ============================================
CREATE TABLE DormitoryRooms (
    RoomId INTEGER PRIMARY KEY AUTOINCREMENT,
    DormitoryId INTEGER NOT NULL,          -- 所属宿舍楼ID
    FloorNumber INTEGER NOT NULL,           -- 楼层号（1-6）
    RoomNumber TEXT NOT NULL,               -- 房间号（如"101"、"201"）
    RoomName TEXT,                         -- 房间名称
    BedCount INTEGER DEFAULT 4,            -- 床位数
    OccupiedCount INTEGER DEFAULT 0,       -- 已入住人数
    RoomType TEXT CHECK(RoomType IN ('标准间', '套间', '其他')), -- 房间类型
    IsActive INTEGER DEFAULT 1,             -- 是否启用
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (DormitoryId) REFERENCES Dormitories(DormitoryId) ON DELETE CASCADE,
    UNIQUE(DormitoryId, FloorNumber, RoomNumber) -- 同一楼同一房间号唯一
);

-- ============================================
-- 3. 宿舍床位表
-- ============================================
CREATE TABLE DormitoryBeds (
    BedId INTEGER PRIMARY KEY AUTOINCREMENT,
    RoomId INTEGER NOT NULL,               -- 所属房间ID
    BedNumber TEXT NOT NULL,                -- 床位号（如"A"、"B"、"C"、"D"）
    BedPosition TEXT CHECK(BedPosition IN ('上铺', '下铺', '上铺2', '下铺2')), -- 床位位置
    IsOccupied INTEGER DEFAULT 0,           -- 是否已分配
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (RoomId) REFERENCES DormitoryRooms(RoomId) ON DELETE CASCADE,
    UNIQUE(RoomId, BedNumber) -- 同一房间内床位号唯一
);

-- ============================================
-- 4. 宿管人员表
-- ============================================
CREATE TABLE DormitoryStaff (
    StaffId INTEGER PRIMARY KEY AUTOINCREMENT,
    StaffName TEXT NOT NULL,               -- 宿管姓名
    StaffCode TEXT NOT NULL UNIQUE,        -- 工号
    DormitoryId INTEGER NOT NULL,          -- 负责的宿舍楼ID
    FloorNumber INTEGER,                   -- 负责的楼层（NULL表示负责整栋楼）
    PhoneNumber TEXT,                      -- 联系电话
    Gender TEXT CHECK(Gender IN ('男', '女')),
    Position TEXT,                         -- 职位（如"宿管阿姨"、"楼层管理员"）
    IsAdmin INTEGER DEFAULT 0,             -- 是否为管理员（可以查看整栋楼）
    IsActive INTEGER DEFAULT 1,             -- 是否在职
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (DormitoryId) REFERENCES Dormitories(DormitoryId) ON DELETE CASCADE
);

-- ============================================
-- 5. 学生宿舍分配表
-- ============================================
CREATE TABLE StudentDormitoryAssignments (
    AssignmentId INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL,            -- 学生ID（关联Students表）
    BedId INTEGER NOT NULL,                -- 分配的床位ID
    RoomId INTEGER NOT NULL,               -- 房间ID
    DormitoryId INTEGER NOT NULL,          -- 宿舍楼ID
    AssignDate TEXT NOT NULL,              -- 分配日期
    IsCurrent INTEGER DEFAULT 1,           -- 是否当前有效（历史记录保留）
    LeaveDate TEXT,                        -- 退宿日期
    Remarks TEXT,                          -- 备注
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE,
    FOREIGN KEY (BedId) REFERENCES DormitoryBeds(BedId) ON DELETE CASCADE,
    FOREIGN KEY (RoomId) REFERENCES DormitoryRooms(RoomId) ON DELETE CASCADE,
    FOREIGN KEY (DormitoryId) REFERENCES Dormitories(DormitoryId) ON DELETE CASCADE,
    UNIQUE(StudentId, IsCurrent) -- 每个学生只能有一个当前有效的分配
);

-- ============================================
-- 6. 每晚查寝记录表
-- ============================================
CREATE TABLE DormitoryCheckInRecords (
    CheckInId INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL,            -- 学生ID
    RoomId INTEGER NOT NULL,               -- 房间ID
    DormitoryId INTEGER NOT NULL,          -- 宿舍楼ID
    CheckInDate TEXT NOT NULL,             -- 查寝日期（YYYY-MM-DD）
    CheckInTime TEXT NOT NULL,             -- 查寝时间（HH:MM:SS）
    Status TEXT NOT NULL CHECK(Status IN ('在寝', '未归', '请假', '外出', '晚归')), -- 查寝状态
    StaffId INTEGER,                       -- 查寝人ID
    Remarks TEXT,                          -- 备注
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE,
    FOREIGN KEY (RoomId) REFERENCES DormitoryRooms(RoomId) ON DELETE CASCADE,
    FOREIGN KEY (DormitoryId) REFERENCES Dormitories(DormitoryId) ON DELETE CASCADE,
    FOREIGN KEY (StaffId) REFERENCES DormitoryStaff(StaffId) ON DELETE SET NULL,
    UNIQUE(StudentId, CheckInDate) -- 每个学生每天只能有一条查寝记录
);

-- ============================================
-- 7. 请假申请表
-- ============================================
CREATE TABLE LeaveApplications (
    LeaveId INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL,            -- 学生ID
    LeaveType TEXT NOT NULL CHECK(LeaveType IN ('病假', '事假', '其他')), -- 请假类型
    Reason TEXT NOT NULL,                  -- 请假原因
    StartTime TEXT NOT NULL,               -- 开始时间（YYYY-MM-DD HH:MM:SS）
    EndTime TEXT NOT NULL,                 -- 结束时间（YYYY-MM-DD HH:MM:SS）
    LeaveDays INTEGER NOT NULL,            -- 请假天数
    ApproverId INTEGER,                    -- 审批人ID（教师或班主任）
    ApproverName TEXT,                     -- 审批人姓名
    ApprovalStatus TEXT DEFAULT '待审批' CHECK(ApprovalStatus IN ('待审批', '已通过', '已拒绝', '已撤销')), -- 审批状态
    ApprovalTime TEXT,                      -- 审批时间
    ApprovalRemarks TEXT,                   -- 审批备注
    ParentContact TEXT,                    -- 家长联系方式
    ParentConfirm INTEGER DEFAULT 0,        -- 家长是否确认
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE
);

-- ============================================
-- 8. 班主任表（关联学生表）
-- ============================================
CREATE TABLE HeadTeachers (
    TeacherId INTEGER PRIMARY KEY AUTOINCREMENT,
    TeacherName TEXT NOT NULL,             -- 班主任姓名
    TeacherCode TEXT NOT NULL UNIQUE,      -- 工号
    ClassName TEXT NOT NULL UNIQUE,        -- 负责的班级
    PhoneNumber TEXT,                      -- 联系电话
    Email TEXT,                            -- 邮箱
    IsActive INTEGER DEFAULT 1,             -- 是否在职
    CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT DEFAULT (datetime('now', 'localtime'))
);

-- ============================================
-- 索引创建（优化查询性能）
-- ============================================

-- 宿舍楼表索引
CREATE INDEX idx_dormitories_gender ON Dormitories(GenderType);

-- 宿舍房间表索引
CREATE INDEX idx_dormitoryrooms_dormitory ON DormitoryRooms(DormitoryId);
CREATE INDEX idx_dormitoryrooms_floor ON DormitoryRooms(FloorNumber);

-- 宿舍床位表索引
CREATE INDEX idx_dormitorybeds_room ON DormitoryBeds(RoomId);

-- 宿管人员表索引
CREATE INDEX idx_dormitorystaff_dormitory ON DormitoryStaff(DormitoryId);
CREATE INDEX idx_dormitorystaff_floor ON DormitoryStaff(FloorNumber);

-- 学生宿舍分配表索引
CREATE INDEX idx_studentdormitoryassignments_student ON StudentDormitoryAssignments(StudentId);
CREATE INDEX idx_studentdormitoryassignments_dormitory ON StudentDormitoryAssignments(DormitoryId);
CREATE INDEX idx_studentdormitoryassignments_room ON StudentDormitoryAssignments(RoomId);
CREATE INDEX idx_studentdormitoryassignments_current ON StudentDormitoryAssignments(IsCurrent);

-- 查寝记录表索引
CREATE INDEX idx_dormitorycheckin_date ON DormitoryCheckInRecords(CheckInDate);
CREATE INDEX idx_dormitorycheckin_student ON DormitoryCheckInRecords(StudentId);
CREATE INDEX idx_dormitorycheckin_room ON DormitoryCheckInRecords(RoomId);
CREATE INDEX idx_dormitorycheckin_status ON DormitoryCheckInRecords(Status);
CREATE INDEX idx_dormitorycheckin_dormitory ON DormitoryCheckInRecords(DormitoryId);
CREATE INDEX idx_dormitorycheckin_dormitory_date ON DormitoryCheckInRecords(DormitoryId, CheckInDate);

-- 请假申请表索引
CREATE INDEX idx_leaveapplications_student ON LeaveApplications(StudentId);
CREATE INDEX idx_leaveapplications_status ON LeaveApplications(ApprovalStatus);
CREATE INDEX idx_leaveapplications_time ON LeaveApplications(StartTime, EndTime);
CREATE INDEX idx_leaveapplications_approver ON LeaveApplications(ApproverId);

-- 班主任表索引
CREATE INDEX idx_headteachers_class ON HeadTeachers(ClassName);

-- ============================================
-- 视图创建（简化查询）
-- ============================================

-- 视图1: 宿舍楼汇总信息
CREATE VIEW IF NOT EXISTS vw_DormitorySummary AS
SELECT
    d.DormitoryId,
    d.DormitoryName,
    d.DormitoryCode,
    d.GenderType,
    d.FloorCount,
    COUNT(DISTINCT r.RoomId) as TotalRooms,
    SUM(r.BedCount) as TotalBeds,
    SUM(r.OccupiedCount) as TotalOccupied,
    SUM(r.BedCount) - SUM(r.OccupiedCount) as AvailableBeds,
    ROUND((SUM(r.OccupiedCount) * 100.0) / SUM(r.BedCount), 2) as OccupancyRate,
    d.IsActive
FROM Dormitories d
LEFT JOIN DormitoryRooms r ON d.DormitoryId = r.DormitoryId
GROUP BY d.DormitoryId
ORDER BY d.DormitoryCode;

-- 视图2: 楼层查寝情况统计（宿管阿姨查看）
CREATE VIEW IF NOT EXISTS vw_FloorCheckInSummary AS
SELECT
    ds.DormitoryId,
    ds.FloorNumber,
    ds.StaffId,
    ds.StaffName,
    r.FloorNumber as RoomFloor,
    COUNT(DISTINCT r.RoomId) as TotalRooms,
    COUNT(DISTINCT ci.StudentId) as TotalStudents,
    SUM(CASE WHEN ci.Status = '在寝' THEN 1 ELSE 0 END) as PresentCount,
    SUM(CASE WHEN ci.Status = '未归' THEN 1 ELSE 0 END) as AbsentCount,
    SUM(CASE WHEN ci.Status = '请假' THEN 1 ELSE 0 END) as LeaveCount,
    SUM(CASE WHEN ci.Status = '外出' THEN 1 ELSE 0 END) as OutCount,
    SUM(CASE WHEN ci.Status = '晚归' THEN 1 ELSE 0 END) as LateCount,
    ci.CheckInDate
FROM DormitoryStaff ds
LEFT JOIN DormitoryRooms r ON ds.DormitoryId = r.DormitoryId AND (ds.FloorNumber IS NULL OR r.FloorNumber = ds.FloorNumber)
LEFT JOIN DormitoryCheckInRecords ci ON r.RoomId = ci.RoomId
GROUP BY ds.DormitoryId, ds.FloorNumber, ci.CheckInDate
ORDER BY ds.DormitoryId, ds.FloorNumber, ci.CheckInDate DESC;

-- 视图3: 学生宿舍详细信息
CREATE VIEW IF NOT EXISTS vw_StudentDormitoryDetail AS
SELECT
    sda.AssignmentId,
    s.StudentId,
    s.StudentNumber,
    s.StudentName,
    s.ClassName,
    s.Gender,
    d.DormitoryName,
    d.DormitoryCode,
    r.FloorNumber,
    r.RoomNumber,
    r.RoomName,
    b.BedNumber,
    b.BedPosition,
    sda.AssignDate,
    sda.IsCurrent,
    ds.StaffName as FloorManager,
    CASE
        WHEN sda.LeaveDate IS NOT NULL THEN '已退宿'
        ELSE '在住'
    END as Status
FROM StudentDormitoryAssignments sda
JOIN Students s ON sda.StudentId = s.StudentId
JOIN Dormitories d ON sda.DormitoryId = d.DormitoryId
JOIN DormitoryRooms r ON sda.RoomId = r.RoomId
JOIN DormitoryBeds b ON sda.BedId = b.BedId
LEFT JOIN DormitoryStaff ds ON d.DormitoryId = ds.DormitoryId AND (ds.FloorNumber IS NULL OR ds.FloorNumber = r.FloorNumber)
WHERE sda.IsCurrent = 1
ORDER BY d.DormitoryCode, r.FloorNumber, r.RoomNumber, b.BedNumber;

-- 视图4: 每日查寝详细情况
CREATE VIEW IF NOT EXISTS vw_DailyCheckInDetail AS
SELECT
    ci.CheckInId,
    ci.CheckInDate,
    ci.CheckInTime,
    ci.Status,
    ci.Remarks,
    s.StudentId,
    s.StudentNumber,
    s.StudentName,
    s.ClassName,
    d.DormitoryName,
    d.DormitoryCode,
    r.FloorNumber,
    r.RoomNumber,
    r.RoomName,
    b.BedNumber,
    b.BedPosition,
    ds.StaffName as CheckerName,
    CASE
        WHEN la.LeaveId IS NOT NULL AND ci.Status = '请假' THEN '请假不在校'
        WHEN ci.Status = '在寝' THEN '正常'
        WHEN ci.Status = '未归' THEN '未归'
        WHEN ci.Status = '外出' THEN '外出'
        WHEN ci.Status = '晚归' THEN '晚归'
        ELSE '未知'
    END as StatusDescription
FROM DormitoryCheckInRecords ci
JOIN Students s ON ci.StudentId = s.StudentId
JOIN DormitoryRooms r ON ci.RoomId = r.RoomId
JOIN Dormitories d ON ci.DormitoryId = d.DormitoryId
JOIN DormitoryBeds b ON r.RoomId = b.RoomId
LEFT JOIN DormitoryStaff ds ON ci.StaffId = ds.StaffId
LEFT JOIN LeaveApplications la ON s.StudentId = la.StudentId
    AND ci.CheckInDate >= date(la.StartTime)
    AND ci.CheckInDate <= date(la.EndTime)
    AND la.ApprovalStatus = '已通过'
ORDER BY ci.CheckInDate DESC, d.DormitoryCode, r.FloorNumber, r.RoomNumber;

-- 视图5: 班级出勤情况（班主任查看）
CREATE VIEW IF NOT EXISTS vw_ClassAttendanceDaily AS
SELECT
    ht.ClassName,
    ht.TeacherName as HeadTeacher,
    s.StudentId,
    s.StudentNumber,
    s.StudentName,
    date('now', 'localtime') as Today,
    COUNT(DISTINCT ci.CheckInId) as CheckInCount,
    MAX(ci.Status) as CheckInStatus,
    CASE
        WHEN la.LeaveId IS NOT NULL 
            AND date('now', 'localtime') >= date(la.StartTime) 
            AND date('now', 'localtime') <= date(la.EndTime)
            AND la.ApprovalStatus = '已通过'
        THEN '请假不在校'
        WHEN ci.Status = '在寝' THEN '在校'
        WHEN ci.Status IS NULL THEN '未查寝'
        ELSE ci.Status
    END as TodayStatus,
    la.LeaveType,
    la.StartTime as LeaveStartTime,
    la.EndTime as LeaveEndTime
FROM Students s
LEFT JOIN HeadTeachers ht ON s.ClassName = ht.ClassName
LEFT JOIN DormitoryCheckInRecords ci ON s.StudentId = ci.StudentId 
    AND ci.CheckInDate = date('now', 'localtime')
LEFT JOIN LeaveApplications la ON s.StudentId = la.StudentId
    AND date('now', 'localtime') >= date(la.StartTime) 
    AND date('now', 'localtime') <= date(la.EndTime)
    AND la.ApprovalStatus = '已通过'
WHERE s.ClassName IS NOT NULL
GROUP BY s.StudentId
ORDER BY ht.ClassName, s.StudentNumber;

-- 视图6: 请假申请详细信息
CREATE VIEW IF NOT EXISTS vw_LeaveApplicationDetail AS
SELECT
    la.LeaveId,
    la.StudentId,
    s.StudentNumber,
    s.StudentName,
    s.ClassName,
    ht.TeacherName as HeadTeacher,
    la.LeaveType,
    la.Reason,
    la.StartTime,
    la.EndTime,
    la.LeaveDays,
    la.ApproverId,
    la.ApproverName,
    la.ApprovalStatus,
    la.ApprovalTime,
    la.ApprovalRemarks,
    la.ParentContact,
    la.ParentConfirm,
    CASE
        WHEN la.ApprovalStatus = '已通过' 
            AND date('now', 'localtime') >= date(la.StartTime) 
            AND date('now', 'localtime') <= date(la.EndTime)
        THEN 1
        ELSE 0
    END as IsCurrentlyOnLeave,
    CASE
        WHEN la.ApprovalStatus = '已通过' 
            AND date('now', 'localtime') >= date(la.StartTime) 
            AND date('now', 'localtime') <= date(la.EndTime)
        THEN '请假不在校'
        ELSE '在校'
    END as TodayStatus,
    la.CreatedAt
FROM LeaveApplications la
JOIN Students s ON la.StudentId = s.StudentId
LEFT JOIN HeadTeachers ht ON s.ClassName = ht.ClassName
ORDER BY la.CreatedAt DESC;

-- 视图7: 宿管阿姨查看的房间入住情况
CREATE VIEW IF NOT EXISTS vw_RoomOccupancyByStaff AS
SELECT
    ds.StaffId,
    ds.StaffName,
    ds.DormitoryId,
    d.DormitoryName,
    ds.FloorNumber,
    r.RoomId,
    r.RoomNumber,
    r.RoomName,
    r.BedCount,
    COUNT(DISTINCT sda.StudentId) as OccupiedCount,
    r.BedCount - COUNT(DISTINCT sda.StudentId) as AvailableCount,
    GROUP_CONCAT(s.StudentName, ', ') as StudentNames
FROM DormitoryStaff ds
JOIN Dormitories d ON ds.DormitoryId = d.DormitoryId
LEFT JOIN DormitoryRooms r ON d.DormitoryId = r.DormitoryId AND (ds.FloorNumber IS NULL OR r.FloorNumber = ds.FloorNumber)
LEFT JOIN StudentDormitoryAssignments sda ON r.RoomId = sda.RoomId AND sda.IsCurrent = 1
LEFT JOIN Students s ON sda.StudentId = s.StudentId
GROUP BY ds.StaffId, r.RoomId
ORDER BY ds.DormitoryId, ds.FloorNumber, r.RoomNumber;

-- ============================================
-- 触发器（自动维护）
-- ============================================

-- 更新房间已入住人数
CREATE TRIGGER IF NOT EXISTS trg_update_room_occupancy_after_assignment
AFTER INSERT ON StudentDormitoryAssignments
WHEN NEW.IsCurrent = 1
BEGIN
    UPDATE DormitoryRooms SET OccupiedCount = (
        SELECT COUNT(*) FROM StudentDormitoryAssignments 
        WHERE RoomId = NEW.RoomId AND IsCurrent = 1
    ) WHERE RoomId = NEW.RoomId;
    UPDATE DormitoryBeds SET IsOccupied = 1 WHERE BedId = NEW.BedId;
END;

-- 触发器1: 当分配保持有效时，更新床位状态
CREATE TRIGGER IF NOT EXISTS trg_update_room_occupancy_after_update_current
AFTER UPDATE ON StudentDormitoryAssignments
WHEN NEW.IsCurrent = 1 AND OLD.IsCurrent = 1
BEGIN
    UPDATE DormitoryRooms SET OccupiedCount = (
        SELECT COUNT(*) FROM StudentDormitoryAssignments
        WHERE RoomId = NEW.RoomId AND IsCurrent = 1
    ) WHERE RoomId = NEW.RoomId;
    UPDATE DormitoryBeds SET IsOccupied = 1 WHERE BedId = NEW.BedId;
END;

-- 触发器2: 当分配失效时，更新床位状态
CREATE TRIGGER IF NOT EXISTS trg_update_room_occupancy_after_update_inactive
AFTER UPDATE ON StudentDormitoryAssignments
WHEN NEW.IsCurrent = 0 AND OLD.IsCurrent = 1
BEGIN
    UPDATE DormitoryRooms SET OccupiedCount = (
        SELECT COUNT(*) FROM StudentDormitoryAssignments
        WHERE RoomId = OLD.RoomId AND IsCurrent = 1
    ) WHERE RoomId = OLD.RoomId;
    UPDATE DormitoryBeds SET IsOccupied = 0 WHERE BedId = NEW.BedId;
END;

-- 触发器3: 当其他情况时，仅更新房间人数
CREATE TRIGGER IF NOT EXISTS trg_update_room_occupancy_after_update_other
AFTER UPDATE ON StudentDormitoryAssignments
WHEN NOT ((NEW.IsCurrent = 1 AND OLD.IsCurrent = 1) OR (NEW.IsCurrent = 0 AND OLD.IsCurrent = 1))
BEGIN
    UPDATE DormitoryRooms SET OccupiedCount = (
        SELECT COUNT(*) FROM StudentDormitoryAssignments
        WHERE RoomId = NEW.RoomId AND IsCurrent = 1
    ) WHERE RoomId = NEW.RoomId;
END;

CREATE TRIGGER IF NOT EXISTS trg_update_room_occupancy_after_delete
AFTER DELETE ON StudentDormitoryAssignments
BEGIN
    UPDATE DormitoryRooms SET OccupiedCount = (
        SELECT COUNT(*) FROM StudentDormitoryAssignments 
        WHERE RoomId = OLD.RoomId AND IsCurrent = 1
    ) WHERE RoomId = OLD.RoomId;
    UPDATE DormitoryBeds SET IsOccupied = 0 WHERE BedId = OLD.BedId;
END;

-- 自动更新时间戳
CREATE TRIGGER IF NOT EXISTS trg_dormitories_update
AFTER UPDATE ON Dormitories
BEGIN
    UPDATE Dormitories SET UpdatedAt = datetime('now', 'localtime') WHERE DormitoryId = NEW.DormitoryId;
END;

CREATE TRIGGER IF NOT EXISTS trg_dormitoryrooms_update
AFTER UPDATE ON DormitoryRooms
BEGIN
    UPDATE DormitoryRooms SET UpdatedAt = datetime('now', 'localtime') WHERE RoomId = NEW.RoomId;
END;

CREATE TRIGGER IF NOT EXISTS trg_leaveapplications_update
AFTER UPDATE ON LeaveApplications
BEGIN
    UPDATE LeaveApplications SET UpdatedAt = datetime('now', 'localtime') WHERE LeaveId = NEW.LeaveId;
END;

CREATE TRIGGER IF NOT EXISTS trg_studentdormitoryassignments_update
AFTER UPDATE ON StudentDormitoryAssignments
BEGIN
    UPDATE StudentDormitoryAssignments SET UpdatedAt = datetime('now', 'localtime') WHERE AssignmentId = NEW.AssignmentId;
END;

-- ============================================
-- 初始化示例数据
-- ============================================

-- 初始化宿舍楼（5号楼、6号楼男生宿舍）
INSERT INTO Dormitories (DormitoryName, DormitoryCode, GenderType, FloorCount, Address) VALUES
('5号楼', 'D5', '男', 6, '校区东区5号楼'),
('6号楼', 'D6', '男', 6, '校区东区6号楼');

-- 初始化宿管人员
INSERT INTO DormitoryStaff (StaffName, StaffCode, DormitoryId, FloorNumber, PhoneNumber, Gender, Position, IsAdmin) VALUES
('王阿姨', 'D001', 1, NULL, '13800138001', '女', '宿管阿姨', 1),
('李阿姨', 'D002', 2, NULL, '13800138002', '女', '宿管阿姨', 1),
('张管理员', 'D003', 1, 1, '13800138003', '女', '楼层管理员', 0),
('赵管理员', 'D004', 1, 2, '13800138004', '女', '楼层管理员', 0),
('孙管理员', 'D005', 2, 1, '13800138005', '女', '楼层管理员', 0);

-- 初始化班主任（示例）
INSERT INTO HeadTeachers (TeacherName, TeacherCode, ClassName, PhoneNumber, Email) VALUES
('张老师', 'T001', '高一(1)班', '13900139001', 'zhang@school.com'),
('李老师', 'T002', '高一(2)班', '13900139002', 'li@school.com'),
('王老师', 'T003', '高二(1)班', '13900139003', 'wang@school.com'),
('赵老师', 'T004', '高二(2)班', '13900139004', 'zhao@school.com');

-- 注：学生数据请从现有的Students表中导入
-- 房间和床位数据请根据实际情况批量插入
-- 查寝记录和请假记录请在日常使用中通过应用程序添加
