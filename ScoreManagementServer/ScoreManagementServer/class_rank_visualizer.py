#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
班级排名趋势分析工具
查看整个班级每个学生的排名变化情况
"""

import sqlite3
from datetime import datetime, timedelta

# 数据库路径
DB_PATH = 'E:\StudentScoreProject\ScoreManagementServer\ScoreManagementServer/StudentData.db'

# 科目映射
SUBJECT_MAPPING = {
    1: '语文', 2: '数学', 3: '英语',
    4: '物理', 5: '化学', 6: '生物',
    7: '政治', 8: '历史', 9: '地理',
    10: '总分'
}


def connect_db():
    """连接数据库"""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def get_all_classes(conn):
    """获取所有班级"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT DISTINCT ClassName
        FROM Students
        WHERE ClassName IS NOT NULL AND ClassName != ''
        ORDER BY ClassName
    """)
    return cursor.fetchall()


def get_students_in_class(conn, class_name):
    """获取班级所有学生"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT StudentId, StudentNumber, StudentName
        FROM Students
        WHERE ClassName = ?
        ORDER BY StudentNumber
    """, (class_name,))
    return cursor.fetchall()


def get_class_rank_trend(conn, class_name, subject_id, start_date=None, end_date=None, rank_type='grade'):
    """获取班级某科目的排名趋势

    Args:
        conn: 数据库连接
        class_name: 班级名称
        subject_id: 科目ID
        start_date: 开始日期
        end_date: 结束日期
        rank_type: 排名类型，'class'为班级排名，'grade'为年级排名（默认）
    """
    rank_field = 's.ClassRank' if rank_type == 'class' else 's.GradeRank'

    sql = f"""
        SELECT
            e.ExamId,
            e.ExamName,
            e.ExamDate,
            s.StudentId,
            st.StudentNumber,
            st.StudentName,
            {rank_field} as Rank
        FROM Scores s
        JOIN Exams e ON s.ExamId = e.ExamId
        JOIN Students st ON s.StudentId = st.StudentId
        WHERE st.ClassName = ? AND s.SubjectId = ?
    """

    params = [class_name, subject_id]

    if start_date:
        sql += " AND e.ExamDate >= ?"
        params.append(start_date)

    if end_date:
        sql += " AND e.ExamDate <= ?"
        params.append(end_date)

    sql += " ORDER BY e.ExamDate, st.StudentNumber"

    cursor = conn.cursor()
    cursor.execute(sql, params)
    return cursor.fetchall()


def get_all_subjects_with_scores(conn, class_name):
    """获取班级有成绩记录的科目"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT DISTINCT
            sb.SubjectId,
            sb.SubjectName,
            sb.SortOrder,
            COUNT(s.ScoreId) as RecordCount
        FROM Subjects sb
        JOIN Scores s ON sb.SubjectId = s.SubjectId
        JOIN Students st ON s.StudentId = st.StudentId
        WHERE st.ClassName = ?
        GROUP BY sb.SubjectId, sb.SubjectName, sb.SortOrder
        ORDER BY sb.SortOrder
    """, (class_name,))
    return cursor.fetchall()


def print_class_rank_summary(scores, class_name, subject_name, decline_threshold=5, rank_type='grade'):
    """打印班级排名变化摘要

    Args:
        scores: 成绩数据
        class_name: 班级名称
        subject_name: 科目名称
        decline_threshold: 退步显示阈值
        rank_type: 排名类型，'class'为班级排名，'grade'为年级排名
    """
    if not scores:
        print(f"⚠️  {class_name}没有{subject_name}成绩记录")
        return

    rank_type_name = "年级排名" if rank_type == 'grade' else "班级排名"

    # 按学生分组
    students_data = {}
    for s in scores:
        if s['StudentId'] not in students_data:
            students_data[s['StudentId']] = {
                'name': s['StudentName'],
                'number': s['StudentNumber'],
                'records': []
            }
        students_data[s['StudentId']]['records'].append(s)

    # 调试信息：显示有多少学生有记录
    print(f"\n📋 调试信息: 共获取 {len(scores)} 条成绩记录，涉及 {len(students_data)} 名学生")

    # 找出进步和退步最大的学生
    improvements = []
    declines = []
    no_change = []

    # 统计每个学生的考试次数
    exam_count_distribution = {}
    for student_id, data in students_data.items():
        records = sorted(data['records'], key=lambda x: datetime.strptime(x['ExamDate'], '%Y-%m-%d'))
        exam_count = len(records)
        exam_count_distribution[exam_count] = exam_count_distribution.get(exam_count, 0) + 1

    # 显示考试次数分布
    print(f"📊 考试次数分布:", end=" ")
    for exam_count in sorted(exam_count_distribution.keys()):
        print(f"{exam_count}次考试: {exam_count_distribution[exam_count]}人", end="; ")
    print()

    for student_id, data in students_data.items():
        records = sorted(data['records'], key=lambda x: datetime.strptime(x['ExamDate'], '%Y-%m-%d'))

        if len(records) >= 2:
            first_rank = records[0]['Rank'] if records[0]['Rank'] else 999
            last_rank = records[-1]['Rank'] if records[-1]['Rank'] else 999

            change = first_rank - last_rank  # 正数表示进步（排名上升）

            if change > 0:
                improvements.append((change, data['name'], data['number'],
                                   records[0]['ExamName'], records[0]['Rank'],
                                   records[-1]['ExamName'], records[-1]['Rank']))
            elif change < 0:
                declines.append((abs(change), data['name'], data['number'],
                               records[0]['ExamName'], records[0]['Rank'],
                               records[-1]['ExamName'], records[-1]['Rank']))
            else:
                no_change.append((data['name'], data['number']))

    improvements.sort(reverse=True)
    declines.sort(reverse=True)

    print(f"\n{'='*80}")
    print(f"📊 {class_name} - {subject_name}{rank_type_name}变化分析")
    print(f"{'='*80}")

    # 显示所有进步的学生
    if improvements:
        print(f"\n📈 进步学生（共{len(improvements)}人）:")
        print(f"{'序号':<6} {'姓名':<10} {'学号':<12} {'变化':<8} {'初始考试':<15} {'初始排名':<10} {'最近考试':<15} {'最新排名':<10}")
        print(f"{'-'*80}")
        for i, (change, name, number, first_exam, first_rank, last_exam, last_rank) in enumerate(improvements, 1):
            first_rank_str = str(first_rank) if first_rank else 'N/A'
            last_rank_str = str(last_rank) if last_rank else 'N/A'
            print(f"{i:<6} {name:<10} {number:<12} +{change:<7} {first_exam:<15} {first_rank_str:<10} {last_exam:<15} {last_rank_str:<10}")

    # 只显示退步明显的学生（超过阈值）
    if declines:
        significant_declines = [d for d in declines if d[0] >= decline_threshold]
        if significant_declines:
            print(f"\n📉 明显退步学生（退步{decline_threshold}名及以上，共{len(significant_declines)}人）:")
            print(f"{'序号':<6} {'姓名':<10} {'学号':<12} {'变化':<8} {'初始考试':<15} {'初始排名':<10} {'最近考试':<15} {'最新排名':<10}")
            print(f"{'-'*80}")
            for i, (change, name, number, first_exam, first_rank, last_exam, last_rank) in enumerate(significant_declines, 1):
                first_rank_str = str(first_rank) if first_rank else 'N/A'
                last_rank_str = str(last_rank) if last_rank else 'N/A'
                print(f"{i:<6} {name:<10} {number:<12} -{change:<7} {first_exam:<15} {first_rank_str:<10} {last_exam:<15} {last_rank_str:<10}")
        else:
            print(f"\n📉 没有学生退步{decline_threshold}名及以上")

    # 显示统计信息
    total_students = len(students_data)
    valid_students = len(improvements) + len(declines) + len(no_change)
    print(f"\n📌 统计摘要:")
    print(f"  班级总人数: {total_students}人")
    print(f"  有记录人数: {valid_students}人")
    print(f"  进步人数: {len(improvements)}人")
    print(f"  退步人数: {len(declines)}人")
    print(f"  保持不变: {len(no_change)}人")

    if improvements:
        avg_improvement = sum(i[0] for i in improvements) / len(improvements)
        print(f"  平均进步: {avg_improvement:.1f}名")

    if declines:
        avg_decline = sum(d[0] for d in declines) / len(declines)
        print(f"  平均退步: {avg_decline:.1f}名")

    print(f"{'='*80}")


def main():
    print("=" * 80)
    print("          班级排名趋势分析工具")
    print("=" * 80)

    conn = connect_db()

    try:
        while True:  # 主循环：支持连续查询
            # 第一步：选择班级
            classes = get_all_classes(conn)

            if not classes:
                print("❌ 数据库中没有班级信息")
                return

            print(f"\n找到 {len(classes)} 个班级:")
            for i, c in enumerate(classes, 1):
                print(f"  {i}. {c['ClassName']}")

            choice = input("\n请选择班级编号: ").strip()
            if not choice.isdigit() or int(choice) < 1 or int(choice) > len(classes):
                print("❌ 无效选择")
                continue

            class_name = classes[int(choice) - 1]['ClassName']

            # 第二步：显示可用科目
            subjects = get_all_subjects_with_scores(conn, class_name)

            if not subjects:
                print(f"⚠️  {class_name}没有成绩记录")
                continue

            print(f"\n{'='*80}")
            print(f"  {class_name} 的考试科目")
            print(f"{'='*80}")

            for i, (subject_id, subject_name, sort_order, count) in enumerate(subjects, 1):
                print(f"  {i}. {subject_name} ({count}条记录)")

            choice = input(f"\n请选择科目（1-{len(subjects)}）: ").strip()
            if not choice.isdigit() or int(choice) < 1 or int(choice) > len(subjects):
                print("❌ 无效选择")
                continue

            subject_id, subject_name = subjects[int(choice) - 1][:2]

            # 第三步：选择排名类型（移到前面，因为选择时间范围时需要用到）
            print(f"\n{'='*80}")
            print(f"  选择排名类型")
            print(f"{'='*80}")
            print(f"  1. 年级排名（默认）")
            print(f"  2. 班级排名")

            rank_type_choice = input(f"\n请选择排名类型（1-2，默认1）: ").strip()
            if not rank_type_choice or not rank_type_choice.isdigit():
                rank_type = 'grade'
            elif rank_type_choice == '1':
                rank_type = 'grade'
            else:
                rank_type = 'class'

            rank_type_name = "年级排名" if rank_type == 'grade' else "班级排名"
            print(f"  排名类型: {rank_type_name}")

            # 第四步：选择时间范围
            print(f"\n{'='*80}")
            print(f"  选择时间范围")
            print(f"{'='*80}")
            print(f"  1. 本学期（最近6个月）")
            print(f"  2. 最近两次考试")
            print(f"  3. 全部历史数据")

            choice = input(f"\n请选择时间范围（1-3，默认3）: ").strip()
            if not choice or not choice.isdigit():
                choice = 3
            else:
                choice = int(choice)

            start_date = None
            end_date = None

            if choice == 1:
                # 本学期（最近6个月）
                end_date = datetime.now().strftime('%Y-%m-%d')
                start_date = (datetime.now() - timedelta(days=180)).strftime('%Y-%m-%d')
                print(f"  时间范围: {start_date} 至 {end_date}")
            elif choice == 2:
                # 获取最近两次考试的日期（从 Exams 表中查询该科目有考试的记录）
                cursor = conn.cursor()
                cursor.execute("""
                    SELECT DISTINCT e.ExamId, e.ExamDate, e.ExamName
                    FROM Exams e
                    JOIN Scores s ON e.ExamId = s.ExamId
                    WHERE s.SubjectId = ?
                    ORDER BY e.ExamDate DESC
                    LIMIT 2
                """, (subject_id,))
                recent_exams = cursor.fetchall()

                if len(recent_exams) >= 2:
                    # 获取这两次考试的日期范围
                    exam_dates = [e['ExamDate'] for e in recent_exams]
                    start_date = min(exam_dates)
                    end_date = max(exam_dates)
                    print(f"  时间范围: 最近两次考试 ({start_date} 至 {end_date})")
                    print(f"    考试1: {recent_exams[1]['ExamName']} ({recent_exams[1]['ExamDate']})")
                    print(f"    考试2: {recent_exams[0]['ExamName']} ({recent_exams[0]['ExamDate']})")
                else:
                    print("  ⚠️ 考试次数不足2次，使用全部数据")
            else:
                print("  时间范围: 全部历史数据")

            # 第五步：选择退步阈值
            decline_threshold = input("\n退步显示阈值（退步多少名以上才显示，默认5名）: ").strip()
            if decline_threshold and decline_threshold.isdigit():
                decline_threshold = int(decline_threshold)
            else:
                decline_threshold = 5
            print(f"退步阈值: {decline_threshold}名")

            # 第六步：获取数据并分析
            print(f"\n正在获取{class_name}的{subject_name}{rank_type_name}数据...")
            scores = get_class_rank_trend(conn, class_name, subject_id, start_date, end_date, rank_type)

            if not scores:
                print(f"⚠️  该班级没有{subject_name}成绩记录")
                continue

            # 打印摘要
            print_class_rank_summary(scores, class_name, subject_name, decline_threshold, rank_type)

            # 询问是否继续
            while True:
                continue_query = input("\n是否继续查询？(y/n): ").strip().lower()
                if continue_query in ['y', 'n']:
                    break
                print("❌ 请输入 y 或 n")

            if continue_query == 'n':
                print("\n感谢使用班级排名趋势分析工具，再见！")
                break  # 退出主循环

    except Exception as e:
        print(f"\n❌ 发生错误: {e}")
        import traceback
        traceback.print_exc()

    finally:
        conn.close()


if __name__ == '__main__':
    main()
