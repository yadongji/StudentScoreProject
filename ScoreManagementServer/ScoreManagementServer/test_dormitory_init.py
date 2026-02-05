"""测试数据库初始化"""
import sqlite3
import os

def test_init():
    db_path = "test_dormitory.db"

    # 删除旧的测试数据库
    if os.path.exists(db_path):
        os.remove(db_path)

    # 创建新的测试数据库
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()

    print("="*60)
    print("测试宿舍管理数据库初始化")
    print("="*60)

    # 读取并执行SQL脚本
    try:
        with open('database_schema_dormitory_leave.sql', 'r', encoding='utf-8') as f:
            sql_script = f.read()

        print("\n正在执行SQL脚本...")
        cursor.executescript(sql_script)
        conn.commit()
        print("✅ SQL脚本执行成功！")

        # 检查表是否创建成功
        cursor.execute("""
            SELECT name FROM sqlite_master
            WHERE type='table' AND name NOT LIKE 'sqlite_%'
            ORDER BY name
        """)
        tables = cursor.fetchall()
        print(f"\n✅ 成功创建 {len(tables)} 个表：")
        for table in tables:
            print(f"   - {table[0]}")

        # 检查视图是否创建成功
        cursor.execute("""
            SELECT name FROM sqlite_master
            WHERE type='view'
            ORDER BY name
        """)
        views = cursor.fetchall()
        print(f"\n✅ 成功创建 {len(views)} 个视图：")
        for view in views:
            print(f"   - {view[0]}")

        # 检查触发器是否创建成功
        cursor.execute("""
            SELECT name FROM sqlite_master
            WHERE type='trigger'
            ORDER BY name
        """)
        triggers = cursor.fetchall()
        print(f"\n✅ 成功创建 {len(triggers)} 个触发器：")
        for trigger in triggers:
            print(f"   - {trigger[0]}")

        # 检查初始数据
        cursor.execute("SELECT COUNT(*) FROM Dormitories")
        dorm_count = cursor.fetchone()[0]
        cursor.execute("SELECT COUNT(*) FROM DormitoryStaff")
        staff_count = cursor.fetchone()[0]
        cursor.execute("SELECT COUNT(*) FROM HeadTeachers")
        teacher_count = cursor.fetchone()[0]

        print(f"\n✅ 初始数据：")
        print(f"   - 宿舍楼: {dorm_count} 栋")
        print(f"   - 宿管人员: {staff_count} 人")
        print(f"   - 班主任: {teacher_count} 人")

        print("\n" + "="*60)
        print("✅ 数据库初始化测试完成！")
        print("="*60)

    except Exception as e:
        print(f"\n❌ 错误: {e}")
        import traceback
        traceback.print_exc()

    finally:
        conn.close()

if __name__ == "__main__":
    test_init()
