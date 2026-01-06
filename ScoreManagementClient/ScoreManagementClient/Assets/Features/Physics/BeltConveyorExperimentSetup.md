# 皮带传送带物理实验 - 场景配置指南

## 📋 实验概述

本实验模拟皮带传送带上的物块运动，支持：
- ✅ 实时数据采集和可视化
- ✅ 动态参数调节（摩擦系数、传送带速度等）
- ✅ 能量守恒验证
- ✅ 理论值与实验值对比

---

## 🚀 快速开始

### 第1步：创建新场景

1. 在Unity中：**File → New Scene**
2. 保存场景：`Assets/Scenes/BeltConveyorExperiment.unity`

---

### 第2步：创建基础场景结构

#### 2.1 创建实验控制器（根对象）

```
Hierarchy:
└── BeltConveyorExperiment (Empty GameObject)
    └── BeltConveyorExperimentController (Script)
```

**操作：**
1. 在Hierarchy中右键 → **Create Empty**
2. 命名为 `BeltConveyorExperiment`
3. 添加组件：**BeltConveyorExperimentController** 脚本

---

#### 2.2 创建传送带

```
Hierarchy:
└── BeltConveyorExperiment
    └── Conveyor (Cube)
        └── BeltConveyor (Script)
```

**操作：**
1. 创建Cube：右键 → **3D Object → Cube**
2. 命名为 `Conveyor`
3. 设置Transform：
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (5, 0.1, 10)

4. 添加组件：
   - **BoxCollider**
   - **BeltConveyor** 脚本

5. 配置BeltConveyor脚本：
   - Belt Speed: 2
   - Belt Width: 5
   - Belt Length: 10
   - Is Running: true
   - Is Inclined: false
   - Incline Angle: 0

---

#### 2.3 创建物块（Block）

```
Hierarchy:
└── BeltConveyorExperiment
    └── Conveyor
    └── Block (Cube)
        └── PhysicsObject (Script)
        └── DataLogger (Script)
        └── EnergyCalculator (Script)
        └── Rigidbody
        └── BoxCollider
```

**操作：**
1. 创建Cube：右键 → **3D Object → Cube**
2. 命名为 `Block`
3. 设置Transform：
   - Position: (0, 0.3, -4)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

4. 添加组件：
   - **Rigidbody**
   - **BoxCollider**
   - **PhysicsObject** 脚本
   - **DataLogger** 脚本
   - **EnergyCalculator** 脚本

5. 配置Rigidbody：
   - Mass: 1
   - Use Gravity: true
   - Is Kinematic: false
   - Drag: 0.1
   - Angular Drag: 0.05

6. 配置PhysicsObject：
   - Mass: 1
   - Object Name: "TestBlock"
   - Object Color: Red

7. 配置DataLogger：
   - Target Object: [拖入Block对象自身]
   - Max Data Points: 500
   - Sample Interval: 0.02
   - Record Velocity: ✓
   - Record Kinetic Energy: ✓
   - Record Potential Energy: ✓

8. 配置EnergyCalculator：
   - Target Object: [拖入Block对象自身]
   - Show UI: true
   - Show Debug: true
   - Gravity: 9.81
   - Reference Height: 0

---

#### 2.4 创建物理材质控制器

```
Hierarchy:
└── BeltConveyorExperiment
    └── Conveyor
    └── Block
    └── PhysicsMaterialController (Empty GameObject)
```

**操作：**
1. 创建Empty GameObject
2. 命名为 `PhysicsMaterialController`
3. 添加组件：**PhysicsMaterialController** 脚本

4. 配置脚本：
   - Initial Dynamic Friction: 0.6
   - Initial Static Friction: 0.6
   - Friction Coefficient Range: Min=0, Max=1

5. 应用材质：
   - 在Project窗口右键 → **Create → Physic Material**
   - 命名为 `ConveyorMaterial`
   - 设置Dynamic Friction: 0.6
   - 设置Static Friction: 0.6

6. 将材质应用到传送带：
   - 选中 `Conveyor` 对象
   - 在BoxCollider组件中，拖入 `ConveyorMaterial`

---

#### 2.5 创建参数控制器和UI

```
Hierarchy:
└── BeltConveyorExperiment
    └── Conveyor
    └── Block
    └── PhysicsMaterialController
    └── ParameterController (Empty GameObject)
```

**操作：**
1. 创建Empty GameObject
2. 命名为 `ParameterController`
3. 添加组件：**ParameterController** 脚本

4. 创建UI Canvas：
   - 右键 → **UI → Canvas**
   - 命名为 `ExperimentUI`

