#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""检查数据库中是否有总分科目（SubjectId=10）"""
import sqlite3
import os

DB_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "StudentData.db")

conn = sqlite3.connect(DB_PATH)
cursor = conn.cursor()

print("=" * 80)
print("检查总分科目（SubjectId=10）")
print("=" * 80)

print("\n检查Subjects表中的所有科目:")
cursor.execute("SELECT * FROM Subjects ORDER BY SubjectId")
subjects = cursor.fetchall()
for sub in subjects:
    print(f"  SubjectId={sub[0]}, SubjectName={sub[1]}, SubjectCode={sub[2] if len(sub) > 2 else 'N/A'}")

print("\n检查SubjectId=10是否存在:")
cursor.execute("SELECT * FROM Subjects WHERE SubjectId = ?", (10,))
total_subject = cursor.fetchone()

if total_subject:
    print(f"✅ 找到SubjectId=10的科目:")
    print(f"  SubjectId={total_subject[0]}, SubjectName={total_subject[1]}, SubjectCode={total_subject[2] if len(total_subject) > 2 else 'N/A'}")

    # 检查科目名称是否为'总分'
    if total_subject[1] != '总分':
        print(f"\n⚠️  警告: SubjectId=10的科目名称是'{total_subject[1]}'，应该是'总分'")
        update = input("是否更新为'总分'? (y/N): ").strip().lower()
        if update == 'y':
            cursor.execute("UPDATE Subjects SET SubjectName = '总分' WHERE SubjectId = ?", (10,))
            conn.commit()
            print("✅ 已更新为'总分'")
else:
    print("❌ 未找到SubjectId=10的科目，准备添加...")

    # 添加总分科目（SubjectId固定为10）
    cursor.execute("""
        INSERT INTO Subjects (SubjectId, SubjectName, SubjectCode)
        VALUES (10, '总分', 'TOTAL')
    """)
    conn.commit()
    print("✅ 已添加'总分'科目 (SubjectId=10)")

conn.close()

print("\n" + "=" * 80)
print("✅ 检查完成")
print("=" * 80)
