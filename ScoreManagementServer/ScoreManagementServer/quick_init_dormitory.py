"""
快速初始化宿舍管理数据
用于演示和测试
"""

import sqlite3
from datetime import datetime, timedelta

def quick_init():
    """快速初始化示例数据"""
    
    db_path = "StudentData.db"
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    print("="*60)
    print("宿舍管理系统 - 快速初始化")
    print("="*60)
    
    # 1. 检查表是否存在
    cursor.execute("""
        SELECT name FROM sqlite_master 
        WHERE type='table' AND name='Dormitories'
    """)
    
    if not cursor.fetchone():
        print("\n⚠️  数据库表未初始化，请先运行系统初始化！")
        print("运行步骤：")
        print("1. 双击 宿舍管理系统.bat")
        print("2. 选择 1. 初始化数据库表")
        return
    
    # 2. 添加宿舍楼
    print("\n📝 正在添加宿舍楼...")
    dormitories = [
        ("5号楼", "D5", "男", 6, "校区东区5号楼"),
        ("6号楼", "D6", "男", 6, "校区东区6号楼"),
    ]
    
    for name, code, gender, floors, addr in dormitories:
        try:
            cursor.execute("""
                INSERT INTO Dormitories (DormitoryName, DormitoryCode, GenderType, FloorCount, Address)
                VALUES (?, ?, ?, ?, ?)
            """, (name, code, gender, floors, addr))
            print(f"  ✅ {name} ({code})")
        except sqlite3.IntegrityError:
            print(f"  ⏭️  {name} ({code}) 已存在，跳过")
    
    # 3. 添加宿管人员
    print("\n👥 正在添加宿管人员...")
    staff_list = [
        ("王阿姨", "D001", 1, None, "13800138001", "女", "宿管阿姨", 1),  # 5号楼整栋
        ("李阿姨", "D002", 2, None, "13800138002", "女", "宿管阿姨", 1),  # 6号楼整栋
        ("张管理员", "D003", 1, 1, "13800138003", "女", "楼层管理员", 0),  # 5号楼1楼
        ("赵管理员", "D004", 1, 2, "13800138004", "女", "楼层管理员", 0),  # 5号楼2楼
        ("孙管理员", "D005", 2, 1, "13800138005", "女", "楼层管理员", 0),  # 6号楼1楼
    ]
    
    for name, code, dorm_id, floor, phone, gender, position, is_admin in staff_list:
        try:
            cursor.execute("""
                INSERT INTO DormitoryStaff (StaffName, StaffCode, DormitoryId, FloorNumber,
                                             PhoneNumber, Gender, Position, IsAdmin)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """, (name, code, dorm_id, floor, phone, gender, position, is_admin))
            print(f"  ✅ {name} ({position})")
        except sqlite3.IntegrityError:
            print(f"  ⏭️  {name} 已存在，跳过")
    
    # 4. 添加房间和床位
    print("\n🏠 正在添加房间...")
    
    # 5号楼 - 每层10个房间
    for floor in range(1, 7):
        for room in range(1, 11):
            room_number = f"{floor}{room:02d}"
            room_name = f"{floor}楼{room_number}室"
            
            try:
                # 添加房间
                cursor.execute("""
                    INSERT INTO DormitoryRooms (DormitoryId, FloorNumber, RoomNumber, 
                                                   BedCount, RoomType, RoomName)
                    VALUES (?, ?, ?, ?, ?, ?)
                """, (1, floor, room_number, 4, "标准间", room_name))
                room_id = cursor.lastrowid
                
                # 添加床位
                bed_positions = ['上铺', '下铺', '上铺', '下铺']
                bed_codes = ['A', 'B', 'C', 'D']
                
                for i, (code, pos) in enumerate(zip(bed_codes, bed_positions)):
                    cursor.execute("""
                        INSERT INTO DormitoryBeds (RoomId, BedNumber, BedPosition)
                        VALUES (?, ?, ?)
                    """, (room_id, code, pos))
                
                if room <= 2:
                    print(f"  ✅ 5号楼 {floor}楼 {room_number}室")
            except sqlite3.IntegrityError:
                pass  # 房间已存在
    
    # 6号楼 - 每层10个房间
    for floor in range(1, 7):
        for room in range(1, 11):
            room_number = f"{floor}{room:02d}"
            room_name = f"{floor}楼{room_number}室"
            
            try:
                # 添加房间
                cursor.execute("""
                    INSERT INTO DormitoryRooms (DormitoryId, FloorNumber, RoomNumber, 
                                                   BedCount, RoomType, RoomName)
                    VALUES (?, ?, ?, ?, ?, ?)
                """, (2, floor, room_number, 4, "标准间", room_name))
                room_id = cursor.lastrowid
                
                # 添加床位
                bed_positions = ['上铺', '下铺', '上铺', '下铺']
                bed_codes = ['A', 'B', 'C', 'D']
                
                for i, (code, pos) in enumerate(zip(bed_codes, bed_positions)):
                    cursor.execute("""
                        INSERT INTO DormitoryBeds (RoomId, BedNumber, BedPosition)
                        VALUES (?, ?, ?)
                    """, (room_id, code, pos))
                
                if room <= 2:
                    print(f"  ✅ 6号楼 {floor}楼 {room_number}室")
            except sqlite3.IntegrityError:
                pass  # 房间已存在
    
    # 5. 添加班主任
    print("\n👨‍🏫 正在添加班主任...")
    head_teachers = [
        ("张老师", "T001", "高一(1)班", "13900139001", "zhang@school.com"),
        ("李老师", "T002", "高一(2)班", "13900139002", "li@school.com"),
        ("王老师", "T003", "高二(1)班", "13900139003", "wang@school.com"),
        ("赵老师", "T004", "高二(2)班", "13900139004", "zhao@school.com"),
    ]
    
    for name, code, class_name, phone, email in head_teachers:
        try:
            cursor.execute("""
                INSERT INTO HeadTeachers (TeacherName, TeacherCode, ClassName, PhoneNumber, Email)
                VALUES (?, ?, ?, ?, ?)
            """, (name, code, class_name, phone, email))
            print(f"  ✅ {name} - {class_name}")
        except sqlite3.IntegrityError:
            print(f"  ⏭️  {name} 已存在，跳过")
    
    conn.commit()
    
    # 6. 显示统计信息
    print("\n" + "="*60)
    print("📊 初始化完成统计")
    print("="*60)
    
    # 宿舍楼统计
    cursor.execute("SELECT COUNT(*) FROM Dormitories")
    dorm_count = cursor.fetchone()[0]
    print(f"\n🏢 宿舍楼: {dorm_count} 栋")
    
    cursor.execute("""
        SELECT DormitoryName, SUM(TotalRooms) as Rooms, SUM(TotalBeds) as Beds
        FROM vw_DormitorySummary
        GROUP BY DormitoryName
    """)
    for row in cursor.fetchall():
        print(f"   {row[0]}: {row[1]} 个房间, {row[2]} 个床位")
    
    # 宿管人员统计
    cursor.execute("SELECT COUNT(*) FROM DormitoryStaff")
    staff_count = cursor.fetchone()[0]
    print(f"\n👥 宿管人员: {staff_count} 人")
    
    cursor.execute("""
        SELECT DormitoryName, COUNT(*) as Count
        FROM DormitoryStaff ds
        JOIN Dormitories d ON ds.DormitoryId = d.DormitoryId
        GROUP BY DormitoryName
    """)
    for row in cursor.fetchall():
        print(f"   {row[0]}: {row[1]} 人")
    
    # 班主任统计
    cursor.execute("SELECT COUNT(*) FROM HeadTeachers")
    teacher_count = cursor.fetchone()[0]
    print(f"\n👨‍🏫 班主任: {teacher_count} 人")
    
    conn.close()
    
    print("\n" + "="*60)
    print("✅ 快速初始化完成！")
    print("="*60)
    print("\n下一步操作：")
    print("1. 运行 宿舍管理系统.bat")
    print("2. 使用系统功能进行查寝和请假管理")
    print("3. 参考 宿舍请假管理系统使用说明.md 获取详细帮助")
    print()


if __name__ == "__main__":
    quick_init()
