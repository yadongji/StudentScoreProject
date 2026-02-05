"""
宿舍管理和请假管理系统
功能：初始化数据库、查询宿舍情况、管理查寝记录、请假申请
"""

import sqlite3
from datetime import datetime, timedelta
import json

class DormitoryLeaveSystem:
    def __init__(self, db_path):
        """初始化数据库连接"""
        self.db_path = db_path
        self.conn = None
        
    def connect(self):
        """连接数据库"""
        self.conn = sqlite3.connect(self.db_path)
        self.conn.row_factory = sqlite3.Row  # 返回字典格式
        
    def disconnect(self):
        """断开数据库连接"""
        if self.conn:
            self.conn.close()
            
    def execute_query(self, query, params=None):
        """执行SQL查询"""
        if not self.conn:
            self.connect()
        cursor = self.conn.cursor()
        if params:
            cursor.execute(query, params)
        else:
            cursor.execute(query)
        return cursor
    
    def execute_script(self, script):
        """执行SQL脚本"""
        if not self.conn:
            self.connect()
        cursor = self.conn.cursor()
        cursor.executescript(script)
        self.conn.commit()
        
    def initialize_tables(self, schema_file='database_schema_dormitory_leave.sql'):
        """初始化数据库表"""
        try:
            with open(schema_file, 'r', encoding='utf-8') as f:
                schema = f.read()
            self.execute_script(schema)
            print("✅ 数据库表初始化成功！")
            print("\n📊 初始化完成:")
            cursor = self.execute_query("""
                SELECT COUNT(*) as count FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'
            """)
            table_count = cursor.fetchone()[0]
            cursor = self.execute_query("SELECT COUNT(*) as count FROM sqlite_master WHERE type='view'")
            view_count = cursor.fetchone()[0]
            cursor = self.execute_query("SELECT COUNT(*) as count FROM sqlite_master WHERE type='trigger'")
            trigger_count = cursor.fetchone()[0]
            print(f"   - 表: {table_count} 个")
            print(f"   - 视图: {view_count} 个")
            print(f"   - 触发器: {trigger_count} 个")
            print("\n💡 下一步：运行 '快速初始化宿舍数据.bat' 添加示例数据")
            return True
        except FileNotFoundError:
            print(f"❌ 错误: 找不到文件 '{schema_file}'")
            print("💡 提示: 请确保在正确的目录下运行程序")
            return False
        except Exception as e:
            print(f"❌ 初始化失败: {e}")
            import traceback
            traceback.print_exc()
            return False
            
    # ==================== 宿舍楼管理 ====================
    def add_dormitory(self, name, code, gender_type, floor_count=6, address=''):
        """添加宿舍楼"""
        query = """
        INSERT INTO Dormitories (DormitoryName, DormitoryCode, GenderType, FloorCount, Address)
        VALUES (?, ?, ?, ?, ?)
        """
        try:
            self.execute_query(query, (name, code, gender_type, floor_count, address))
            self.conn.commit()
            print(f"✅ 宿舍楼 {name} 添加成功！")
            return True
        except Exception as e:
            print(f"❌ 添加失败: {e}")
            return False
            
    def list_dormitories(self):
        """列出所有宿舍楼"""
        query = """
        SELECT * FROM vw_DormitorySummary
        ORDER BY DormitoryCode
        """
        cursor = self.execute_query(query)
        rows = cursor.fetchall()
        return rows
        
    # ==================== 宿管人员管理 ====================
    def add_staff(self, name, code, dormitory_id, floor_number=None, 
                  phone='', gender='女', position='宿管阿姨', is_admin=1):
        """添加宿管人员"""
        query = """
        INSERT INTO DormitoryStaff (StaffName, StaffCode, DormitoryId, FloorNumber,
                                     PhoneNumber, Gender, Position, IsAdmin)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """
        try:
            self.execute_query(query, (name, code, dormitory_id, floor_number,
                                       phone, gender, position, is_admin))
            self.conn.commit()
            print(f"✅ 宿管 {name} 添加成功！")
            return True
        except Exception as e:
            print(f"❌ 添加失败: {e}")
            return False
            
    def list_staff(self, dormitory_id=None):
        """列出宿管人员"""
        if dormitory_id:
            query = "SELECT * FROM DormitoryStaff WHERE DormitoryId = ? ORDER BY FloorNumber, StaffName"
            cursor = self.execute_query(query, (dormitory_id,))
        else:
            query = "SELECT * FROM DormitoryStaff ORDER BY DormitoryId, FloorNumber, StaffName"
            cursor = self.execute_query(query)
        return cursor.fetchall()
        
    # ==================== 房间和床位管理 ====================
    def add_room(self, dormitory_id, floor_number, room_number, 
                  bed_count=4, room_type='标准间', room_name=''):
        """添加宿舍房间"""
        query = """
        INSERT INTO DormitoryRooms (DormitoryId, FloorNumber, RoomNumber, 
                                     BedCount, RoomType, RoomName)
        VALUES (?, ?, ?, ?, ?, ?)
        """
        try:
            self.execute_query(query, (dormitory_id, floor_number, room_number,
                                       bed_count, room_type, room_name))
            room_id = self.execute_query("SELECT last_insert_rowid()").fetchone()[0]
            
            # 自动添加床位
            bed_positions = ['上铺', '下铺'] * (bed_count // 2)
            if bed_count % 2 == 1:
                bed_positions.append('上铺')
                
            for i, position in enumerate(bed_positions):
                bed_code = chr(65 + i)  # A, B, C, D...
                bed_query = """
                INSERT INTO DormitoryBeds (RoomId, BedNumber, BedPosition)
                VALUES (?, ?, ?)
                """
                self.execute_query(bed_query, (room_id, bed_code, position))
                
            self.conn.commit()
            print(f"✅ 房间 {room_number} 添加成功，包含 {bed_count} 个床位！")
            return True
        except Exception as e:
            print(f"❌ 添加失败: {e}")
            return False
            
    def list_rooms(self, dormitory_id=None, floor_number=None):
        """列出宿舍房间"""
        if dormitory_id and floor_number:
            query = """
            SELECT * FROM DormitoryRooms 
            WHERE DormitoryId = ? AND FloorNumber = ?
            ORDER BY RoomNumber
            """
            cursor = self.execute_query(query, (dormitory_id, floor_number))
        elif dormitory_id:
            query = """
            SELECT * FROM DormitoryRooms 
            WHERE DormitoryId = ?
            ORDER BY FloorNumber, RoomNumber
            """
            cursor = self.execute_query(query, (dormitory_id,))
        else:
            query = "SELECT * FROM DormitoryRooms ORDER BY DormitoryId, FloorNumber, RoomNumber"
            cursor = self.execute_query(query)
        return cursor.fetchall()
        
    # ==================== 学生宿舍分配 ====================
    def assign_student_to_dormitory(self, student_id, bed_id, room_id, dormitory_id):
        """分配学生到宿舍"""
        # 先取消该学生之前的分配
        update_query = """
        UPDATE StudentDormitoryAssignments 
        SET IsCurrent = 0, LeaveDate = datetime('now', 'localtime')
        WHERE StudentId = ? AND IsCurrent = 1
        """
        self.execute_query(update_query, (student_id,))
        
        # 创建新分配
        query = """
        INSERT INTO StudentDormitoryAssignments 
        (StudentId, BedId, RoomId, DormitoryId, AssignDate, IsCurrent)
        VALUES (?, ?, ?, ?, datetime('now', 'localtime'), 1)
        """
        try:
            self.execute_query(query, (student_id, bed_id, room_id, dormitory_id))
            self.conn.commit()
            print(f"✅ 学生分配成功！")
            return True
        except Exception as e:
            print(f"❌ 分配失败: {e}")
            return False
            
    def get_student_dormitory_info(self, student_id):
        """获取学生宿舍信息"""
        query = "SELECT * FROM vw_StudentDormitoryDetail WHERE StudentId = ?"
        cursor = self.execute_query(query, (student_id,))
        return cursor.fetchone()
        
    # ==================== 查寝记录管理 ====================
    def record_check_in(self, student_id, room_id, dormitory_id, status, 
                        staff_id=None, remarks=''):
        """记录查寝"""
        check_in_date = datetime.now().strftime('%Y-%m-%d')
        check_in_time = datetime.now().strftime('%H:%M:%S')
        
        query = """
        INSERT OR REPLACE INTO DormitoryCheckInRecords 
        (StudentId, RoomId, DormitoryId, CheckInDate, CheckInTime, Status, StaffId, Remarks)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """
        try:
            self.execute_query(query, (student_id, room_id, dormitory_id, 
                                       check_in_date, check_in_time, status, staff_id, remarks))
            self.conn.commit()
            return True
        except Exception as e:
            print(f"❌ 记录失败: {e}")
            return False
            
    def get_daily_check_in_summary(self, dormitory_id, floor_number=None, date=None):
        """获取每日查寝汇总（宿管阿姨查看）"""
        if not date:
            date = datetime.now().strftime('%Y-%m-%d')
            
        if floor_number:
            query = """
            SELECT * FROM vw_FloorCheckInSummary
            WHERE DormitoryId = ? AND FloorNumber = ? AND CheckInDate = ?
            """
            cursor = self.execute_query(query, (dormitory_id, floor_number, date))
        else:
            query = """
            SELECT * FROM vw_FloorCheckInSummary
            WHERE DormitoryId = ? AND CheckInDate = ?
            """
            cursor = self.execute_query(query, (dormitory_id, date))
        return cursor.fetchall()
        
    def get_room_check_in_detail(self, room_id, date=None):
        """获取房间查寝详情"""
        if not date:
            date = datetime.now().strftime('%Y-%m-%d')
            
        query = """
        SELECT * FROM vw_DailyCheckInDetail
        WHERE RoomId = ? AND CheckInDate = ?
        ORDER BY BedNumber
        """
        cursor = self.execute_query(query, (room_id, date))
        return cursor.fetchall()
        
    def batch_record_check_in_by_room(self, room_id, dormitory_id, staff_id, 
                                      status_map, date=None):
        """按房间批量记录查寝
        
        Args:
            room_id: 房间ID
            dormitory_id: 宿舍楼ID
            staff_id: 宿管ID
            status_map: 字典 {学生ID: 状态}
            date: 日期 (YYYY-MM-DD)，默认今天
        """
        if not date:
            date = datetime.now().strftime('%Y-%m-%d')
            
        success_count = 0
        for student_id, status in status_map.items():
            if self.record_check_in(student_id, room_id, dormitory_id, 
                                     status, staff_id):
                success_count += 1
                
        print(f"✅ 成功记录 {success_count}/{len(status_map)} 个学生的查寝情况！")
        return success_count
        
    # ==================== 请假管理 ====================
    def apply_leave(self, student_id, leave_type, reason, start_time, end_time, 
                    parent_contact='', parent_confirm=0):
        """学生申请请假"""
        # 计算请假天数
        start_dt = datetime.strptime(start_time, '%Y-%m-%d %H:%M:%S')
        end_dt = datetime.strptime(end_time, '%Y-%m-%d %H:%M:%S')
        leave_days = (end_dt - start_dt).days + 1
        
        query = """
        INSERT INTO LeaveApplications 
        (StudentId, LeaveType, Reason, StartTime, EndTime, LeaveDays,
         ParentContact, ParentConfirm, ApprovalStatus)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, '待审批')
        """
        try:
            self.execute_query(query, (student_id, leave_type, reason, start_time,
                                       end_time, leave_days, parent_contact, parent_confirm))
            self.conn.commit()
            print(f"✅ 请假申请提交成功！")
            return True
        except Exception as e:
            print(f"❌ 申请失败: {e}")
            return False
            
    def approve_leave(self, leave_id, approver_id, approver_name, approval_status, remarks=''):
        """审批请假申请"""
        query = """
        UPDATE LeaveApplications 
        SET ApproverId = ?, ApproverName = ?, ApprovalStatus = ?,
            ApprovalTime = datetime('now', 'localtime'), ApprovalRemarks = ?
        WHERE LeaveId = ?
        """
        try:
            self.execute_query(query, (approver_id, approver_name, approval_status,
                                       remarks, leave_id))
            self.conn.commit()
            print(f"✅ 请假审批完成！")
            return True
        except Exception as e:
            print(f"❌ 审批失败: {e}")
            return False
            
    def get_student_leaves(self, student_id):
        """获取学生请假记录"""
        query = "SELECT * FROM vw_LeaveApplicationDetail WHERE StudentId = ?"
        cursor = self.execute_query(query, (student_id,))
        return cursor.fetchall()
        
    def get_class_attendance(self, class_name, date=None):
        """获取班级出勤情况（班主任查看）"""
        query = """
        SELECT * FROM vw_ClassAttendanceDaily
        WHERE ClassName = ?
        """
        cursor = self.execute_query(query, (class_name,))
        return cursor.fetchall()
        
    def get_leave_students_by_date(self, date=None):
        """获取指定日期所有请假的学生"""
        if not date:
            date = datetime.now().strftime('%Y-%m-%d')
            
        query = """
        SELECT * FROM vw_LeaveApplicationDetail
        WHERE IsCurrentlyOnLeave = 1
        ORDER BY ClassName, StudentName
        """
        cursor = self.execute_query(query)
        return cursor.fetchall()
        
    # ==================== 班主任管理 ====================
    def add_head_teacher(self, name, code, class_name, phone='', email=''):
        """添加班主任"""
        query = """
        INSERT INTO HeadTeachers (TeacherName, TeacherCode, ClassName, PhoneNumber, Email)
        VALUES (?, ?, ?, ?, ?)
        """
        try:
            self.execute_query(query, (name, code, class_name, phone, email))
            self.conn.commit()
            print(f"✅ 班主任 {name} 添加成功！")
            return True
        except Exception as e:
            print(f"❌ 添加失败: {e}")
            return False
            
    def list_head_teachers(self):
        """列出所有班主任"""
        query = "SELECT * FROM HeadTeachers ORDER BY ClassName"
        cursor = self.execute_query(query)
        return cursor.fetchall()
        
    # ==================== 统计报表 ====================
    def get_dormitory_statistics(self, dormitory_id=None):
        """获取宿舍统计信息"""
        if dormitory_id:
            query = "SELECT * FROM vw_DormitorySummary WHERE DormitoryId = ?"
            cursor = self.execute_query(query, (dormitory_id,))
        else:
            query = "SELECT * FROM vw_DormitorySummary ORDER BY DormitoryCode"
            cursor = self.execute_query(query)
        return cursor.fetchall()
        
    def get_check_in_statistics(self, dormitory_id, start_date=None, end_date=None):
        """获取查寝统计"""
        if not start_date:
            start_date = (datetime.now() - timedelta(days=7)).strftime('%Y-%m-%d')
        if not end_date:
            end_date = datetime.now().strftime('%Y-%m-%d')
            
        query = """
        SELECT 
            CheckInDate,
            COUNT(*) as TotalStudents,
            SUM(CASE WHEN Status = '在寝' THEN 1 ELSE 0 END) as PresentCount,
            SUM(CASE WHEN Status = '未归' THEN 1 ELSE 0 END) as AbsentCount,
            SUM(CASE WHEN Status = '请假' THEN 1 ELSE 0 END) as LeaveCount,
            SUM(CASE WHEN Status = '外出' THEN 1 ELSE 0 END) as OutCount,
            SUM(CASE WHEN Status = '晚归' THEN 1 ELSE 0 END) as LateCount
        FROM DormitoryCheckInRecords
        WHERE DormitoryId = ? AND CheckInDate BETWEEN ? AND ?
        GROUP BY CheckInDate
        ORDER BY CheckInDate DESC
        """
        cursor = self.execute_query(query, (dormitory_id, start_date, end_date))
        return cursor.fetchall()


# ==================== 命令行界面 ====================
def main():
    """主函数"""
    db_path = "StudentData.db"
    system = DormitoryLeaveSystem(db_path)
    system.connect()
    
    while True:
        print("\n" + "="*60)
        print("宿舍管理与请假管理系统")
        print("="*60)
        print("1. 初始化数据库表")
        print("2. 添加宿舍楼")
        print("3. 添加宿管人员")
        print("4. 添加房间")
        print("5. 查看宿舍楼信息")
        print("6. 查看查寝情况")
        print("7. 记录查寝")
        print("8. 申请请假")
        print("9. 审批请假")
        print("10. 查看班级出勤（班主任）")
        print("11. 查看请假学生列表")
        print("0. 退出")
        print("="*60)
        
        choice = input("\n请选择操作 (0-11): ").strip()
        
        if choice == '0':
            print("再见！")
            break
            
        elif choice == '1':
            if system.initialize_tables():
                print("数据库初始化完成！")
                
        elif choice == '2':
            name = input("宿舍楼名称 (如: 5号楼): ")
            code = input("宿舍楼代码 (如: D5): ")
            gender = input("性别 (男/女): ")
            floors = input("楼层数 (默认6): ") or "6"
            address = input("地址: ") or ""
            system.add_dormitory(name, code, gender, int(floors), address)
            
        elif choice == '3':
            dormitories = system.list_dormitories()
            print("\n宿舍楼列表:")
            for d in dormitories:
                print(f"  ID: {d['DormitoryId']}, 名称: {d['DormitoryName']}, 代码: {d['DormitoryCode']}")
            
            dorm_id = input("\n选择宿舍楼ID: ")
            name = input("宿管姓名: ")
            code = input("工号: ")
            floor = input("楼层 (留空表示整栋楼): ") or None
            phone = input("电话: ")
            position = input("职位 (默认宿管阿姨): ") or "宿管阿姨"
            
            if floor:
                floor = int(floor)
            
            system.add_staff(name, code, int(dorm_id), floor, phone, '女', position)
            
        elif choice == '4':
            dormitories = system.list_dormitories()
            print("\n宿舍楼列表:")
            for d in dormitories:
                print(f"  ID: {d['DormitoryId']}, 名称: {d['DormitoryName']}")
            
            dorm_id = input("\n选择宿舍楼ID: ")
            floor = input("楼层: ")
            room = input("房间号 (如: 101): ")
            beds = input("床位数 (默认4): ") or "4"
            
            system.add_room(int(dorm_id), int(floor), room, int(beds))
            
        elif choice == '5':
            dorms = system.list_dormitories()
            print("\n宿舍楼信息:")
            print(f"{'宿舍楼':<10} {'代码':<10} {'类型':<10} {'总房间':<10} {'总床位':<10} {'已住':<10} {'空余':<10} {'入住率':<10}")
            print("-" * 80)
            for d in dorms:
                print(f"{d['DormitoryName']:<10} {d['DormitoryCode']:<10} {d['GenderType']:<10} "
                      f"{d['TotalRooms']:<10} {d['TotalBeds']:<10} {d['TotalOccupied']:<10} "
                      f"{d['AvailableBeds']:<10} {d['OccupancyRate']:.1f}%")
                      
        elif choice == '6':
            dormitories = system.list_dormitories()
            print("\n宿舍楼列表:")
            for d in dormitories:
                print(f"  ID: {d['DormitoryId']}, 名称: {d['DormitoryName']}")
            
            dorm_id = input("\n选择宿舍楼ID: ")
            floor = input("楼层 (留空查看整栋楼): ") or None
            date = input("日期 (YYYY-MM-DD, 默认今天): ") or None
            
            if floor:
                floor = int(floor)
            
            results = system.get_daily_check_in_summary(int(dorm_id), floor, date)
            print("\n查寝情况汇总:")
            for r in results:
                print(f"  宿管: {r['StaffName']}")
                print(f"  楼层: {r['FloorNumber']}")
                print(f"  日期: {r['CheckInDate']}")
                print(f"  总人数: {r['TotalStudents']}, 在寝: {r['PresentCount']}, "
                      f"未归: {r['AbsentCount']}, 请假: {r['LeaveCount']}")
                      
        elif choice == '7':
            print("功能开发中...")
            
        elif choice == '8':
            print("功能开发中...")
            
        elif choice == '9':
            print("功能开发中...")
            
        elif choice == '10':
            print("功能开发中...")
            
        elif choice == '11':
            date = input("日期 (YYYY-MM-DD, 默认今天): ") or None
            students = system.get_leave_students_by_date(date)
            print("\n请假学生列表:")
            for s in students:
                print(f"  姓名: {s['StudentName']}, 班级: {s['ClassName']}")
                print(f"  请假类型: {s['LeaveType']}, 原因: {s['Reason']}")
                print(f"  时间: {s['LeaveStartTime']} ~ {s['LeaveEndTime']}")
                print("-" * 50)
                
        else:
            print("无效的选择！")
    
    system.disconnect()


if __name__ == "__main__":
    main()
