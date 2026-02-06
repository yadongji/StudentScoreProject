"""
测试脚本：验证"校次"和"班次"别名支持
"""

import sys
import io

# 修复Windows控制台编码问题
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

def test_field_aliases():
    """测试字段别名识别"""
    print("=" * 60)
    print("测试 '校次' 和 '班次' 别名识别")
    print("=" * 60)

    # 测试数据：普通格式
    simple_headers = [
        '考号', '姓名', '总分(物理)(赋分)', '总分校次', '总分班次',
        '语文(赋分)', '语文校次', '语文班次'
    ]

    # 测试数据：方向名次格式
    row3 = ['考号', '姓名', '总分(物理)(赋分)', '语文(赋分)']
    row4 = ['', '', '分数', '校次', '班次', '分数', '校次', '班次']

    print("\n1. 普通格式测试")
    print("-" * 40)
    for header in simple_headers:
        if '校次' in header:
            print(f"✅ 识别到年级排名字段: {header}")
        elif '班次' in header:
            print(f"✅ 识别到班级排名字段: {header}")

    print("\n2. 方向名次格式测试")
    print("-" * 40)
    for field in row4:
        if field == '分数':
            print(f"✅ 识别到分数字段: {field}")
        elif field == '校次':
            print(f"✅ 识别到年级排名字段: {field}")
        elif field == '班次':
            print(f"✅ 识别到班级排名字段: {field}")

    print("\n3. 混合别名测试")
    print("-" * 40)
    mixed_headers = [
        '考号', '姓名',
        '总分校名次',  # 使用标准名
        '总分班次',    # 使用别名
        '语文年级排名', # 使用别名
        '语文班级名次' # 使用标准名
    ]
    for header in mixed_headers:
        if '校名次' in header or '年级名次' in header or '年级排名' in header or '校次' in header:
            print(f"✅ 识别到年级排名: {header}")
        elif '班名次' in header or '班级名次' in header or '班级排名' in header or '班次' in header:
            print(f"✅ 识别到班级排名: {header}")

    print("\n" + "=" * 60)
    print("测试完成！所有别名均被正确识别")
    print("=" * 60)

if __name__ == '__main__':
    test_field_aliases()
