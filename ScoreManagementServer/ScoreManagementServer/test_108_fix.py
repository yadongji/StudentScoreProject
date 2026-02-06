"""
测试修复后的列检测逻辑
"""

import sys
import io

# 修复Windows控制台编码问题
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# 复制修复后的列检测函数
def detect_sheet_columns_simple_fixed(header_row):
    """
    普通格式的列检测（单行表头）- 修复版本

    Excel结构（普通格式）：
    - 第1行：单行表头（如"学校"、"班级"、"学号"、"姓名"、"语文"、"班次"、"校次"等）
    - 第2行开始：实际数据
    """
    col_map = {}

    # 1. 检测基础信息列（前5列）- 优先精确匹配完整字段名
    for idx, header in enumerate(header_row):
        if not header:
            continue
        # 优先精确匹配完整字段名（避免被复合字段名误匹配）
        if header == '学号' or header == '考号':
            col_map['学号'] = idx
        elif header == '姓名':
            col_map['姓名'] = idx
        elif header == '班级':
            col_map['班级'] = idx
        elif header == '学校':
            col_map['学校'] = idx

    # 2. 如果精确匹配失败，使用包含匹配（向后兼容）
    if '学号' not in col_map:
        for idx, header in enumerate(header_row):
            if header and ('学号' in header or '考号' in header):
                col_map['学号'] = idx
                break
    if '姓名' not in col_map:
        for idx, header in enumerate(header_row):
            if header and '姓名' == header:  # 只精确匹配
                col_map['姓名'] = idx
                break
    if '班级' not in col_map:
        for idx, header in enumerate(header_row):
            if header and '班级' == header:  # 只精确匹配
                col_map['班级'] = idx
                break
    if '学校' not in col_map:
        for idx, header in enumerate(header_row):
            if header and '学校' == header:  # 只精确匹配
                col_map['学校'] = idx
                break

    # 2. 检测各学科列 - 使用相邻列判断
    subjects = ['总分', '语文', '数学', '英语', '物理', '化学', '生物', '政治', '历史', '地理']

    # 第一步：找到所有学科的成绩列
    subject_score_cols = {}  # {学科名: 成绩列索引}
    for idx, header in enumerate(header_row):
        if not header:
            continue

        for subject in subjects:
            # 精确匹配或包含学科名
            if header == subject or (subject in header and not ('班次' in header or '校次' in header or '名次' in header or '排名' in header)):
                subject_score_cols[subject] = idx
                col_map[f'{subject}_score'] = idx
                break

    # 第二步：基于成绩列位置，查找相邻的"班次"和"校次"列
    for subject, score_col in subject_score_cols.items():
        # 查找成绩列后面的"班次"和"校次"
        # 格式：成绩列, 班次, 校次
        for offset in range(1, 4):  # 检查后面3列
            col_idx = score_col + offset
            if col_idx >= len(header_row):
                break

            header = header_row[col_idx]
            if not header:
                continue

            # 判断是否为班次列
            if header == '班次' or header == '班级名次' or header == '班级排名':
                if f'{subject}_class_rank' not in col_map:
                    col_map[f'{subject}_class_rank'] = col_idx

            # 判断是否为校次列
            elif header == '校次' or header == '校名次' or header == '年级名次' or header == '年级排名':
                if f'{subject}_grade_rank' not in col_map:
                    col_map[f'{subject}_grade_rank'] = col_idx

            # 遇到新的学科列就停止
            if header in subjects:
                break

    return col_map


