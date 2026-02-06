# -*- coding: utf-8 -*-
import sys
import io
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

headers = ['班级', '姓名', '总分', '学校名次', '班级名次', '语文', '班次', '校次']

print("表头:")
for idx, h in enumerate(headers, 1):
    print(f"  列{idx}: {h}")

print("\n测试:")
for idx, h in enumerate(headers):
    if h == '班级':
        print(f"  找到班级列: {idx}")
    elif h == '姓名':
        print(f"  找到姓名列: {idx}")
    elif h == '总分':
        print(f"  找到总分列: {idx}")

# 测试相邻列判断
for idx, h in enumerate(headers):
    if h == '语文':
        print(f"  语文列在位置: {idx}")
        # 查找相邻的班次和校次
        if idx + 1 < len(headers) and headers[idx + 1] == '班次':
            print(f"    语文班次在位置: {idx + 1}")
        if idx + 2 < len(headers) and headers[idx + 2] == '校次':
            print(f"    语文校次在位置: {idx + 2}")