5. 在Canvas下创建UI元素：

   **控制按钮：**
   - 创建Button：**UI → Button**
   - 命名为 `StartButton`
   - 文本："开始实验"

   - 创建Button：`PauseButton`
   - 文本："暂停实验"

   - 创建Button：`ResetButton`
   - 文本："重置实验"

   - 创建Button：`StopButton`
   - 文本："停止实验"

   **参数滑块：**
   - 创建Slider：**UI → Slider**
   - 命名为 `FrictionSlider`
   - 设置Min=0, Max=1, Value=0.6

   - 创建Text：命名为 `FrictionValueText`
   - 文本："0.60"

   - 创建Slider：`AngleSlider`
   - 设置Min=0, Max=90, Value=30

   - 创建Text：`AngleValueText`
   - 文本："30.0°"

   - 创建Slider：`BeltSpeedSlider`
   - 设置Min=0, Max=10, Value=2

   - 创建Text：`BeltSpeedValueText`
   - 文本："2.00 m/s"

   **数据显示：**
   - 创建Text：`ExperimentStatusText`
   - 文本："状态: 未开始"

   - 创建Text：`TimeText`
   - 文本："时间: 0.00s"

   - 创建Text：`VelocityText`
   - 文本："速度: 0.000 m/s"

   - 创建Text：`EnergyText`
   - 文本："动能: 0.00 J\n势能: 0.00 J\n总能: 0.00 J"

   **计算结果：**
   - 创建Text：`AccelerationText`
   - 文本："加速度: 0.000 m/s²"

   - 创建Text：`TheoreticalVelocityText`
   - 文本："理论速度(1s): 0.000 m/s"

6. 配置ParameterController：
   - Material Controller: [拖入PhysicsMaterialController对象]
   - Friction Slider: [拖入FrictionSlider]
   - Gravity Slider: [不设置]
   - Angle Slider: [拖入AngleSlider]
   - Belt Speed Slider: [拖入BeltSpeedSlider]
   - 拖入所有对应的Text对象

---

#### 2.6 创建图表

```
Hierarchy:
└── BeltConveyorExperiment
    └── ExperimentUI
    └── VelocityChart (Empty GameObject)
    └── EnergyChart (Empty GameObject)
```

**操作：**
1. 创建Empty GameObject
2. 命名为 `VelocityChart`
3. 添加组件：**SimpleChartDrawer** 脚本
4. 设置Transform：
   - Position: (5, 3, -5)
   - Rotation: (90, 0, 0)

5. 配置SimpleChartDrawer：
   - Line Color: Cyan
   - Grid Color: Gray
   - Max Points: 100
   - Chart Width: 5
   - Chart Height: 3
   - Min Value: 0
   - Max Value: 10

6. 重复上述步骤创建 `EnergyChart`
7. 修改Transform：
   - Position: (5, 1, -5)
   - Chart Height: 2
   - Max Value: 100

---

### 第3步：连接所有引用

选中 `BeltConveyorExperiment` 对象，配置 **BeltConveyorExperimentController** 脚本：

| 字段 | 引用对象 |
|------|---------|
| Block Object | [拖入Block] |
| Conveyor | [拖入Conveyor] |
| Material Controller | [拖入PhysicsMaterialController] |
| Data Logger | [拖入Block（会自动获取DataLogger组件）] |
| Energy Calculator | [拖入Block] |
| Parameter Controller | [拖入ParameterController] |
| Start Button | [拖入StartButton] |
| Pause Button | [拖入PauseButton] |
| Reset Button | [拖入ResetButton] |
| Stop Button | [拖入StopButton] |
| Experiment Status Text | [拖入ExperimentStatusText] |
| Time Text | [拖入TimeText] |
| Velocity Text | [拖入VelocityText] |
| Energy Text | [拖入EnergyText] |
| Velocity Chart | [拖入VelocityChart] |
| Energy Chart | [拖入EnergyChart] |

---

### 第4步：配置摄像机和灯光

#### 4.1 设置摄像机

1. 选中Main Camera
2. 设置Transform：
   - Position: (8, 6, -8)
   - Rotation: (35, 0, 0)

3. 配置Camera：
   - Clear Flags: Solid Color
   - Background: Light Gray
   - Field of View: 60

#### 4.2 设置灯光

1. 创建Directional Light：**Light → Directional Light**
2. 设置Transform：
   - Rotation: (50, -30, 0)

3. 配置Light：
   - Intensity: 1
   - Color: White

---

### 第5步：测试场景

1. 点击 **Play** 按钮
2. 点击UI中的 **"开始实验"** 按钮
3. 观察：
   - 物块在传送带上移动
   - 实时数据更新
   - 图表绘制
   - 能量计算

---

## 🎨 场景完整结构

```
BeltConveyorExperiment (Root)
├── Conveyor (Cube)
│   ├── BoxCollider
│   └── BeltConveyor (Script)
├── Block (Cube)
│   ├── Rigidbody
│   ├── BoxCollider
│   ├── PhysicsObject (Script)
│   ├── DataLogger (Script)
│   └── EnergyCalculator (Script)
├── PhysicsMaterialController (Empty)
│   └── PhysicsMaterialController (Script)
├── ParameterController (Empty)
│   └── ParameterController (Script)
├── ExperimentUI (Canvas)
│   ├── StartButton (Button)
│   ├── PauseButton (Button)
│   ├── ResetButton (Button)
│   ├── StopButton (Button)
│   ├── FrictionSlider (Slider)
│   ├── FrictionValueText (Text)
│   ├── AngleSlider (Slider)
│   ├── AngleValueText (Text)
│   ├── BeltSpeedSlider (Slider)
│   ├── BeltSpeedValueText (Text)
│   ├── ExperimentStatusText (Text)
│   ├── TimeText (Text)
│   ├── VelocityText (Text)
│   ├── EnergyText (Text)
│   ├── AccelerationText (Text)
│   └── TheoreticalVelocityText (Text)
├── VelocityChart (Empty)
│   └── SimpleChartDrawer (Script)
└── EnergyChart (Empty)
    └── SimpleChartDrawer (Script)
```

