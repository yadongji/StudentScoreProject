# Unity 学生成绩管理系统 - 架构说明

## 项目概述

这是一个基于Unity开发的物理实验可视化系统，采用MVC架构设计，支持与Python服务器通过JSON进行交互。

## 目录结构

```
Assets/
├── Prefabs/                          # 预制体文件夹
│   ├── UI/                          # UI预制体
│   │   ├── Login/                   # 登录相关UI
│   │   ├── Common/                  # 通用UI组件
│   │   └── Experiment/              # 实验相关UI
│   ├── 3DObjects/                   # 3D物体预制体
│   │   ├── Experiment/              # 实验3D物体
│   │   └── Equipment/               # 实验设备
│   ├── Effects/                     # 特效预制体
│   ├── Characters/                  # 角色预制体
│   └── Materials/                   # 材质预制体
│
├── Scripts/                         # 脚本文件夹
│   ├── Core/                        # 核心层
│   │   └── Base/                   # 基础类和接口
│   │       ├── IModel.cs           # Model接口
│   │       ├── IView.cs            # View接口
│   │       ├── IController.cs      # Controller接口
│   │       ├── IService.cs         # Service接口
│   │       ├── IManager.cs         # Manager接口
│   │       ├── BaseModel.cs        # Model基类
│   │       ├── BaseView.cs         # View基类
│   │       ├── BaseController.cs   # Controller基类
│   │       ├── BaseService.cs      # Service基类
│   │       ├── BaseManager.cs      # Manager基类
│   │       └── LifecycleManager.cs # 生命周期管理器
│   │
│   ├── Models/                       # 数据模型层
│   │   ├── AuthModels.cs          # 认证相关数据模型
│   │   └── PhysicsModels.cs       # 物理实验相关数据模型
│   │
│   ├── Views/                        # 视图层（待扩展）
│   ├── Controllers/                  # 控制器层（待扩展）
│   ├── Services/                     # 服务层
│   │   └── LoginService.cs        # 登录服务
│   │
│   ├── Features/                     # 功能模块
│   │   ├── Auth/                  # 登录认证模块
│   │   │   ├── LoginController.cs # 登录控制器
│   │   │   └── LoginView.cs      # 登录视图
│   │   └── Physics/               # 物理实验模块
│   │       ├── PhysicsObject.cs   # 物理对象基类
│   │       ├── ExperimentManager.cs # 实验管理器
│   │       ├── PhysicsExperimentView.cs # 实验视图
│   │       └── PhysicsExperimentController.cs # 实验控制器
│   │
│   ├── Utils/                        # 工具类
│   │   ├── DebugHelper.cs         # 调试帮助类
│   │   ├── EventSystem.cs         # 事件系统
│   │   ├── JsonHelper.cs          # JSON帮助类
│   │   ├── ExtensionMethods.cs    # 扩展方法
│   │   ├── StringHelper.cs        # 字符串帮助类
│   │   ├── MathHelper.cs          # 数学帮助类
│   │   ├── PlayerPrefsHelper.cs  # PlayerPrefs帮助类
│   │   ├── ObjectPool.cs          # 对象池
│   │   ├── Singleton.cs           # 单例模式
│   │   └── CoroutineHelper.cs     # 协程帮助类
│   │
│   └── Hotfix/                      # 热更新接口层（预留）
│       ├── IHotfixInterface.cs    # 热更新接口
│       ├── HotfixManager.cs       # 热更新管理器
│       ├── HotfixBridge.cs        # 热更新桥接器
│       └── HotfixAttribute.cs     # 热更新特性
│
└── Resources/                       # 资源文件夹
```

## MVC架构说明

### Model（模型层）
- **职责**：负责数据存储和业务逻辑
- **位置**：`Assets/Scripts/Models/`
- **示例**：
  - `LoginRequest` - 登录请求数据模型
  - `LoginResponse` - 登录响应数据模型
  - `PhysicsObjectData` - 物理对象数据模型

### View（视图层）
- **职责**：负责UI显示和用户交互
- **位置**：`Assets/Scripts/Features/` 各模块下的 `*View.cs`
- **示例**：
  - `LoginView` - 登录界面视图
  - `PhysicsExperimentView` - 物理实验界面视图

### Controller（控制器层）
- **职责**：负责业务逻辑控制，协调Model和View
- **位置**：`Assets/Scripts/Features/` 各模块下的 `*Controller.cs`
- **示例**：
  - `LoginController` - 登录逻辑控制器
  - `PhysicsExperimentController` - 物理实验逻辑控制器

### Service（服务层）
- **职责**：负责与外部系统交互（如网络服务）
- **位置**：`Assets/Scripts/Services/`
- **示例**：
  - `LoginService` - 登录网络服务

### Manager（管理器层）
- **职责**：管理特定模块的生命周期和全局状态
- **位置**：`Assets/Scripts/Features/` 各模块下的 `*Manager.cs`
- **示例**：
  - `ExperimentManager` - 实验管理器

## 核心组件

### 事件系统 (EventSystem)
- **位置**：`Utils/EventSystem.cs`
- **用途**：模块间解耦通信
- **使用方式**：
  ```csharp
  // 订阅事件
  EventSystem.Subscribe<LoginState>("LoginStateChanged", OnLoginStateChanged);

  // 发布事件
  EventSystem.Publish<LoginState>("LoginStateChanged", newState);

  // 取消订阅
  EventSystem.Unsubscribe<LoginState>("LoginStateChanged", OnLoginStateChanged);
  ```

