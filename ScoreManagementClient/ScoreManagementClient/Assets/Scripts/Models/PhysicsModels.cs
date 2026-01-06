using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 实验类型枚举
/// </summary>
public enum ExperimentType
{
    /// <summary>
    /// 力学实验
    /// </summary>
    Mechanics,

    /// <summary>
    /// 电学实验
    /// </summary>
    Electricity,

    /// <summary>
    /// 光学实验
    /// </summary>
    Optics,

    /// <summary>
    /// 热学实验
    /// </summary>
    Thermodynamics,

    /// <summary>
    /// 波动实验
    /// </summary>
    Waves
}

/// <summary>
/// 实验状态枚举
/// </summary>
public enum ExperimentState
{
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted,

    /// <summary>
    /// 运行中
    /// </summary>
    Running,

    /// <summary>
    /// 暂停
    /// </summary>
    Paused,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed
}

/// <summary>
/// 实验数据模型
/// </summary>
[Serializable]
public class ExperimentData
{
    public string experimentId;
    public string experimentName;
    public ExperimentType experimentType;
    public string description;
    public string parameters;
    public long createTime;
    public long updateTime;
}

/// <summary>
/// 物理对象数据模型
/// </summary>
[Serializable]
public class PhysicsObjectData
{
    public string objectId;
    public string objectName;
    public string objectPrefab;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public float mass;
    public string properties;
}

/// <summary>
/// 实验结果数据模型
/// </summary>
[Serializable]
public class ExperimentResult
{
    public string experimentId;
    public string userId;
    public float duration;
    public string results;
    public long timestamp;
}

/// <summary>
/// 物理数据点 - 用于记录单帧物理数据
/// </summary>
[Serializable]
public class PhysicsDataPoint
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public float speed;
    public float kineticEnergy;
    public float potentialEnergy;
    public float totalEnergy;
}

/// <summary>
/// 物理数据导出模型
/// </summary>
[Serializable]
public class PhysicsDataExport
{
    public string objectName;
    public float mass;
    public float sampleInterval;
    public List<PhysicsDataPoint> dataPoints;
}

/// <summary>
/// 皮带传送带配置
/// </summary>
[Serializable]
public class BeltConveyorConfig
{
    public float beltSpeed;
    public float beltLength;
    public float beltWidth;
    public float frictionCoefficient;
    public float angle;
}

/// <summary>
/// 能量数据
/// </summary>
[Serializable]
public class EnergyData
{
    public float kineticEnergy;
    public float potentialEnergy;
    public float elasticPotentialEnergy;
    public float totalEnergy;
    public float time;
}
