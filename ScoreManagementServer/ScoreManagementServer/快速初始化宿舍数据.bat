@echo off
chcp 65001 >nul
echo ========================================
echo 宿舍管理系统 - 快速初始化
echo ========================================
echo.
echo 此脚本将快速创建示例数据：
echo - 2栋宿舍楼（5号楼、6号楼）
echo - 5个宿管人员
echo - 120个房间（每栋楼6层，每层10间）
echo - 480个床位（每间房4个床位）
echo - 4个班主任
echo.
pause
echo.
python quick_init_dormitory.py
pause
