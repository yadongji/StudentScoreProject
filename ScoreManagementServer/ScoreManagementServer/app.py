"""
宿舍管理与请假管理系统 - Web版本
宿管阿姨、班主任、学生家长都可以通过浏览器使用
"""

from flask import Flask, render_template, request, jsonify, redirect, url_for, session
import sqlite3
from datetime import datetime, timedelta
import json
from functools import wraps

app = Flask(__name__)
app.secret_key = 'dormitory_system_2026_secret_key'  # 用于session管理
app.config['JSON_AS_ASCII'] = False  # 支持中文

# 数据库路径
DB_PATH = 'StudentData.db'


def get_db():
    """获取数据库连接"""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


# ============================================
# 装饰器：登录验证
# ============================================
def login_required(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        if 'user_id' not in session:
            return redirect(url_for('login'))
        return f(*args, **kwargs)
    return decorated_function


def role_required(role):
    """角色权限验证"""
    def decorator(f):
        @wraps(f)
        def decorated_function(*args, **kwargs):
            if 'user_id' not in session:
                return redirect(url_for('login'))
            if session.get('role') != role:
                return redirect(url_for('index'))
            return f(*args, **kwargs)
        return decorated_function
    return decorator


# ============================================
# 首页和登录
# ============================================
@app.route('/')
def index():
    """首页"""
    if 'user_id' in session:
        # 已登录，跳转到对应的首页
        role = session.get('role')
        if role == 'admin':
            return redirect(url_for('admin_dashboard'))
        elif role == 'staff':
            return redirect(url_for('staff_dashboard'))
        elif role == 'teacher':
            return redirect(url_for('teacher_dashboard'))
        elif role == 'parent':
            return redirect(url_for('parent_dashboard'))
    
    return render_template('index.html')


@app.route('/login', methods=['GET', 'POST'])
def login():
    """登录页面"""
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')
        role = request.form.get('role')  # admin, staff, teacher, parent
        
        # 简化版：直接登录，实际应用中应该验证密码
        session['user_id'] = username
        session['user_name'] = username
        session['role'] = role
        
        # 跳转到对应的首页
        if role == 'admin':
            return redirect(url_for('admin_dashboard'))
        elif role == 'staff':
            return redirect(url_for('staff_dashboard'))
        elif role == 'teacher':
            return redirect(url_for('teacher_dashboard'))
        elif role == 'parent':
            return redirect(url_for('parent_dashboard'))
    
    return render_template('login.html')


@app.route('/logout')
def logout():
    """退出登录"""
    session.clear()
    return redirect(url_for('index'))


# ============================================
# 管理员界面
# ============================================
@app.route('/admin')
@login_required
@role_required('admin')
def admin_dashboard():
    """管理员首页"""
    return render_template('admin/dashboard.html', 
                         user_name=session.get('user_name'))


@app.route('/admin/dormitories')
@login_required
@role_required('admin')
def admin_dormitories():
    """宿舍楼管理"""
    conn = get_db()
    dorms = conn.execute("""
        SELECT * FROM vw_DormitorySummary
        ORDER BY DormitoryCode
    """).fetchall()
    conn.close()
    return render_template('admin/dormitories.html', 
                         dorms=dorms,
                         user_name=session.get('user_name'))


@app.route('/admin/staff')
@login_required
@role_required('admin')
def admin_staff():
    """宿管人员管理"""
    conn = get_db()
    staff = conn.execute("""
        SELECT ds.*, d.DormitoryName
        FROM DormitoryStaff ds
        JOIN Dormitories d ON ds.DormitoryId = d.DormitoryId
        ORDER BY ds.DormitoryId, ds.FloorNumber
    """).fetchall()
    conn.close()
    return render_template('admin/staff.html',
                         staff=staff,
                         user_name=session.get('user_name'))


@app.route('/api/admin/add_dormitory', methods=['POST'])
@login_required
@role_required('admin')
def api_add_dormitory():
    """添加宿舍楼API"""
    data = request.json
    conn = get_db()
    try:
        conn.execute("""
            INSERT INTO Dormitories (DormitoryName, DormitoryCode, GenderType, FloorCount, Address)
            VALUES (?, ?, ?, ?, ?)
        """, (data['name'], data['code'], data['gender'], data['floors'], data['address']))
        conn.commit()
        conn.close()
        return jsonify({'success': True, 'message': '添加成功'})
    except Exception as e:
        conn.close()
        return jsonify({'success': False, 'message': str(e)})


# ============================================
# 宿管阿姨界面
# ============================================
@app.route('/staff')
@login_required
@role_required('staff')
def staff_dashboard():
    """宿管首页"""
    conn = get_db()
    # 获取宿管负责的宿舍楼
    dorm_id = session.get('dormitory_id')
    
    # 查询今日查寝统计
    today = datetime.now().strftime('%Y-%m-%d')
    stats = conn.execute("""
        SELECT 
            COUNT(*) as TotalStudents,
            SUM(CASE WHEN Status = '在寝' THEN 1 ELSE 0 END) as PresentCount,
            SUM(CASE WHEN Status = '未归' THEN 1 ELSE 0 END) as AbsentCount,
            SUM(CASE WHEN Status = '请假' THEN 1 ELSE 0 END) as LeaveCount,
            SUM(CASE WHEN Status = '外出' THEN 1 ELSE 0 END) as OutCount,
            SUM(CASE WHEN Status = '晚归' THEN 1 ELSE 0 END) as LateCount
        FROM DormitoryCheckInRecords
        WHERE DormitoryId = ? AND CheckInDate = ?
    """, (dorm_id, today)).fetchone()
    
    conn.close()
    return render_template('staff/dashboard.html',
                         stats=stats,
                         user_name=session.get('user_name'),
                         dorm_name=session.get('dormitory_name'))


@app.route('/staff/checkin')
@login_required
@role_required('staff')
def staff_checkin():
    """查寝记录页面"""
    dorm_id = session.get('dormitory_id')
    date = request.args.get('date', datetime.now().strftime('%Y-%m-%d'))
    
    conn = get_db()
    # 获取所有房间
    rooms = conn.execute("""
        SELECT r.*, 
               COUNT(DISTINCT sda.StudentId) as OccupiedCount,
               GROUP_CONCAT(s.StudentName, ', ') as StudentNames
        FROM DormitoryRooms r
        LEFT JOIN StudentDormitoryAssignments sda ON r.RoomId = sda.RoomId AND sda.IsCurrent = 1
        LEFT JOIN Students s ON sda.StudentId = s.StudentId
        WHERE r.DormitoryId = ?
        GROUP BY r.RoomId
        ORDER BY r.FloorNumber, r.RoomNumber
    """, (dorm_id,)).fetchall()
    
    # 获取当日查寝记录
    checkins = conn.execute("""
        SELECT * FROM vw_DailyCheckInDetail
        WHERE DormitoryId = ? AND CheckInDate = ?
        ORDER BY FloorNumber, RoomNumber, BedNumber
    """, (dorm_id, date)).fetchall()
    
    conn.close()
    
    # 将查寝记录转换为字典，方便查找
    checkin_dict = {f"{row['RoomId']}_{row['StudentId']}": row for row in checkins}
    
    return render_template('staff/checkin.html',
                         rooms=rooms,
                         checkins=checkin_dict,
                         date=date,
                         user_name=session.get('user_name'))


@app.route('/api/staff/checkin', methods=['POST'])
@login_required
@role_required('staff')
def api_staff_checkin():
    """提交查寝记录API"""
    data = request.json
    conn = get_db()
    try:
        for record in data['records']:
            conn.execute("""
                INSERT OR REPLACE INTO DormitoryCheckInRecords 
                (StudentId, RoomId, DormitoryId, CheckInDate, CheckInTime, Status, StaffId, Remarks)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """, (record['student_id'], record['room_id'], record['dormitory_id'],
                  record['date'], record['time'], record['status'],
                  session.get('staff_id'), record.get('remarks', '')))
        
        conn.commit()
        conn.close()
        return jsonify({'success': True, 'message': '查寝记录提交成功'})
    except Exception as e:
        conn.close()
        return jsonify({'success': False, 'message': str(e)})


# ============================================
# 班主任界面
# ============================================
@app.route('/teacher')
@login_required
@role_required('teacher')
def teacher_dashboard():
    """班主任首页"""
    class_name = session.get('class_name')
    today = datetime.now().strftime('%Y-%m-%d')
    
    conn = get_db()
    # 获取班级今日出勤统计
    stats = conn.execute("""
        SELECT 
            COUNT(*) as TotalStudents,
            SUM(CASE WHEN TodayStatus = '请假不在校' THEN 1 ELSE 0 END) as LeaveCount,
            SUM(CASE WHEN TodayStatus = '在校' THEN 1 ELSE 0 END) as PresentCount,
            SUM(CASE WHEN TodayStatus = '未查寝' THEN 1 ELSE 0 END) as NoCheckInCount
        FROM vw_ClassAttendanceDaily
        WHERE ClassName = ?
    """, (class_name,)).fetchone()
    
    conn.close()
    return render_template('teacher/dashboard.html',
                         stats=stats,
                         user_name=session.get('user_name'),
                         class_name=class_name)


@app.route('/teacher/attendance')
@login_required
@role_required('teacher')
def teacher_attendance():
    """班级出勤详情"""
    class_name = session.get('class_name')
    
    conn = get_db()
    students = conn.execute("""
        SELECT * FROM vw_ClassAttendanceDaily
        WHERE ClassName = ?
        ORDER BY StudentNumber
    """, (class_name,)).fetchall()
    conn.close()
    
    return render_template('teacher/attendance.html',
                         students=students,
                         class_name=class_name,
                         user_name=session.get('user_name'))


@app.route('/teacher/leaves')
@login_required
@role_required('teacher')
def teacher_leaves():
    """请假申请列表"""
    class_name = session.get('class_name')
    
    conn = get_db()
    leaves = conn.execute("""
        SELECT * FROM vw_LeaveApplicationDetail
        WHERE ClassName = ? AND ApprovalStatus IN ('待审批', '已通过', '已拒绝')
        ORDER BY CreatedAt DESC
    """, (class_name,)).fetchall()
    conn.close()
    
    return render_template('teacher/leaves.html',
                         leaves=leaves,
                         class_name=class_name,
                         user_name=session.get('user_name'))


@app.route('/api/teacher/approve_leave', methods=['POST'])
@login_required
@role_required('teacher')
def api_approve_leave():
    """审批请假申请API"""
    data = request.json
    conn = get_db()
    try:
        conn.execute("""
            UPDATE LeaveApplications 
            SET ApproverId = ?, ApproverName = ?, ApprovalStatus = ?,
                ApprovalTime = datetime('now', 'localtime'), ApprovalRemarks = ?
            WHERE LeaveId = ?
        """, (session.get('teacher_id'), session.get('user_name'),
              data['status'], data.get('remarks', ''), data['leave_id']))
        conn.commit()
        conn.close()
        
        status_text = '通过' if data['status'] == '已通过' else '拒绝'
        return jsonify({'success': True, 'message': f'审批已{status_text}'})
    except Exception as e:
        conn.close()
        return jsonify({'success': False, 'message': str(e)})


# ============================================
# 家长界面
# ============================================
@app.route('/parent')
@login_required
@role_required('parent')
def parent_dashboard():
    """家长首页"""
    student_id = session.get('student_id')
    
    conn = get_db()
    # 获取学生信息
    student = conn.execute("""
        SELECT s.*, vw.RoomName, vw.DormitoryName, vw.RoomNumber, vw.BedNumber
        FROM Students s
        LEFT JOIN vw_StudentDormitoryDetail vw ON s.StudentId = vw.StudentId
        WHERE s.StudentId = ?
    """, (student_id,)).fetchone()
    
    # 获取今日出勤状态
    today = datetime.now().strftime('%Y-%m-%d')
    checkin = conn.execute("""
        SELECT Status FROM DormitoryCheckInRecords
        WHERE StudentId = ? AND CheckInDate = ?
    """, (student_id, today)).fetchone()
    
    # 获取最近的请假记录
    recent_leaves = conn.execute("""
        SELECT * FROM vw_LeaveApplicationDetail
        WHERE StudentId = ?
        ORDER BY CreatedAt DESC
        LIMIT 5
    """, (student_id,)).fetchall()
    
    conn.close()
    
    return render_template('parent/dashboard.html',
                         student=student,
                         checkin=checkin,
                         recent_leaves=recent_leaves,
                         user_name=session.get('user_name'))


@app.route('/parent/apply_leave', methods=['GET', 'POST'])
@login_required
@role_required('parent')
def parent_apply_leave():
    """申请请假"""
    if request.method == 'POST':
        data = request.json
        conn = get_db()
        try:
            # 计算请假天数
            start_dt = datetime.strptime(data['start_time'], '%Y-%m-%d %H:%M:%S')
            end_dt = datetime.strptime(data['end_time'], '%Y-%m-%d %H:%M:%S')
            leave_days = (end_dt - start_dt).days + 1
            
            conn.execute("""
                INSERT INTO LeaveApplications 
                (StudentId, LeaveType, Reason, StartTime, EndTime, LeaveDays,
                 ParentContact, ParentConfirm, ApprovalStatus)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, '待审批')
            """, (session.get('student_id'), data['leave_type'], data['reason'],
                  data['start_time'], data['end_time'], leave_days,
                  data.get('parent_contact', ''), 1))
            
            conn.commit()
            conn.close()
            return jsonify({'success': True, 'message': '请假申请提交成功'})
        except Exception as e:
            conn.close()
            return jsonify({'success': False, 'message': str(e)})
    
    return render_template('parent/apply_leave.html',
                         student_id=session.get('student_id'),
                         user_name=session.get('user_name'))


@app.route('/parent/leave_history')
@login_required
@role_required('parent')
def parent_leave_history():
    """请假历史"""
    student_id = session.get('student_id')
    
    conn = get_db()
    leaves = conn.execute("""
        SELECT * FROM vw_LeaveApplicationDetail
        WHERE StudentId = ?
        ORDER BY CreatedAt DESC
    """, (student_id,)).fetchall()
    conn.close()
    
    return render_template('parent/leave_history.html',
                         leaves=leaves,
                         user_name=session.get('user_name'))


# ============================================
# 启动服务器
# ============================================
if __name__ == '__main__':
    print("="*60)
    print("宿舍管理与请假管理系统 - Web版")
    print("="*60)
    print("\n🌐 服务器启动中...")
    print("📍 访问地址: http://127.0.0.1:5000")
    print("📱 手机访问: 请确保在同一局域网内")
    print("\n按 Ctrl+C 停止服务器\n")
    print("="*60)
    
    app.run(host='0.0.0.0', port=5000, debug=True)
