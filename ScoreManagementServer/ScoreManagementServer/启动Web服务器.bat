@echo off
chcp 65001 >nul
echo ========================================
echo 宿舍管理与请假管理系统 - Web版
echo ========================================
echo.

echo 📦 检查Flask是否已安装...
python -c "import flask" 2>nul
if errorlevel 1 (
    echo ⚠️  Flask未安装，正在安装...
    pip install Flask
    if errorlevel 1 (
        echo ❌ Flask安装失败
        pause
        exit /b 1
    )
    echo ✅ Flask安装成功
    echo.
)

echo.
echo 🌐 正在启动Web服务器...
echo.
echo ========================================
echo 访问地址：
echo   📱 电脑访问：http://127.0.0.1:5000
echo   📱 手机访问：http://[本机IP]:5000
echo   💡 手机访问请确保在同一WiFi网络下
echo ========================================
echo.
echo 按 Ctrl+C 停止服务器
echo.
python app.py
pause
