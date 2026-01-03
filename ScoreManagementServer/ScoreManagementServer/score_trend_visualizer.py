#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
成绩趋势可视化工具
支持查询学生总分或单科成绩的进步/退退情况，并生成折线图
"""

import sqlite3
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from datetime import datetime
import numpy as np

# 数据库路径
DB_PATH = r'E:\StudentScoreProject\ScoreManagementServer\ScoreManagementServer\StudentData.db'

# 设置中文字体
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun']
plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题

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


def get_score_trend(conn, student_id, subject_id):
    """获取学生某科目的成绩趋势"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT
            e.ExamId,
            e.ExamName,
            e.ExamDate,
            s.Score,
            s.ClassRank,
            s.GradeRank,
            ROW_NUMBER() OVER (ORDER BY e.ExamDate) as ExamSeq
        FROM Scores s
        JOIN Exams e ON s.ExamId = e.ExamId
        WHERE s.StudentId = ? AND s.SubjectId = ?
        ORDER BY e.ExamDate
    """, (student_id, subject_id))
    return cursor.fetchall()


def get_all_subjects(conn, student_id):
    """获取学生的所有科目"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT DISTINCT
            sb.SubjectId,
            sb.SubjectName,
            COUNT(s.ScoreId) as ExamCount
        FROM Subjects sb
        JOIN Scores s ON sb.SubjectId = s.SubjectId
        WHERE s.StudentId = ?
        GROUP BY sb.SubjectId, sb.SubjectName
        ORDER BY sb.SortOrder
    """, (student_id,))
    return cursor.fetchall()


def plot_trend(scores, subject_name, student_name, show_grade_rank=True):
    """绘制成绩趋势图（仅显示年级排名）"""
    if len(scores) < 2:
        print("⚠️  该学生只有1次考试记录，无法绘制趋势图")
        return

    # 提取数据
    exam_names = [s['ExamName'] for s in scores]
    exam_dates = [datetime.strptime(s['ExamDate'], '%Y-%m-%d') for s in scores]
    grade_ranks = [s['GradeRank'] if s['GradeRank'] else 0 for s in scores]

    # 创建图表（仅用于显示排名）
    fig, ax1 = plt.subplots(figsize=(12, 6))

    # 绘制年级排名折线
    color = '#e74c3c'  # 红色
    ax1.set_xlabel('考试时间', fontsize=12)
    ax1.set_ylabel('年级排名', color=color, fontsize=12)
    line1 = ax1.plot(exam_dates, grade_ranks, 'o-', color=color, linewidth=2.5, markersize=8, label='年级排名')
    ax1.tick_params(axis='y', labelcolor=color)
    ax1.invert_yaxis()  # 排名越小越好，反转y轴
    ax1.grid(True, alpha=0.3)

    # 设置x轴格式
    ax1.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))
    ax1.xaxis.set_major_locator(mdates.AutoDateLocator())
    plt.xticks(rotation=45, ha='right')

    # 添加数据标签
    for i, (x, y_rank) in enumerate(zip(exam_dates, grade_ranks)):
        # 排名标签
        if y_rank > 0:
            ax1.text(x, y_rank, f'#{y_rank}',
                    ha='center', va='top', fontsize=9, color=color, fontweight='bold')

        # 进步/退步标记（基于排名变化）
        if i > 0 and y_rank > 0 and grade_ranks[i-1] > 0:
            prev_rank = grade_ranks[i-1]
            rank_change = prev_rank - y_rank  # 前一次排名 - 当前排名（正数表示进步）
            if rank_change > 0:
                trend_text = f'↑+{rank_change}'
                trend_color = 'green'
            elif rank_change < 0:
                trend_text = f'↓{rank_change}'
                trend_color = 'red'
            else:
                trend_text = '→0'
                trend_color = 'gray'

            # 计算两个日期之间的中间点
            date_diff = x - exam_dates[i-1]
            mid_x = exam_dates[i-1] + date_diff / 2
            mid_y = (grade_ranks[i-1] + y_rank) / 2
            ax1.text(mid_x, mid_y, trend_text,
                    ha='center', va='center', fontsize=10,
                    color=trend_color, fontweight='bold',
                    bbox=dict(boxstyle='round,pad=0.3', facecolor='white', alpha=0.8))

    # 图例
    ax1.legend([line1], ['年级排名'], loc='best')

    # 标题
    title = f'{student_name} - {subject_name}年级排名变化趋势'
    plt.title(title, fontsize=14, fontweight='bold', pad=20)

    plt.tight_layout()

    # 保存图表
    filename = f'趋势图_{student_name}_{subject_name}.png'
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    print(f"\n✅ 图表已保存: {filename}")

    plt.show()


def print_trend_summary(scores, subject_name):
    """打印趋势摘要"""
    if len(scores) < 2:
        print("\n该学生只有1次考试记录")
        return

    print(f"\n{'='*60}")
    print(f"📊 {subject_name}成绩趋势分析")
    print(f"{'='*60}")
    print(f"{'考试名称':<15} {'考试日期':<12} {'分数':<8} {'校名次':<8} {'变化':<10} {'趋势'}")
    print(f"{'-'*60}")

    for i, s in enumerate(scores):
        score_str = f"{s['Score']:.1f}" if s['Score'] is not None else '-'
        rank_str = str(s['GradeRank']) if s['GradeRank'] else '-'
        if i == 0:
            print(f"{s['ExamName']:<15} {s['ExamDate']:<12} {score_str:<8} {rank_str:<8} {'-':<10} {'-'}")
        else:
            # 计算排名变化（不是分数变化）
            prev_rank = scores[i-1]['GradeRank']
            rank_change = prev_rank - s['GradeRank']  # 排名变化：前一次排名 - 当前排名（正数表示进步）

            if rank_change > 0:
                trend = "↑ 进步"
                trend_icon = "📈"
            elif rank_change < 0:
                trend = "↓ 退步"
                trend_icon = "📉"
            else:
                trend = "→ 持平"
                trend_icon = "➡️"

            change_str = f"+{rank_change}" if rank_change > 0 else f"{rank_change}"

            print(f"{s['ExamName']:<15} {s['ExamDate']:<12} {score_str:<8} {rank_str:<8} {change_str:<10} {trend}")

    print(f"{'='*60}")


def main():
    print("=" * 60)
    print("        成绩趋势可视化工具")
    print("=" * 60)

    conn = connect_db()

    try:
        while True:  # 主循环：支持连续查询
            # 第一步：搜索学生
            while True:
                keyword = input("\n请输入学号或姓名（支持模糊搜索）: ").strip()
                if not keyword:
                    continue

                students = search_student(conn, keyword)

                if not students:
                    print("❌ 未找到匹配的学生，请重新输入")
                    continue

                    # 显示搜索结果
                print(f"\n找到 {len(students)} 个学生:")
                for i, s in enumerate(students, 1):
                    print(f"  {i}. 学号: {s['StudentNumber']}, 姓名: {s['StudentName']}, 班级: {s['ClassName'] or '未设置'}")

                # 选择学生
                choice = input("\n请选择学生编号（1-{}）: ".format(len(students))).strip()
                if not choice.isdigit() or int(choice) < 1 or int(choice) > len(students):
                    print("❌ 无效选择")
                    continue

                student = students[int(choice) - 1]
                student_id = student['StudentId']
                student_name = student['StudentName']
                student_number = student['StudentNumber']

                break

            # 第二步：显示所有可用科目
            subjects = get_all_subjects(conn, student_id)

            print(f"\n{'='*60}")
            print(f"  {student_name} ({student_number}) 的考试科目")
            print(f"{'='*60}")

            subject_options = []
            for i, (subject_id, subject_name, exam_count) in enumerate(subjects, 1):
                print(f"  {i}. {subject_name} ({exam_count}次考试)")
                subject_options.append((subject_id, subject_name))

            # 第三步：选择科目
            while True:
                print(f"\n  {len(subject_options) + 1}. 全部科目对比")
                print(f"  {len(subject_options) + 2}. 综合查看（含总分排名变化）")
                choice = input(f"\n请选择科目（1-{len(subject_options) + 2}）: ").strip()

                if not choice.isdigit() or int(choice) < 1 or int(choice) > len(subject_options) + 2:
                    print("❌ 无效选择，请重新输入")
                    continue

                choice = int(choice)
                break

            # 第四步：选择展示内容
            show_grade_rank = input("\n是否显示年级排名？（y/n，默认y）: ").strip().lower()
            show_grade_rank = show_grade_rank != 'n'

            # 第五步：生成图表
            if choice <= len(subject_options):
                # 选择单个科目
                subject_id, subject_name = subject_options[choice - 1]
                scores = get_score_trend(conn, student_id, subject_id)

                if not scores:
                    print(f"⚠️  该学生没有{subject_name}成绩记录")
                else:
                    # 打印趋势摘要
                    print_trend_summary(scores, subject_name)

                    # 绘制图表
                    plot_trend(scores, subject_name, student_name, show_grade_rank)

            elif choice == len(subject_options) + 1:
                # 全部科目对比
                print(f"\n正在生成{student_name}的全部科目对比图...")
                plot_all_subjects(conn, student_id, student_name, show_grade_rank)

            else:
                # 综合查看（含总分排名变化）
                print(f"\n正在生成{student_name}的综合成绩分析图...")
                plot_comprehensive_view(conn, student_id, student_name)

            # 询问是否继续
            while True:
                continue_query = input("\n是否继续查询？(y/n): ").strip().lower()
                if continue_query in ['y', 'n']:
                    break
                print("❌ 请输入 y 或 n")

            if continue_query == 'n':
                print("\n感谢使用成绩趋势可视化工具，再见！")
                break  # 退出主循环

    except Exception as e:
        print(f"\n❌ 发生错误: {e}")
        import traceback
        traceback.print_exc()

    finally:
        conn.close()


def plot_all_subjects(conn, student_id, student_name, show_grade_rank):
    """绘制所有科目对比图（仅显示年级排名）"""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT
            e.ExamDate,
            e.ExamName,
            sb.SubjectName,
            s.GradeRank,
            sb.SortOrder
        FROM Scores s
        JOIN Exams e ON s.ExamId = e.ExamId
        JOIN Subjects sb ON s.SubjectId = sb.SubjectId
        WHERE s.StudentId = ? AND sb.SubjectId != 10
        ORDER BY e.ExamDate, sb.SortOrder
    """, (student_id,))

    all_scores = cursor.fetchall()

    if not all_scores:
        print("⚠️  该学生没有成绩记录")
        return

    # 整理数据
    exams = sorted(set(s['ExamDate'] for s in all_scores), key=lambda x: datetime.strptime(x, '%Y-%m-%d'))

    # 获取所有科目并按SortOrder排序
    subject_info = {}
    for s in all_scores:
        subject_name = s['SubjectName']
        if subject_name not in subject_info:
            subject_info[subject_name] = s['SortOrder']
    subjects = sorted(subject_info.keys(), key=lambda x: subject_info[x])

    # 构建矩阵
    data_matrix = {}
    for subject in subjects:
        data_matrix[subject] = []
        for exam in exams:
            found = False
            for s in all_scores:
                if s['ExamDate'] == exam and s['SubjectName'] == subject:
                    data_matrix[subject].append(s['GradeRank'] if s['GradeRank'] else 999)
                    found = True
                    break
            if not found:
                data_matrix[subject].append(999)  # 999 表示缺考

    # 绘制
    fig, ax = plt.subplots(figsize=(14, 8))

    colors = ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c',
             '#e67e22', '#34495e', '#7f8c8d']

    for i, subject in enumerate(subjects):
        if not data_matrix[subject]:
            continue

        exam_dates = [datetime.strptime(d, '%Y-%m-%d') for d in exams]
        ranks = data_matrix[subject]
        color = colors[i % len(colors)]

        # 只绘制有效数据（非999）
        valid_data = [(x, y) for x, y in zip(exam_dates, ranks) if y != 999]
        if valid_data:
            valid_dates, valid_ranks = zip(*valid_data)
            ax.plot(valid_dates, valid_ranks, 'o-', color=color, linewidth=2, markersize=6,
                    label=subject, alpha=0.8)

            # 添加数据标签
            for x, y in zip(valid_dates, valid_ranks):
                ax.text(x, y, f'#{y}',
                       ha='center', va='top', fontsize=8, color=color, fontweight='bold')

    # 设置x轴
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))
    ax.xaxis.set_major_locator(mdates.AutoDateLocator())
    plt.xticks(rotation=45, ha='right')

    # 网格和标签
    ax.grid(True, alpha=0.3)
    ax.set_xlabel('考试时间', fontsize=12)
    ax.set_ylabel('年级排名', fontsize=12)
    ax.invert_yaxis()  # 排名越小越好，反转y轴
    plt.legend(loc='best', ncol=3, fontsize=10)

    # 标题
    title = f'{student_name} - 全部科目年级排名对比'
    plt.title(title, fontsize=14, fontweight='bold', pad=20)

    plt.tight_layout()

    # 保存
    filename = f'趋势图_{student_name}_全部科目.png'
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    print(f"\n✅ 图表已保存: {filename}")

    plt.show()


