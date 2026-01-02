@echo off
chcp 65001 > nul
echo ========================================
echo    成绩趋势可视化工具
echo ========================================
echo.

cd /d "%~dp0"

REM 检查Python
python --version > nul 2>&1
if errorlevel 1 (
    echo ❌ 未检测到Python，请先安装Python
    pause
    exit /b 1
)

REM 检查依赖
echo 检查依赖...
python -c "import matplotlib" > nul 2>&1
if errorlevel 1 (
    echo 安装matplotlib...
    pip install matplotlib
)

python -c "import numpy" > nul 2>&1
if errorlevel 1 (
    echo 安装numpy...
    pip install numpy
)

echo.
echo ✅ 启动可视化工具...
echo.
python score_trend_visualizer.py

pause
