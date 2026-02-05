@echo off
chcp 65001 >nul
echo ========================================
echo 宿舍管理系统 - 测试
echo ========================================
echo.
echo 此脚本将测试数据库初始化功能
echo.
pause
echo.
echo 测试1: 检查Python环境...
python --version
if errorlevel 1 (
    echo ❌ Python未安装或未配置环境变量
    pause
    exit /b 1
)
echo ✅ Python环境正常
echo.

echo 测试2: 检查SQLite3模块...
python -c "import sqlite3; print('SQLite3版本:', sqlite3.sqlite_version)"
if errorlevel 1 (
    echo ❌ SQLite3模块未安装
    pause
    exit /b 1
)
echo ✅ SQLite3模块正常
echo.

echo 测试3: 检查数据库文件...
if exist "StudentData.db" (
    echo ⚠️ 数据库文件已存在，将创建测试副本
    copy StudentData.db test_backup.db >nul
)

echo 测试4: 测试数据库初始化...
python -c "import sqlite3; conn = sqlite3.connect('test_init.db'); cursor = conn.cursor(); cursor.executescript(open('database_schema_dormitory_leave.sql', 'r', encoding='utf-8').read()); conn.commit(); tables = cursor.execute('SELECT name FROM sqlite_master WHERE type=\"table\" AND name NOT LIKE \"sqlite_%\"').fetchall(); views = cursor.execute('SELECT name FROM sqlite_master WHERE type=\"view\"').fetchall(); triggers = cursor.execute('SELECT name FROM sqlite_master WHERE type=\"trigger\"').fetchall(); print('✅ 测试通过！'); print(f'   表: {len(tables)}个, 视图: {len(views)}个, 触发器: {len(triggers)}个'); conn.close(); import os; os.remove('test_init.db')"
if errorlevel 1 (
    echo ❌ 数据库初始化失败
    pause
    exit /b 1
)
echo.

echo ========================================
echo ✅ 所有测试通过！
echo ========================================
echo.
echo 系统已准备就绪，请按以下步骤操作：
echo.
echo 1. 运行: 宿舍管理系统.bat
echo 2. 选择: 1. 初始化数据库表
echo 3. 运行: 快速初始化宿舍数据.bat
echo.
pause
