# CodeBuddy 聊天记录同步文件夹

此文件夹用于在不同电脑之间同步 CodeBuddy 的聊天记录。

## 如何使用

### 1. 从 CodeBuddy 导出聊天记录（源电脑）

聊天记录实际存储在：
```
C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\History\
```

执行以下命令复制聊天记录到此文件夹：
```bash
# 复制所有聊天历史
xcopy "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\History" ".\codebuddy-sync\History\" /E /I /Y

# 复制用户设置（可选）
xcopy "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\settings.json" ".\codebuddy-sync\" /Y

# 复制全局存储（可选）
xcopy "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\globalStorage" ".\codebuddy-sync\globalStorage\" /E /I /Y
```

### 2. 提交到 Git
```bash
git add .codebuddy-sync/
git commit -m "Sync CodeBuddy chat history"
git push
```

### 3. 在其他电脑导入聊天记录

```bash
# 从 Git 拉取最新聊天记录
git pull

# 导入到 CodeBuddy（请先备份原数据）
xcopy ".\codebuddy-sync\History\*" "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\History\" /E /I /Y

# 导入设置（可选）
copy ".\codebuddy-sync\settings.json" "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\"

# 导入全局存储（可选）
xcopy ".\codebuddy-sync\globalStorage\*" "C:\Users\你的用户名\AppData\Roaming\CodeBuddy CN\User\globalStorage\" /E /I /Y
```

### 4. 重启 CodeBuddy

完成导入后，重启 CodeBuddy 即可看到同步的聊天记录。

## 注意事项

⚠️ **重要提示**：
1. 在导入聊天记录前，建议先备份原有的 CodeBuddy 数据
2. 聊天记录文件使用哈希值命名（如 `-221dc037`），这是正常现象
3. 如果多台电脑同时使用 CodeBuddy，可能会导致聊天记录冲突，建议定期同步
4. 不要在 CodeBuddy 运行时复制/替换聊天记录文件

## 自动同步脚本（可选）

### 同步脚本 sync-codebuddy.bat
```batch
@echo off
echo 正在同步 CodeBuddy 聊天记录...
set CODEBUDDY_PATH=%APPDATA%\CodeBuddy CN\User
set SYNC_PATH=%~dp0.codebuddy-sync

echo 从本地复制到 Git...
xcopy "%CODEBUDDY_PATH%\History" "%SYNC_PATH%\History\" /E /I /D /Y
copy "%CODEBUDDY_PATH%\settings.json" "%SYNC_PATH%\" /Y
xcopy "%CODEBUDDY_PATH%\globalStorage" "%SYNC_PATH%\globalStorage\" /E /I /D /Y

echo 同步完成！
echo 请运行以下命令提交到 Git：
echo git add .codebuddy-sync/
echo git commit -m "Sync CodeBuddy chat history"
echo git push
pause
```

### 导入脚本 import-codebuddy.bat
```batch
@echo off
echo 正在导入 CodeBuddy 聊天记录...
set CODEBUDDY_PATH=%APPDATA%\CodeBuddy CN\User
set SYNC_PATH=%~dp0.codebuddy-sync

echo ⚠️ 警告：这将会覆盖当前的 CodeBuddy 数据！
echo 是否继续？(Y/N)
set /p confirm=
if /i not "%confirm%"=="Y" exit /b

echo 备份当前数据...
xcopy "%CODEBUDDY_PATH%" "%CODEBUDDY_PATH%.backup\" /E /I /Y

echo 从 Git 导入到本地...
xcopy "%SYNC_PATH%\History\*" "%CODEBUDDY_PATH%\History\" /E /I /Y
copy "%SYNC_PATH%\settings.json" "%CODEBUDDY_PATH%\" /Y
xcopy "%SYNC_PATH%\globalStorage\*" "%CODEBUDDY_PATH%\globalStorage\" /E /I /Y

echo 导入完成！请重启 CodeBuddy。
pause
```
