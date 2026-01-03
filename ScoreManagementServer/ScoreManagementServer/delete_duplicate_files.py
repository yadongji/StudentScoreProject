# -*- coding: utf-8 -*-
"""
批量删除重复文件
删除所有带（1）后缀的文件和文件夹
"""

import os
import shutil

# 设置目录路径
base_dir = r'D:\高中物理+教案全集'

# 递归查找所有带（1）后缀的文件和文件夹
duplicate_items = []
for root, dirs, files in os.walk(base_dir):
    # 检查文件
    for file in files:
        if '（1）' in file or '(1)' in file:
            duplicate_items.append(os.path.join(root, file))
    
    # 检查文件夹
    for dir_name in dirs:
        if '（1）' in dir_name or '(1)' in dir_name:
            duplicate_items.append(os.path.join(root, dir_name))

print(f"找到 {len(duplicate_items)} 个重复项目（文件和文件夹）")

# 删除项目
deleted_count = 0
for item_path in duplicate_items:
    try:
        if os.path.isfile(item_path):
            os.remove(item_path)
            print(f"已删除文件: {os.path.relpath(item_path, base_dir)}")
            deleted_count += 1
        elif os.path.isdir(item_path):
            shutil.rmtree(item_path)
            print(f"已删除文件夹: {os.path.relpath(item_path, base_dir)}")
            deleted_count += 1
    except Exception as e:
        print(f"删除失败: {item_path}")
        print(f"错误: {e}")

print(f"\n删除完成！共删除 {deleted_count} 个项目")
