# -*- coding: utf-8 -*-
"""
直接测试导入108班成绩表
"""
import sys
import io
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# 导入主程序中的检测函数
sys.path.insert(0, '.')
from excel_to_sqlite_v2 import detect_sheet_columns_simple

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

print("测试修复后的列检测逻辑")
print("=" * 80)

print("\n表头:")
for idx, h in enumerate(headers, 1):
    print(f"  列{idx:2d}: {h}")

print("\n检测结果:")
col_map = detect_sheet_columns_simple(headers)

# 显示所有映射
print("\n字段映射:")
for key in sorted(col_map.keys()):
    print(f"  {key}: 列{col_map[key] + 1}")

# 验证关键字段
print("\n验证:")
errors = 0

if '班级' in col_map and col_map['班级'] == 0:
    print("  正确: 班级列 = 列1")
else:
    print(f"  错误: 班级列应为列1")
    errors += 1

if '姓名' in col_map and col_map['姓名'] == 1:
    print("  正确: 姓名列 = 列2")
else:
    print(f"  错误: 姓名列应为列2")
    errors += 1

if '总分_score' in col_map and col_map['总分_score'] == 2:
    print("  正确: 总分成绩列 = 列3")
else:
    print(f"  错误: 总分成绩列应为列3")
    errors += 1

# 验证学科排名
if '语文_class_rank' in col_map and col_map['语文_class_rank'] == 6:
    print("  正确: 语文班次列 = 列7")
else:
    print(f"  错误: 语文班次列应为列7")
    errors += 1

if '语文_grade_rank' in col_map and col_map['语文_grade_rank'] == 7:
    print("  正确: 语文校次列 = 列8")
else:
    print(f"  错误: 语文校次列应为列8")
    errors += 1

print("\n" + "=" * 80)
if errors == 0:
    print("测试通过！")
else:
    print(f"测试失败！发现 {errors} 个错误")
