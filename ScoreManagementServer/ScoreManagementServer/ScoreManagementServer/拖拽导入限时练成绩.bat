@echo off
chcp 65001 > nul
python time_limit_exam_importer.py %1
pause