def plot_comprehensive_view(conn, student_id, student_name):
    """综合查看：展示所有学科和总分的年级排名变化趋势"""
    # 获取所有学科成绩数据
    cursor = conn.cursor()
    cursor.execute("""
        SELECT
            e.ExamDate,
            e.ExamName,
            sb.SubjectName,
            s.GradeRank,
            sb.SortOrder,
            sb.SubjectId
        FROM Scores s
        JOIN Exams e ON s.ExamId = e.ExamId
        JOIN Subjects sb ON s.SubjectId = sb.SubjectId
        WHERE s.StudentId = ?
        ORDER BY e.ExamDate, sb.SortOrder
    """, (student_id,))

    all_scores = cursor.fetchall()

    if not all_scores:
        print("⚠️  该学生没有成绩记录")
        return

    # 整理数据
    exams = sorted(set(s['ExamDate'] for s in all_scores), key=lambda x: datetime.strptime(x, '%Y-%m-%d'))
    exam_names = {e['ExamDate']: e['ExamName'] for e in all_scores}

    # 获取所有科目并按SortOrder排序
    subject_info = {}
    for s in all_scores:
        subject_name = s['SubjectName']
        if subject_name not in subject_info:
            subject_info[subject_name] = {'SortOrder': s['SortOrder'], 'SubjectId': s['SubjectId']}
    subjects = sorted(subject_info.keys(), key=lambda x: subject_info[x]['SortOrder'])

    # 构建排名矩阵
    rank_matrix = {}
    for subject in subjects:
        rank_matrix[subject] = []
        for exam in exams:
            found = False
            for s in all_scores:
                if s['ExamDate'] == exam and s['SubjectName'] == subject:
                    rank_matrix[subject].append(s['GradeRank'] if s['GradeRank'] else 999)
                    found = True
                    break
            if not found:
                rank_matrix[subject].append(999)  # 999 表示缺考

    # 创建图表
    fig, ax = plt.subplots(figsize=(16, 9))

    # 学科颜色映射
    colors = {
        '语文': '#e74c3c', '数学': '#3498db', '英语': '#2ecc71',
        '物理': '#f39c12', '化学': '#9b59b6', '生物': '#1abc9c',
        '政治': '#e67e22', '历史': '#34495e', '地理': '#7f8c8d',
        '总分': '#c0392b'  # 总分用深红色
    }

    # 绘制各学科排名折线
    for subject in subjects:
        exam_dates = [datetime.strptime(d, '%Y-%m-%d') for d in exams]
        ranks = rank_matrix[subject]
        color = colors.get(subject, '#95a5a6')

        # 只绘制有效数据（非999）
        valid_data = [(x, y) for x, y in zip(exam_dates, ranks) if y != 999]
        if valid_data:
            valid_dates, valid_ranks = zip(*valid_data)
            linewidth = 2.5 if subject == '总分' else 2
            markersize = 8 if subject == '总分' else 6
            linestyle = '--' if subject == '总分' else '-'
            alpha = 0.9 if subject == '总分' else 0.7

            ax.plot(valid_dates, valid_ranks, 'o', color=color,
                    linewidth=linewidth, markersize=markersize,
                    linestyle=linestyle, label=subject, alpha=alpha)

            # 添加排名标签
            for x, y in zip(valid_dates, valid_ranks):
                ax.text(x, y, f'#{y}',
                       ha='center', va='top',
                       fontsize=9 if subject != '总分' else 10,
                       color=color, fontweight='bold')

            # 总分添加变化标记
            if subject == '总分' and len(valid_ranks) > 1:
                for i in range(1, len(valid_ranks)):
                    prev_rank = valid_ranks[i-1]
                    curr_rank = valid_ranks[i]
                    rank_change = prev_rank - curr_rank  # 前一次排名 - 当前排名

                    if rank_change > 0:
                        change_text = f'↑+{rank_change}'
                        change_color = 'green'
                    elif rank_change < 0:
                        change_text = f'↓{rank_change}'
                        change_color = 'red'
                    else:
                        change_text = '→0'
                        change_color = 'gray'

                    date_diff = valid_dates[i] - valid_dates[i-1]
                    mid_x = valid_dates[i-1] + date_diff / 2
                    mid_y = (prev_rank + curr_rank) / 2

                    ax.text(mid_x, mid_y, change_text, ha='center', va='center',
                           fontsize=11, color=change_color, fontweight='bold',
                           bbox=dict(boxstyle='round,pad=0.3', facecolor='white', alpha=0.9))

    # 设置y轴
    ax.set_xlabel('考试时间', fontsize=13)
    ax.set_ylabel('年级排名', fontsize=13)
    ax.invert_yaxis()  # 排名越小越好，反转y轴
    ax.grid(True, alpha=0.3)

    # 设置x轴
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))
    ax.xaxis.set_major_locator(mdates.AutoDateLocator())
    plt.xticks(rotation=45, ha='right')

    # 图例
    ax.legend(loc='best', ncol=3, fontsize=10)

    # 标题
    title = f'{student_name} - 综合成绩分析（年级排名变化）'
    plt.title(title, fontsize=15, fontweight='bold', pad=20)

    plt.tight_layout()

    # 保存图表
    filename = f'综合分析_{student_name}.png'
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    print(f"\n✅ 图表已保存: {filename}")

    # 打印排名变化摘要
    print_rank_change_summary(exams, exam_names, rank_matrix.get('总分', []))

    plt.show()