### 调试系统 (DebugHelper)
- **位置**：`Utils/DebugHelper.cs`
- **用途**：统一的日志输出接口
- **使用方式**：
  ```csharp
  DebugHelper.Log("普通日志");
  DebugHelper.LogWarning("警告日志");
  DebugHelper.LogError("错误日志");
  DebugHelper.LogColor("彩色日志", Color.red);
  ```

### 单例模式 (Singleton)
- **位置**：`Utils/Singleton.cs`
- **用途**：提供单例模式支持
- **使用方式**：
  ```csharp
  // MonoBehaviour单例
  public class MyManager : Singleton<MyManager> { }
  var instance = MyManager.Instance;

  // 纯C#类单例
  public class MyService : Singleton<MyService> { }
  var service = MyService.Instance;
  ```

### 对象池 (ObjectPool)
- **位置**：`Utils/ObjectPool.cs`
- **用途**：优化对象创建和销毁性能
- **使用方式**：
  ```csharp
  // 普通对象池
  var pool = new ObjectPool<MyClass>(() => new MyClass());
  var obj = pool.Get();
  pool.Release(obj);

  // GameObject对象池
  var goPool = new GameObjectPool(prefab, parent, initialSize);
  var go = goPool.Get();
  goPool.Release(go);
  ```

## 登录系统

### 架构
```
LoginView (UI层)
    ↓ 用户输入
LoginController (控制层)
    ↓ 调用服务
LoginService (服务层)
    ↓ 网络请求
Python服务器
```

### 使用流程
1. **UI绑定**：在Unity场景中创建LoginView并绑定UI组件
2. **自动初始化**：LoginView会自动创建并初始化LoginController和LoginService
3. **用户输入**：用户输入用户名和密码后点击登录按钮
4. **事件传递**：通过EventSystem传递登录请求
5. **网络请求**：LoginService发送HTTP请求到Python服务器
6. **结果返回**：通过EventSystem返回登录结果到UI层

## 物理实验系统

### 核心类
- **PhysicsObject**：物理对象基类，提供统一的物理属性接口
- **ExperimentManager**：实验管理器，管理实验生命周期
- **PhysicsExperimentView**：实验视图，提供实验控制UI
- **PhysicsExperimentController**：实验控制器，处理实验逻辑

### 功能特性
- 实验开始/暂停/继续/停止/重置
- 时间缩放控制
- 物理对象管理
- 实验数据记录
- 状态机管理

## 热更新接口

虽然当前不使用Xlua，但已预留完整的热更新接口：
- **IHotfixInterface**：热更新接口定义
- **HotfixManager**：热更新管理器
- **HotfixBridge**：C#与Lua通信桥接器
- **HotfixAttribute**：热更新标记特性

集成Xlua时，只需：
1. 引入Xlua包
2. 取消HotfixManager中的注释代码
3. 实现Lua脚本加载逻辑

## 与服务器交互

### 协议
- **数据格式**：JSON
- **通信方式**：HTTP/HTTPS
- **认证方式**：Bearer Token

### 示例代码
```csharp
// 创建登录请求
var request = new LoginRequest
{
    phonenumber = "13800138000",
    password = "123456"
};

// 发送请求（通过EventSystem）
string jsonData = JsonUtility.ToJson(request);
EventSystem.Publish<string>("LoginRequest", jsonData);

// 接收响应
EventSystem.Subscribe<LoginResponse>("LoginSuccess", OnLoginSuccess);
```

## 扩展指南

### 添加新功能模块
1. 在 `Features/` 下创建新文件夹
2. 创建对应的Model、View、Controller、Service
3. 继承相应的基类（BaseView、BaseController等）
4. 使用EventSystem进行模块间通信

### 创建新物理实验
1. 继承 `PhysicsObject` 创建新的物理对象
2. 在 `ExperimentManager` 中添加实验特定逻辑
3. 创建相应的UI和控制逻辑
4. 在场景中布置物理对象

### 添加新的网络服务
1. 在 `Services/` 下创建新的Service类
2. 继承 `BaseService`
3. 实现网络请求逻辑
4. 在Controller中调用Service

## 注意事项

1. **命名空间**：
   - Core层使用 `Core.Base`
   - Utils使用 `Utils`
   - Models使用 `Models`
   - Services使用 `Services`
   - Features使用 `Features.ModuleName`

2. **生命周期管理**：
   - 所有组件通过LifecycleManager统一管理
   - 使用Initialize/Dispose模式管理资源

3. **事件订阅/取消**：
   - 在OnEnable中订阅事件
   - 在OnDisable中取消订阅事件

4. **单例使用**：
   - MonoBehaviour组件使用 `Singleton<T>`
   - 纯C#类使用 `Singleton<T>`

5. **对象池**：
   - 频繁创建销毁的对象使用对象池优化性能

## 版本信息

- **Unity版本**：2019.4+
- **.NET版本**：4.x
- **架构设计**：MVC
- **热更新**：预留接口（未启用）
- **服务器语言**：Python

## 联系与支持

如有问题或建议，请查看项目文档或联系开发团队。
