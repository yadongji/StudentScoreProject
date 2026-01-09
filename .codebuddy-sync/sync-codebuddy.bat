@echo off
echo ========================================
echo   CodeBuddy 聊天记录同步脚本
echo ========================================
echo.

set CODEBUDDY_PATH=%APPDATA%\CodeBuddy CN\User
set SYNC_PATH=%~dp0.codebuddy-sync

echo [1/3] 检查 CodeBuddy 路径...
if not exist "%CODEBUDDY_PATH%" (
    echo ❌ 错误：未找到 CodeBuddy 目录
    echo 路径：%CODEBUDDY_PATH%
    pause
    exit /b 1
)
echo ✅ 找到 CodeBuddy 目录

echo.
echo [2/3] 从本地复制到同步文件夹...
xcopy "%CODEBUDDY_PATH%\History" "%SYNC_PATH%\History\" /E /I /D /Y
copy "%CODEBUDDY_PATH%\settings.json" "%SYNC_PATH%\" /Y >nul 2>&1
xcopy "%CODEBUDDY_PATH%\globalStorage" "%SYNC_PATH%\globalStorage\" /E /I /D /Y >nul 2>&1
echo ✅ 同步完成

echo.
echo [3/3] Git 提交说明...
echo 请在项目根目录执行以下命令提交到 Git：
echo.
echo    git add .codebuddy-sync/
echo    git commit -m "Sync CodeBuddy chat history"
echo    git push
echo.

pause
