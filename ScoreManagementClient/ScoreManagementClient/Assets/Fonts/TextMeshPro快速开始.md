# 快速开始指南 - TextMeshPro中文字体配置

## 🚀 5分钟快速配置

### 第1步：下载字体（1分钟）

**方式A：推荐链接下载**
```
下载地址：https://www.maoken.com/freefonts/16864.html
下载文件：霞鹜文楷GB_1.521_猫啃网.zip
```

**方式B：GitHub下载**
```
下载地址：https://github.com/lxgw/LxgwWenkaiGB/releases/tag/v1.521
下载文件：LXGWWenKaiGB-Regular.ttf
```

---

### 第2步：导入字体到Unity（30秒）

1. 解压下载的压缩包
2. 复制 `LXGWWenKaiGB-Regular.ttf` 到项目目录：
   ```
   ScoreManagementClient/Assets/Fonts/
   ```

---

### 第3步：创建TextMeshPro Font Asset（2分钟）

1. **打开Unity编辑器**
2. **等待字体导入完成**（Unity会自动导入）
3. **在Project窗口中找到导入的字体**
4. **右键字体文件** → **Create** → **TextMeshPro** → **Font Asset**
5. **在弹出的Font Asset Creator窗口中**：
   ```
   Source Font File: [已自动选择你的字体]
   Font Name: CN_Regular
   Sampling Point Size: 36
   Character Set: Custom
   Unicode Hex Range: 4E00-9FFF (添加这个范围)
   Atlas Resolution: 1024 x 1024
   ```
6. **点击 Generate Font Asset**

---

### 第4步：重命名Font Asset（10秒）

1. 在Project窗口中找到生成的Font Asset
2. 重命名为：`CN_Regular_TMP`

---

### 第5步：配置使用字体（30秒）

**方式A：使用编辑器菜单（推荐）**
```
Unity菜单 → Tools → Font Manager → 配置当前场景所有TMP_Text使用CN_Regular字体
```

**方式B：手动配置**
1. 在Hierarchy中选择包含TMP_Text的GameObject
2. 在Inspector中找到TMP_Text组件
3. 在Font Asset字段中拖入 `CN_Regular_TMP`

---

### 第6步：验证配置（10秒）

1. 运行场景
2. 检查中文文本是否正确显示
3. ✅ 完成！

---

## 📋 字符集推荐设置

### 完整中文支持
```
Character Set: Custom
Unicode Hex Range: 4E00-9FFF
```

### 仅常用汉字（节省空间）
```
Character Set: Custom
Unicode Hex Range: 
- 4E00-4DFF (常用字)
- 4E00-62FF (一级字)
```

### 自定义字符（精确控制）
```
Character Set: Custom Sequence
Character Sequence: 直接输入需要包含的字符
```

---

## ⚙️ Font Asset Creator关键参数

| 参数 | 推荐值 | 说明 |
|------|--------|------|
| Sampling Point Size | 36 | 字体质量，数值越大质量越高但文件越大 |
| Atlas Resolution | 1024x1024 | 图集大小，影响包含的字符数量 |
| Atlas Padding | 9 | 字符间距，默认即可 |
| Atlas Render Mode | SDF | 渲染模式，推荐SDF |
| Character Set | Custom | 字符集，选择Custom可包含中文 |

---

## 🔧 常见问题速查

**问题：中文字符显示为方块**
```
解决方案：
1. 确认Character Set选择了Custom
2. 确认添加了Unicode Hex Range: 4E00-9FFF
3. 重新生成Font Asset
```

**问题：字体显示模糊**
```
解决方案：
1. 增加Sampling Point Size到72
2. 提高Atlas Resolution到2048x2048
3. 重新生成Font Asset
```

**问题：Font Asset文件太大**
```
解决方案：
1. 减少字符集范围
2. 降低Atlas Resolution到512x512
3. 重新生成Font Asset
```

**问题：找不到Create -> TextMeshPro菜单**
```
解决方案：
1. 确认TextMeshPro包已导入
2. Window -> Package Manager -> TextMeshPro -> Install
3. 导入完成后重新尝试
```

---

## 🎯 进阶配置

### 创建粗体字体

1. 重复上述步骤，但使用粗体字体文件（如果有）
2. Font Name设置为：CN_Bold
3. 重命名Font Asset为：CN_Bold_TMP

### 创建多个Font Asset

为了优化性能，可以为不同场景创建不同的Font Asset：

```
登录场景Font Asset: 包含常用字符
主界面Font Asset: 包含界面字符
游戏场景Font Asset: 包含游戏专用字符
```

### 使用FontManager自动配置

1. 创建空GameObject，命名为"FontManager"
2. 添加FontManager组件
3. 拖入CN_Regular_TMP到对应字段
4. 勾选"Auto Configure On Start"
5. 场景启动时自动配置所有TMP_Text

---

## 📚 详细文档

如需更详细的配置说明，请查看：
```
Assets/Fonts/字体配置说明.md
```

---

## ✅ 配置检查清单

- [ ] 已下载霞鹜文楷GB字体文件
- [ ] 已导入字体到Unity项目的Fonts目录
- [ ] 已创建TextMeshPro Font Asset
- [ ] 已将Font Asset重命名为CN_Regular_TMP
- [ ] 字符集已设置为Custom，包含中文字符范围
- [ ] 已配置场景中的TMP_Text组件使用新字体
- [ ] 已验证中文文本正常显示

全部勾选即可完成配置！🎉