---

## 🔧 高级配置

### 创建倾斜传送带

1. 选中 `Conveyor` 对象
2. 在BeltConveyor脚本中：
   - Is Inclined: ✓
   - Incline Angle: 30

3. 重新调整Block的初始位置：
   - Position: (0, 0.3, -4)

### 创建弹簧系统

```
Hierarchy:
└── BeltConveyorExperiment
    └── Block
    └── SpringSystem (Empty GameObject)
```

1. 创建Empty GameObject
2. 命名为 `SpringSystem`
3. 添加组件：
   - **SpringJoint**
   - **SpringSystem** 脚本

4. 配置SpringJoint：
   - Connected Body: [选择传送带的Rigidbody或不连接]
   - Anchor: (0, 0, 0)
   - Connected Anchor: (0, 0, 0)
   - Min Distance: 0.1
   - Max Distance: 5
   - Spring: 10
   - Damper: 0.5

5. 配置SpringSystem：
   - Spring Force: 10
   - Damper: 0.5
   - Min Distance: 0.1
   - Max Distance: 5

### 创建皮带传动系统

```
Hierarchy:
└── BeltConveyorExperiment
    └── DriverWheel (Cylinder)
    │   ├── Rigidbody
    │   └── BeltTransmission (Script)
    └── DrivenWheel (Cylinder)
        ├── Rigidbody
        └── Tag: "DrivenWheel"
```

1. 创建Cylinder作为主动轮
2. 添加Rigidbody
3. 添加BeltTransmission脚本
4. 配置脚本参数

5. 创建Cylinder作为从动轮
6. 添加Rigidbody
7. 设置Tag为"DrivenWheel"

---

## 📊 数据分析

### 查看实时数据

在运行时，可以通过以下方式查看数据：

1. **Inspector面板**
   - 选中Block对象
   - 查看DataLogger和EnergyCalculator的实时数据

2. **Console窗口**
   - 查看实验日志
   - 查看能量计算结果

3. **场景中的图表**
   - 速度图表实时显示速度变化
   - 能量图表实时显示能量变化

### 导出实验数据

1. 运行实验
2. 停止实验后，选中 `BeltConveyorExperiment` 对象
3. 在BeltConveyorExperimentController脚本中：
   - 调用 `ExportData()` 方法（可通过代码或Inspector按钮）
4. 查看Console中的JSON数据

---

## ⚠️ 常见问题

**Q: 物块不移动？**
A: 检查以下几点：
- Rigidbody的Is Kinematic是否为false
- 传送带是否在运行（BeltConveyor.Is Running = true）
- 物理材质的摩擦系数是否合理

**Q: 图表不显示？**
A: 确保：
- SimpleChartDrawer脚本已正确配置
- 数据记录已开始（DataLogger.StartRecording）
- Max Points设置合理

**Q: 能量计算不准确？**
A: 检查：
- 重力加速度设置是否正确（9.81）
- 参考高度是否正确
- 数据采样频率是否合适

**Q: 理论值与实验值差异大？**
A: 这可能由于：
- 物理引擎的数值精度
- 摩擦系数的近似计算
- Unity的物理模拟误差

---

## 📚 扩展实验

### 实验1：斜面滑块

将传送带倾斜，验证斜面运动公式：
```
a = g*sin(θ) - μ*g*cos(θ)
```

### 实验2：摩擦力研究

改变摩擦系数，观察：
- 不同μ值下的加速度变化
- 摩擦力对运动的影响
- 动摩擦与静摩擦的区别

### 实验3：能量守恒验证

在无摩擦情况下，验证：
- 动能 + 势能 = 常数
- 机械能守恒定律

### 实验4：弹簧振子

使用弹簧系统，研究：
- 简谐运动
- 弹性势能与动能的转换
- 阻尼对振幅的影响

### 实验5：皮带传动

使用BeltTransmission，验证：
- 线速度关系：v = ω*r
- 角速度与半径的关系
- 传动比的作用

---

## 🎯 实验目标达成检查

- [ ] 场景创建完成
- [ ] 所有脚本配置正确
- [ ] 物块能在传送带上正常运动
- [ ] 实时数据正确采集
- [ ] 图表正常显示
- [ ] 能量计算准确
- [ ] 理论值与实验值可对比
- [ ] 参数调节功能正常
- [ ] 实验可以开始/暂停/重置

---

## 📞 技术支持

如有问题，请参考：
- **脚本说明**：查看各脚本的注释
- **Unity文档**：https://docs.unity3d.com/
- **物理引擎文档**：Unity Physics Manual

祝你实验顺利！🎉
