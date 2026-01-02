#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
限时练成绩查询工具
支持按人和按班级查询
"""

import sqlite3
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from datetime import datetime
import numpy as np

# 数据库路径
DB_PATH = r'E:\StudentScoreProject\ScoreManagementServer\ScoreManagementServer/StudentData.db'

# 设置中文字体
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun']
plt.rcParams['axes.unicode_minus'] = False


def connect_db():
    """连接数据库"""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def search_student(conn, keyword):
    """搜索学生（按学号或姓名）"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT StudentId, StudentNumber, StudentName, ClassName
        FROM Students
        WHERE StudentNumber LIKE ? OR StudentName LIKE ?
        ORDER BY StudentNumber
    """, (f'%{keyword}%', f'%{keyword}%'))
    return cursor.fetchall()


def get_time_limit_exams(conn, subject_id=None):
    """获取限时练考试列表"""
    cursor = conn.cursor()

    if subject_id:
        cursor.execute("""
            SELECT ExamId, ExamName, ExamDate, SubjectId
            FROM TimeLimitExams
            WHERE SubjectId = ?
            ORDER BY ExamDate DESC
        """, (subject_id,))
    else:
        cursor.execute("""
            SELECT ExamId, ExamName, ExamDate, SubjectId
            FROM TimeLimitExams
            ORDER BY ExamDate DESC
        """)

    return cursor.fetchall()


def get_student_time_limit_scores(conn, student_id, exam_ids=None, limit=None):
    """获取学生限时练成绩 - 按考试日期升序排序"""
    sql = """
        SELECT
            tle.ExamId,
            tle.ExamName,
            tle.ExamDate,
            tls.SubjectId,
            sb.SubjectName,
            tls.Score,
            tls.ClassRank,
            tls.GradeRank
        FROM TimeLimitScores tls
        JOIN TimeLimitExams tle ON tls.TimeLimitExamId = tle.ExamId
        JOIN Subjects sb ON tls.SubjectId = sb.SubjectId
        WHERE tls.StudentId = ?
    """
    params = [student_id]

    if exam_ids:
        placeholders = ','.join('?' * len(exam_ids))
        sql += f" AND tle.ExamId IN ({placeholders})"
        params.extend(exam_ids)

    # 按日期升序排序（从早到晚）
    sql += " ORDER BY tle.ExamDate ASC"

    if limit:
        # 如果需要限制数量，先用子查询排序，再取最近的N条
        sql = f"""
            SELECT * FROM (
                {sql}
            ) ORDER BY ExamDate DESC
            LIMIT {limit}
        """

    cursor = conn.cursor()
    cursor.execute(sql, params)
    return cursor.fetchall()


def plot_student_grade_rank_trend(scores, student_name, subject_name=None):
    """绘制学生年级排名趋势图"""
    if len(scores) < 2:
        print("⚠️  限时练记录少于2次，无法绘制趋势图")
        return

    # 按日期升序排序（从早到晚）
    scores = sorted(scores, key=lambda x: datetime.strptime(x['ExamDate'], '%Y-%m-%d'))

    exam_names = [s['ExamName'] for s in scores]
    exam_dates = [datetime.strptime(s['ExamDate'], '%Y-%m-%d') for s in scores]
    grade_ranks = [s['GradeRank'] if s['GradeRank'] else 0 for s in scores]

    # 创建图表
    fig, ax = plt.subplots(figsize=(12, 6))

    # 绘制排名折线
    color = '#e74c3c'  # 红色
    ax.set_xlabel('考试时间', fontsize=12)
    ax.set_ylabel('年级排名', color=color, fontsize=12)

    ax.plot(exam_dates, grade_ranks, 's-', color=color, linewidth=2.5, markersize=8, label='年级排名')
    ax.tick_params(axis='y', labelcolor=color)
    ax.grid(True, alpha=0.3)
    ax.invert_yaxis()  # 排名越小越好，反转y轴

    # 设置x轴格式 - 使用排序后的日期
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))
    ax.xaxis.set_major_locator(mdates.AutoDateLocator())
    plt.xticks(rotation=45, ha='right')

    # 添加数据标签和进步/退步标记
    for i, (x, y_rank) in enumerate(zip(exam_dates, grade_ranks)):
        # 排名标签
        if y_rank > 0:
            ax.text(x, y_rank, f'#{y_rank}',
                    ha='center', va='top', fontsize=9, color=color, fontweight='bold')

        # 进步/退步标记
        if i > 0:
            rank_change = grade_ranks[i-1] - y_rank  # 正数表示进步（排名上升）
            if rank_change > 0:
                trend_text = f'↑+{rank_change}'
                trend_color = 'green'
            elif rank_change < 0:
                trend_text = f'↓{rank_change}'  # 直接显示负数，如 ↓-26
                trend_color = 'red'
            else:
                trend_text = '→0'
                trend_color = 'gray'

            # 计算两个日期之间的中间点
            date_diff = x - exam_dates[i-1]
            mid_x = exam_dates[i-1] + date_diff / 2
            mid_y = (grade_ranks[i-1] + y_rank) / 2

            ax.text(mid_x, mid_y, trend_text,
                    ha='center', va='center', fontsize=10,
                    color=trend_color, fontweight='bold',
                    bbox=dict(boxstyle='round,pad=0.3', facecolor='white', alpha=0.8))

    # 标题
    if subject_name:
        title = f'{student_name} - 限时练{subject_name}年级排名趋势'
    else:
        title = f'{student_name} - 限时练年级排名趋势'
    plt.title(title, fontsize=14, fontweight='bold', pad=20)

    plt.tight_layout()

    # 保存图表
    filename = f'限时练排名趋势_{student_name}.png'
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    print(f"\n✅ 图表已保存: {filename}")

    plt.show()


def query_by_student(conn):
    """按学生查询"""
    print("\n" + "=" * 80)
    print("按学生查询限时练成绩")
    print("=" * 80)

    # 搜索学生
    while True:
        keyword = input("\n请输入学号或姓名（支持模糊搜索）: ").strip()
        if not keyword:
            continue

        students = search_student(conn, keyword)

        if not students:
            print("❌ 未找到匹配的学生，请重新输入")
            continue

        print(f"\n找到 {len(students)} 个学生:")
        for i, s in enumerate(students, 1):
            print(f"  {i}. 学号: {s['StudentNumber']}, 姓名: {s['StudentName']}, 班级: {s['ClassName'] or '未设置'}")

        choice = input("\n请选择学生编号: ").strip()
        if not choice.isdigit() or int(choice) < 1 or int(choice) > len(students):
            print("❌ 无效选择")
            continue

        student = students[int(choice) - 1]
        student_id = student['StudentId']
        student_name = student['StudentName']
        break

    # 选择查询范围
    print(f"\n{'='*80}")
    print("选择查询范围")
    print(f"{'='*80}")
    print("  1. 查询最近3次")
    print("  2. 查询最近5次")
    print("  3. 查询最近10次")
    print("  4. 查询全部")

    limit = input("\n请选择（1-4，默认4）: ").strip()
    limit_map = {'1': 3, '2': 5, '3': 10, '4': None}
    limit = limit_map.get(limit, None)

    if limit:
        print(f"查询最近 {limit} 次限时练")
    else:
        print("查询全部限时练")

    # 获取成绩数据（已按日期升序排序）
    scores = get_student_time_limit_scores(conn, student_id, limit=limit)

    if not scores:
        print(f"\n⚠️  该学生没有限时练成绩记录")
        return

    # 转换为列表
    scores = list(scores)

    # 显示成绩表（按日期升序显示，从早到晚）
    print(f"\n{'='*80}")
    print(f"限时练成绩记录（按时间顺序）")
    print(f"{'='*80}")

    print(f"{'考试名称':<30} {'考试日期':<12} {'科目':<10} {'成绩':<8} {'班级排名':<10} {'年级排名':<10}")
    print(f"{'-'*80}")
    for s in scores:
        class_rank = str(s['ClassRank']) if s['ClassRank'] else '-'
        grade_rank = str(s['GradeRank']) if s['GradeRank'] else '-'
        print(f"{s['ExamName']:<30} {s['ExamDate']:<12} {s['SubjectName']:<10} {s['Score']:<8.1f} {class_rank:<10} {grade_rank:<10}")

    # 绘制趋势图
    if len(scores) >= 2:
        subject_name = scores[0]['SubjectName'] if len(set(s['SubjectName'] for s in scores)) == 1 else None
        plot_student_grade_rank_trend(scores, student_name, subject_name)


def get_class_time_limit_progress(conn, class_name, exam_count=3):
    """获取班级限时练累计进步情况"""
    cursor = conn.cursor()

    # 获取该班级所有学生
    cursor.execute("SELECT StudentId, StudentNumber, StudentName FROM Students WHERE ClassName = ?", (class_name,))
    students = cursor.fetchall()

    if not students:
        return [], [], []

    # 获取所有限时练考试（按日期倒序，最近的在前）
    cursor.execute("""
        SELECT ExamId, ExamName, ExamDate
        FROM TimeLimitExams
        ORDER BY ExamDate DESC
    """)
    all_exams = cursor.fetchall()

    if len(all_exams) < 2:
        return [], [], []

    # 只取最近exam_count次考试
    exams = list(all_exams[:exam_count])
    exam_ids = [e['ExamId'] for e in exams]

    # 获取每个学生在这些考试中的详细成绩变化
    detailed_progress = []

    for student in students:
        student_id = student['StudentId']

        # 获取该学生在这些考试中的成绩（按时间升序排列，从早到晚）
        placeholders = ','.join('?' * len(exam_ids))
        cursor.execute(f"""
            SELECT tle.ExamId, tle.ExamName, tle.ExamDate, tls.GradeRank, tls.ClassRank
            FROM TimeLimitScores tls
            JOIN TimeLimitExams tle ON tls.TimeLimitExamId = tle.ExamId
            WHERE tls.StudentId = ? AND tle.ExamId IN ({placeholders})
            ORDER BY tle.ExamDate ASC
        """, [student_id] + exam_ids)

        scores = cursor.fetchall()

        if len(scores) < 2:
            continue

        # 计算每次考试相比上一次的进步/退步
        score_changes = []
        total_progress = 0

        for i in range(len(scores)):
            exam_name = scores[i]['ExamName']
            exam_date = scores[i]['ExamDate']
            grade_rank = scores[i]['GradeRank'] if scores[i]['GradeRank'] else 999
            class_rank = scores[i]['ClassRank'] if scores[i]['ClassRank'] else 999

            if i > 0:
                prev_rank = scores[i-1]['GradeRank'] if scores[i-1]['GradeRank'] else 999
                change = prev_rank - grade_rank  # 正数表示进步
                total_progress += change

                if change > 0:
                    change_str = f"↑+{change}"
                elif change < 0:
                    change_str = f"↓{abs(change)}"
                else:
                    change_str = "→0"
            else:
                change_str = "-"
                total_progress = 0

            score_changes.append({
                'exam_name': exam_name,
                'exam_date': exam_date,
                'grade_rank': grade_rank,
                'class_rank': class_rank,
                'change': change_str
            })

        detailed_progress.append({
            'student_name': student['StudentName'],
            'student_number': student['StudentNumber'],
            'total_progress': total_progress,
            'score_changes': score_changes
        })

    # 按总进步量排序（从大到小）
    detailed_progress.sort(key=lambda x: x['total_progress'], reverse=True)

    # 分离有进步、没变化、退步的学生
    students_progress = []
    no_change_students = []
    recent_progress = []

    for dp in detailed_progress:
        if dp['total_progress'] > 0:
            recent_progress.append(dp)
        elif dp['total_progress'] == 0:
            no_change_students.append(dp)

    return detailed_progress, no_change_students, recent_progress


def query_by_class(conn):
    """按班级查询"""
    print("\n" + "=" * 80)
    print("按班级查询限时练成绩")
    print("=" * 80)

    # 获取所有班级
    cursor = conn.cursor()
    cursor.execute("""
        SELECT DISTINCT ClassName
        FROM Students
        WHERE ClassName IS NOT NULL AND ClassName != ''
        ORDER BY ClassName
    """)
    classes = cursor.fetchall()

    if not classes:
        print("❌ 数据库中没有班级信息")
        return

    print(f"\n找到 {len(classes)} 个班级:")
    for i, c in enumerate(classes, 1):
        print(f"  {i}. {c['ClassName']}")

    choice = input("\n请选择班级编号: ").strip()
    if not choice.isdigit() or int(choice) < 1 or int(choice) > len(classes):
        print("❌ 无效选择")
        return

    class_name = classes[int(choice) - 1]['ClassName']

    # 选择查看最近几次考试
    print(f"\n{'='*80}")
    print("选择查看最近几次考试的进步情况")
    print(f"{'='*80}")
    print("  2. 最近2次")
    print("  3. 最近3次")
    print("  4. 最近4次")
    print("  5. 最近5次")

    exam_count = input("\n请选择（2-5，默认3）: ").strip()
    if not exam_count or not exam_count.isdigit():
        exam_count = '3'
    exam_count = min(5, max(2, int(exam_count)))  # 限制在2-5之间

    print(f"\n正在分析最近{exam_count}次考试的进步情况...")

    # 获取班级限时练进步情况
    detailed_progress, no_change, progress_students = get_class_time_limit_progress(conn, class_name, exam_count)

    if not detailed_progress and not progress_students:
        print(f"\n⚠️  {class_name}没有足够的限时练记录")
        return

    print(f"\n{'='*80}")
    print(f"{class_name} - 最近{exam_count}次限时练进步详情")
    print(f"{'='*80}")

    # 显示进步学生（总进步量大于0的）
    if progress_students:
        print(f"\n【进步学生】（共{len(progress_students)}人，按总进步量排序）")
        print(f"{'-'*80}")
        for i, dp in enumerate(progress_students, 1):
            print(f"\n{i}. {dp['student_name']} (学号: {dp['student_number']}) - 总进步: ↑+{dp['total_progress']}名")
            for change in dp['score_changes']:
                rank_display = f"#{change['grade_rank']}" if change['grade_rank'] != 999 else "缺考"
                print(f"   {change['exam_date']} [{change['exam_name']}] 年级排名: {rank_display:>6} | 相比上一次: {change['change']}")

    # 显示退步学生（总进步量小于0的）
    declined_students = [dp for dp in detailed_progress if dp['total_progress'] < 0]
    if declined_students:
        print(f"\n【退步学生】（共{len(declined_students)}人，按退步幅度排序）")
        print(f"{'-'*80}")
        for i, dp in enumerate(declined_students, 1):
            print(f"\n{i}. {dp['student_name']} (学号: {dp['student_number']}) - 总退步: ↓{abs(dp['total_progress'])}名")
            for change in dp['score_changes']:
                rank_display = f"#{change['grade_rank']}" if change['grade_rank'] != 999 else "缺考"
                print(f"   {change['exam_date']} [{change['exam_name']}] 年级排名: {rank_display:>6} | 相比上一次: {change['change']}")

    print(f"\n{'='*80}")


def main():
    print("=" * 80)
    print("        限时练成绩查询工具")
    print("=" * 80)

    conn = connect_db()

    try:
        print("\n请选择查询方式:")
        print("  1. 按学生查询（查看年级排名趋势图）")
        print("  2. 按班级查询（查看累计进步排名）")

        choice = input("\n请选择（1-2）: ").strip()

        if choice == '1':
            query_by_student(conn)
        elif choice == '2':
            query_by_class(conn)
        else:
            print("❌ 无效选择")

    except Exception as e:
        print(f"\n❌ 发生错误: {e}")
        import traceback
        traceback.print_exc()

    finally:
        conn.close()


if __name__ == '__main__':
    main()
