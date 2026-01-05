using UnityEngine;
using System.Collections.Generic;
using Core.Base;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 实验管理器 - 管理物理实验的生命周期和对象
    /// </summary>
    public class ExperimentManager : BaseManager
    {
        public static ExperimentManager Instance { get; private set; }

        [Header("实验配置")]
        [SerializeField] private ExperimentType _currentExperimentType = ExperimentType.Mechanics;
        [SerializeField] private float _timeScale = 1f;

        [Header("实验状态")]
        [SerializeField] private ExperimentState _state = ExperimentState.NotStarted;

        private List<PhysicsObject> _physicsObjects = new List<PhysicsObject>();
        private float _experimentStartTime;
        private float _experimentDuration;

        /// <summary>
        /// 当前实验类型
        /// </summary>
        public ExperimentType CurrentExperimentType => _currentExperimentType;

        /// <summary>
        /// 实验状态
        /// </summary>
        public ExperimentState State => _state;

        /// <summary>
        /// 实验运行时长
        /// </summary>
        public float ExperimentDuration => Time.time - _experimentStartTime;

        /// <summary>
        /// 物理对象列表
        /// </summary>
        public IReadOnlyList<PhysicsObject> PhysicsObjects => _physicsObjects;

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            LoadPhysicsObjects();
            Time.timeScale = _timeScale;

            DebugHelper.Log($"✅ [ExperimentManager] 初始化完成 | 实验类型: {_currentExperimentType}");
        }

        private void Update()
        {
            if (_state == ExperimentState.Running)
            {
                _experimentDuration = Time.time - _experimentStartTime;
                UpdateExperiment();
            }
        }

        /// <summary>
        /// 加载场景中的物理对象
        /// </summary>
        private void LoadPhysicsObjects()
        {
            _physicsObjects.Clear();
            PhysicsObject[] objects = FindObjectsOfType<PhysicsObject>();
            _physicsObjects.AddRange(objects);

            DebugHelper.Log($"🔍 [ExperimentManager] 加载 {_physicsObjects.Count} 个物理对象");
        }

        /// <summary>
        /// 更新实验逻辑
        /// </summary>
        protected virtual void UpdateExperiment()
        {
            // 子类可以重写此方法实现具体的实验逻辑
        }

        /// <summary>
        /// 开始实验
        /// </summary>
        public virtual void StartExperiment()
        {
            if (_state == ExperimentState.Running)
            {
                DebugHelper.LogWarning("⚠️ [ExperimentManager] 实验已在运行中");
                return;
            }

            _state = ExperimentState.Running;
            _experimentStartTime = Time.time;
            _experimentDuration = 0f;
            Time.timeScale = _timeScale;

            EventSystem.Publish<ExperimentState>("ExperimentStateChanged", _state);
            DebugHelper.Log("▶️ [ExperimentManager] 实验开始");
        }

        /// <summary>
        /// 暂停实验
        /// </summary>
        public virtual void PauseExperiment()
        {
            if (_state != ExperimentState.Running)
            {
                DebugHelper.LogWarning("⚠️ [ExperimentManager] 实验未运行，无法暂停");
                return;
            }

            _state = ExperimentState.Paused;
            Time.timeScale = 0f;

            EventSystem.Publish<ExperimentState>("ExperimentStateChanged", _state);
            DebugHelper.Log("⏸️ [ExperimentManager] 实验暂停");
        }

        /// <summary>
        /// 继续实验
        /// </summary>
        public virtual void ResumeExperiment()
        {
            if (_state != ExperimentState.Paused)
            {
                DebugHelper.LogWarning("⚠️ [ExperimentManager] 实验未暂停，无法继续");
                return;
            }

            _state = ExperimentState.Running;
            Time.timeScale = _timeScale;

            EventSystem.Publish<ExperimentState>("ExperimentStateChanged", _state);
            DebugHelper.Log("▶️ [ExperimentManager] 实验继续");
        }

        /// <summary>
        /// 停止实验
        /// </summary>
        public virtual void StopExperiment()
        {
            if (_state == ExperimentState.NotStarted)
            {
                DebugHelper.LogWarning("⚠️ [ExperimentManager] 实验未开始");
                return;
            }

            _state = ExperimentState.Completed;
            Time.timeScale = 1f;

            EventSystem.Publish<ExperimentState>("ExperimentStateChanged", _state);
            EventSystem.Publish<float>("ExperimentCompleted", _experimentDuration);

            DebugHelper.Log($"⏹️ [ExperimentManager] 实验结束 | 时长: {_experimentDuration:F2}秒");
        }

        /// <summary>
        /// 重置实验
        /// </summary>
        public virtual void ResetExperiment()
        {
            _state = ExperimentState.NotStarted;
            _experimentDuration = 0f;
            Time.timeScale = 1f;

            foreach (var obj in _physicsObjects)
            {
                obj.ResetPhysics();
            }

            EventSystem.Publish<ExperimentState>("ExperimentStateChanged", _state);
            DebugHelper.Log("🔄 [ExperimentManager] 实验已重置");
        }

        /// <summary>
        /// 设置时间缩放
        /// </summary>
        public void SetTimeScale(float timeScale)
        {
            _timeScale = Mathf.Clamp(timeScale, 0.1f, 10f);

            if (_state == ExperimentState.Running)
            {
                Time.timeScale = _timeScale;
            }

            DebugHelper.Log($"⏱️ [ExperimentManager] 时间缩放: {_timeScale}x");
        }

        /// <summary>
        /// 添加物理对象
        /// </summary>
        public void AddPhysicsObject(PhysicsObject obj)
        {
            if (obj != null && !_physicsObjects.Contains(obj))
            {
                _physicsObjects.Add(obj);
                DebugHelper.Log($"➕ [ExperimentManager] 添加物理对象: {obj.name}");
            }
        }

        /// <summary>
        /// 移除物理对象
        /// </summary>
        public void RemovePhysicsObject(PhysicsObject obj)
        {
            if (obj != null && _physicsObjects.Contains(obj))
            {
                _physicsObjects.Remove(obj);
                DebugHelper.Log($"➖ [ExperimentManager] 移除物理对象: {obj.name}");
            }
        }

        /// <summary>
        /// 获取所有物理对象数据
        /// </summary>
        public List<PhysicsObjectData> GetAllObjectData()
        {
            var dataList = new List<PhysicsObjectData>();
            foreach (var obj in _physicsObjects)
            {
                dataList.Add(obj.ToData());
            }
            return dataList;
        }

        /// <summary>
        /// 从数据加载物理对象
        /// </summary>
        public void LoadObjectData(List<PhysicsObjectData> dataList)
        {
            if (dataList == null || dataList.Count == 0)
                return;

            foreach (var data in dataList)
            {
                // TODO: 从数据加载并实例化物理对象
                DebugHelper.Log($"📦 [ExperimentManager] 加载对象数据: {data.objectName}");
            }
        }
    }
}
