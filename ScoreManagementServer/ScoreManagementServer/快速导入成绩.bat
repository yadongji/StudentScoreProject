@echo off
cd /d "%~dp0"

echo ========================================
echo 高中成绩管理系统 - 快速启动
echo ========================================
echo.

echo 正在检查 .NET 环境...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [错误] 未安装 .NET SDK!
    echo 请从 https://dotnet.microsoft.com/download 下载安装
    pause
    exit /b 1
)

echo .NET 环境正常!
echo.

echo 正在编译导入工具...
dotnet build SimpleImportTool.cs -o bin 2>nul
if %errorlevel% neq 0 (
    echo [错误] 编译失败!
    echo 请确保已安装必要的 NuGet 包:
    echo   - DocumentFormat.OpenXml
    echo   - Microsoft.Data.Sqlite
    echo   - Dapper
    pause
    exit /b 1
)

echo 编译成功!
echo.

echo 正在运行导入工具...
dotnet bin/SimpleImportTool.dll

pause
