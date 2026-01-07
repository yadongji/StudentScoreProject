@echo off
echo ========================================
echo   CodeBuddy 聊天记录导入脚本
echo ========================================
echo.

set CODEBUDDY_PATH=%APPDATA%\CodeBuddy CN\User
set SYNC_PATH=%~dp0.codebuddy-sync

echo [1/4] 检查同步文件夹...
if not exist "%SYNC_PATH%\History" (
    echo ❌ 错误：同步文件夹中没有聊天记录
    echo 请先运行 git pull 拉取最新数据
    pause
    exit /b 1
)
echo ✅ 找到同步文件夹

echo.
echo [2/4] 检查 CodeBuddy 路径...
if not exist "%CODEBUDDY_PATH%" (
    echo ❌ 错误：未找到 CodeBuddy 目录
    echo 路径：%CODEBUDDY_PATH%
    pause
    exit /b 1
)
echo ✅ 找到 CodeBuddy 目录

echo.
echo [3/4] 备份当前数据...
set BACKUP_PATH=%CODEBUDDY_PATH%.backup.%date:~0,4%%date:~5,2%%date:~8,2%
xcopy "%CODEBUDDY_PATH%" "%BACKUP_PATH%\" /E /I /Y >nul
echo ✅ 备份完成：%BACKUP_PATH%

echo.
echo [4/4] 从同步文件夹导入到本地...
xcopy "%SYNC_PATH%\History\*" "%CODEBUDDY_PATH%\History\" /E /I /Y
copy "%SYNC_PATH%\settings.json" "%CODEBUDDY_PATH%\" /Y >nul 2>&1
xcopy "%SYNC_PATH%\globalStorage\*" "%CODEBUDDY_PATH%\globalStorage\" /E /I /Y >nul 2>&1
echo ✅ 导入完成

echo.
echo ========================================
echo   导入完成！请重启 CodeBuddy
echo ========================================
echo.
echo 如需恢复备份，请执行：
echo    xcopy "%BACKUP_PATH%\*" "%CODEBUDDY_PATH%\" /E /I /Y
echo.

pause
