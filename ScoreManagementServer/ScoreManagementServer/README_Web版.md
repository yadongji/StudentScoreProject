# 宿舍管理与请假管理系统 - Web版

## 📋 系统概述

这是一个基于Flask的Web应用程序，为宿管阿姨、班主任、学生家长提供便捷的宿舍管理和请假管理服务。

### ✨ 核心功能

- 🏢 **宿舍楼管理** - 管理宿舍楼信息、统计
- 👩‍🏫 **宿管查寝** - 每晚记录学生就寝情况
- 👨‍🏫 **班级出勤** - 班主任查看班级出勤
- ✍️ **请假管理** - 学生申请、班主任审批
- 📱 **移动友好** - 支持手机访问
- 🎨 **美观界面** - Bootstrap 5响应式设计

---

## 🚀 快速开始

### 1. 安装依赖

```bash
pip install Flask
```

### 2. 启动服务器

```bash
python app.py
```

或双击运行：`启动Web服务器.bat`

### 3. 访问系统

**电脑访问：** http://127.0.0.1:5000
**手机访问：** http://[本机IP]:5000

---

## 👥 用户角色

| 角色 | 功能 | 适用人群 |
|------|------|----------|
| 系统管理员 | 管理宿舍楼、宿管人员 | 学校管理员 |
| 宿管阿姨 | 查看查寝、记录查寝 | 宿管阿姨 |
| 班主任 | 查看出勤、审批请假 | 班主任 |
| 学生家长 | 申请请假、查看记录 | 学生家长 |

---

## 📁 项目结构

```
ScoreManagementServer/
├── app.py                              # Flask主程序
├── requirements.txt                     # Python依赖
├── 启动Web服务器.bat                    # 启动脚本
├── database_schema_dormitory_leave.sql   # 数据库Schema
├── Web版使用说明.md                     # 详细使用说明
└── templates/                          # HTML模板
    ├── base.html                       # 基础模板
    ├── login.html                      # 登录页面
    ├── admin/                          # 管理员页面
    │   └── dashboard.html
    ├── staff/                          # 宿管页面
    │   ├── dashboard.html
    │   └── checkin.html
    ├── teacher/                        # 班主任页面
    │   ├── dashboard.html
    │   └── attendance.html
    └── parent/                         # 家长页面
        ├── dashboard.html
        └── apply_leave.html
```

---

## 🔧 技术栈

- **后端框架**: Flask 2.0.1
- **数据库**: SQLite3
- **前端框架**: Bootstrap 5
- **图标**: Bootstrap Icons
- **弹窗**: SweetAlert2

---

## 📖 使用文档

- 📚 [Web版使用说明.md](Web版使用说明.md) - 详细的使用指南
- 📚 [宿舍系统初始化指南.md](宿舍系统初始化指南.md) - 数据库初始化指南
- 📚 [宿舍请假管理系统使用说明.md](宿舍请假管理系统使用说明.md) - 原命令行版说明

---

## 🎯 功能说明

### 宿管阿姨

1. **查看查寝统计**
   - 今日总人数、在寝、未归、请假、外出、晚归

2. **记录查寝**
   - 选择日期和时间
   - 为每个学生选择状态
   - 提交查寝记录

### 班主任

1. **查看班级出勤**
   - 班级总人数、在校、请假、未查寝
   - 学生详细出勤状态

2. **审批请假**
   - 查看待审批的请假申请
   - 通过或拒绝申请
   - 添加审批备注

### 学生家长

1. **查看学生信息**
   - 基本信息、宿舍信息
   - 今日出勤状态

2. **申请请假**
   - 填写请假申请表
   - 选择请假类型、原因、时间
   - 等待班主任审批

3. **查看请假记录**
   - 查看所有请假历史
   - 了解审批状态

---

## 📊 数据库设计

### 核心表

- `Dormitories` - 宿舍楼表
- `DormitoryRooms` - 宿舍房间表
- `DormitoryBeds` - 宿舍床位表
- `DormitoryStaff` - 宿管人员表
- `StudentDormitoryAssignments` - 学生宿舍分配表
- `DormitoryCheckInRecords` - 查寝记录表
- `LeaveApplications` - 请假申请表
- `HeadTeachers` - 班主任表

### 视图

- `vw_DormitorySummary` - 宿舍楼汇总
- `vw_FloorCheckInSummary` - 楼层查寝统计
- `vw_ClassAttendanceDaily` - 班级出勤情况
- `vw_LeaveApplicationDetail` - 请假申请详情
- 等7个视图

---

## 🌐 API接口

### 宿管API

```
POST /api/staff/checkin
提交查寝记录
```

### 班主任API

```
POST /api/teacher/approve_leave
审批请假申请
```

### 家长API

```
POST /parent/apply_leave
提交请假申请
```

---

## 📱 移动端访问

### 查看本机IP

```bash
ipconfig
```

找到IPv4地址，如：`192.168.1.100`

### 手机访问

```
http://192.168.1.100:5000
```

**注意：**
- 电脑和手机必须在同一WiFi网络
- 确保防火墙允许端口5000

---

## 🔐 安全说明

⚠️ **当前为演示版本**

- 任意用户名密码均可登录
- 无实际的用户认证
- 仅用于演示和测试

**生产环境需要：**
- 添加用户密码验证
- 使用HTTPS加密
- 添加防CSRF保护
- 实现权限控制

---

## 🚧 待开发功能

- [ ] 用户密码验证
- [ ] 微信登录支持
- [ ] 消息推送功能
- [ ] 数据统计图表
- [ ] 导出Excel报表
- [ ] 批量导入数据
- [ ] 视频查寝功能
- [ ] 人脸识别签到

---

## 🐛 已知问题

1. **演示模式**
   - 所有角色使用相同的登录方式
   - 无实际权限验证

2. **数据关联**
   - 需要关联真实的Students表
   - 学生宿舍分配需要实际数据

---

## 📝 更新日志

### v1.0.0 (2026-01-22)

- ✅ 创建Web版本
- ✅ 实现多角色登录
- ✅ 宿管查寝功能
- ✅ 班主任审批功能
- ✅ 家长请假功能
- ✅ 响应式界面设计

---

## 📞 技术支持

如有问题，请查看：
1. Web版使用说明.md
2. 宿舍系统初始化指南.md
3. 控制台错误信息

---

## 📄 许可证

MIT License

---

## 🙏 致谢

- Flask - Web框架
- Bootstrap - UI框架
- SweetAlert2 - 弹窗组件
- Bootstrap Icons - 图标库

---

**宿舍管理系统 - 让宿舍管理更简单！** 🏢✨
