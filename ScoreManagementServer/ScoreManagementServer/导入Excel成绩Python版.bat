@echo off
chcp 65001 >nul
cd /d "%~dp0ScoreManagementServer"

echo ========================================
echo 高中成绩管理系统 - Excel导入工具
echo ========================================
echo.

echo 检查 Python 环境...
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 未检测到 Python!
    echo 请从 https://www.python.org/downloads/ 下载安装
    echo.
    echo 安装时请勾选 "Add Python to PATH"
    pause
    exit /b 1
)

echo ✅ Python 环境正常
echo.

echo 检查 openpyxl 库...
python -c "import openpyxl" >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 缺少 openpyxl 库
    echo 正在安装 openpyxl...
    pip install openpyxl
    if %errorlevel% neq 0 (
        echo ❌ 安装失败,请手动运行: pip install openpyxl
        pause
        exit /b 1
    )
)

echo ✅ 依赖库已就绪
echo.
echo ========================================
echo 正在启动导入工具...
echo ========================================
echo.

python ../excel_to_sqlite.py

pause