def print_rank_change_summary(exams, exam_names, grade_ranks):
    """打印总分排名变化摘要"""
    if not grade_ranks or all(r == 999 for r in grade_ranks):
        print("\n⚠️  该学生没有总分排名记录")
        return

    valid_data = [(i, r) for i, r in enumerate(grade_ranks) if r != 999]
    if len(valid_data) < 1:
        return

    print(f"\n{'='*70}")
    print(f"📊 总分排名变化分析")
    print(f"{'='*70}")
    print(f"{'考试名称':<20} {'考试日期':<12} {'总分排名':<10} {'排名变化':<15} {'趋势'}")
    print(f"{'-'*70}")

    for i, r in valid_data:
        exam_date = exams[i]
        exam_name = exam_names.get(exam_date, '-')
        rank_str = f'#{r}'

        if i == 0 or grade_ranks[i-1] == 999:
            change_str = '-'
            trend = '首次考试'
        else:
            prev_rank = grade_ranks[i-1]
            rank_change = prev_rank - r

            if rank_change > 0:
                change_str = f'+{rank_change}名'
                trend = '↑ 进步'
            elif rank_change < 0:
                change_str = f'{rank_change}名'
                trend = '↓ 退步'
            else:
                change_str = '0名'
                trend = '→ 持平'

        print(f"{exam_name:<20} {exam_date:<12} {rank_str:<10} {change_str:<15} {trend}")

    print(f"{'='*70}\n")


if __name__ == '__main__':
    main()
