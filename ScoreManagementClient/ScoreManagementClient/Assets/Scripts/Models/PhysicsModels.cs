using System;
using UnityEngine;

namespace Models
{
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
}