def test_108_class_headers():
    """测试108班成绩统计表的表头"""
    print("=" * 80)
    print("测试修复后的列检测逻辑")
    print("=" * 80)

    # 108班成绩统计表的表头
    headers = [
        '班级', '姓名', '总分', '学校名次', '班级名次',
        '语文', '班次', '校次',
        '数学', '班次', '校次',
        '英语', '班次', '校次',
        '物理', '班次', '校次',
        '化学', '班次', '校次',
        '生物', '班次', '校次'
    ]

    print("\n表头：")
    for idx, header in enumerate(headers, 1):
        print(f"  列{idx:2d}: {header}")

    print("\n检测结果：")
    print("-" * 80)

    col_map = detect_sheet_columns_simple_fixed(headers)

    # 显示基础信息列
    print("\n基础信息列：")
    if '班级' in col_map:
        print(f"  班级: 列{col_map['班级'] + 1}")
    if '姓名' in col_map:
        print(f"  姓名: 列{col_map['姓名'] + 1}")
    if '学校' in col_map:
        print(f"  学校: 列{col_map['学校'] + 1}")
    if '学号' in col_map:
        print(f"  学号: 列{col_map['学号'] + 1}")

    # 显示学科成绩列
    print("\n学科成绩列：")
    subjects = ['总分', '语文', '数学', '英语', '物理', '化学', '生物']
    for subject in subjects:
        if f'{subject}_score' in col_map:
            score_col = col_map[f'{subject}_score']
            print(f"  {subject}_score: 列{score_col + 1}")

    # 显示学科排名列
    print("\n学科排名列：")
    for subject in subjects:
        if f'{subject}_class_rank' in col_map:
            class_rank_col = col_map[f'{subject}_class_rank']
            print(f"  {subject}_class_rank: 列{class_rank_col + 1}")
        if f'{subject}_grade_rank' in col_map:
            grade_rank_col = col_map[f'{subject}_grade_rank']
            print(f"  {subject}_grade_rank: 列{grade_rank_col + 1}")

    # 验证
    print("\n验证：")
    print("-" * 80)

    errors = 0

    # 验证基础信息列
    if '班级' not in col_map or col_map['班级'] != 0:
        print(f"  错误: 班级列应为列1，实际为列{col_map.get('班级', -1) + 1}")
        errors += 1
    else:
        print(f"  正确: 班级列 = 列1")

    if '姓名' not in col_map or col_map['姓名'] != 1:
        print(f"  错误: 姓名列应为列2，实际为列{col_map.get('姓名', -1) + 1}")
        errors += 1
    else:
        print(f"  正确: 姓名列 = 列2")

    if '总分_score' not in col_map or col_map['总分_score'] != 2:
        print(f"  错误: 总分成绩列应为列3，实际为列{col_map.get('总分_score', -1) + 1}")
        errors += 1
    else:
        print(f"  正确: 总分成绩列 = 列3")

    # 验证学科排名列
    rank_tests = [
        ('语文', 6, 7, '语文'),
        ('数学', 9, 10, '数学'),
        ('英语', 12, 13, '英语'),
        ('物理', 15, 16, '物理'),
        ('化学', 18, 19, '化学'),
        ('生物', 21, 22, '生物'),
    ]

    for subject, expected_class, expected_grade, subject_name in rank_tests:
        class_rank = col_map.get(f'{subject}_class_rank', -1)
        grade_rank = col_map.get(f'{subject}_grade_rank', -1)

        if class_rank != expected_class - 1:
            print(f"  错误: {subject_name}班次列应为列{expected_class}，实际为列{class_rank + 1}")
            errors += 1
        else:
            print(f"  正确: {subject_name}班次列 = 列{expected_class}")

        if grade_rank != expected_grade - 1:
            print(f"  错误: {subject_name}校次列应为列{expected_grade}，实际为列{grade_rank + 1}")
            errors += 1
        else:
            print(f"  正确: {subject_name}校次列 = 列{expected_grade}")

    print("\n" + "=" * 80)
    if errors == 0:
        print("测试通过！所有字段识别正确")
    else:
        print(f"测试失败！发现 {errors} 个错误")
    print("=" * 80)

    return errors == 0


if __name__ == '__main__':
    success = test_108_class_headers()
    sys.exit(0 if success else 1)
