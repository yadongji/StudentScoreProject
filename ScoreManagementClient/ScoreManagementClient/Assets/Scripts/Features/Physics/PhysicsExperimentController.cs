using UnityEngine;
using Core.Base;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 物理实验控制器 - 负责物理实验的逻辑控制
    /// </summary>
    public class PhysicsExperimentController : BaseController
    {
        private ExperimentManager _experimentManager;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _experimentManager = ExperimentManager.Instance;
            if (_experimentManager == null)
            {
                GameObject managerGO = new GameObject("ExperimentManager");
                _experimentManager = managerGO.AddComponent<ExperimentManager>();
            }

            _experimentManager.Initialize();

            // 订阅实验事件
            EventSystem.Subscribe<ExperimentState>("ExperimentStateChanged", OnExperimentStateChanged);

            DebugHelper.Log("✅ [PhysicsExperimentController] 初始化完成");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // 订阅控制器事件
            EventSystem.Subscribe("ExperimentStart", HandleExperimentStart);
            EventSystem.Subscribe("ExperimentPause", HandleExperimentPause);
            EventSystem.Subscribe("ExperimentResume", HandleExperimentResume);
            EventSystem.Subscribe("ExperimentStop", HandleExperimentStop);
            EventSystem.Subscribe("ExperimentReset", HandleExperimentReset);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // 取消订阅控制器事件
            EventSystem.Unsubscribe("ExperimentStart", HandleExperimentStart);
            EventSystem.Unsubscribe("ExperimentPause", HandleExperimentPause);
            EventSystem.Unsubscribe("ExperimentResume", HandleExperimentResume);
            EventSystem.Unsubscribe("ExperimentStop", HandleExperimentStop);
            EventSystem.Unsubscribe("ExperimentReset", HandleExperimentReset);
        }

        #region 事件处理

        private void HandleExperimentStart()
        {
            StartExperiment();
        }

        private void HandleExperimentPause()
        {
            PauseExperiment();
        }

        private void HandleExperimentResume()
        {
            ResumeExperiment();
        }

        private void HandleExperimentStop()
        {
            StopExperiment();
        }

        private void HandleExperimentReset()
        {
            ResetExperiment();
        }

        #endregion

        #region 实验控制

        /// <summary>
        /// 开始实验
        /// </summary>
        public void StartExperiment()
        {
            _experimentManager.StartExperiment();
        }

        /// <summary>
        /// 暂停实验
        /// </summary>
        public void PauseExperiment()
        {
            _experimentManager.PauseExperiment();
        }

        /// <summary>
        /// 继续实验
        /// </summary>
        public void ResumeExperiment()
        {
            _experimentManager.ResumeExperiment();
        }

        /// <summary>
        /// 停止实验
        /// </summary>
        public void StopExperiment()
        {
            _experimentManager.StopExperiment();
        }

        /// <summary>
        /// 重置实验
        /// </summary>
        public void ResetExperiment()
        {
            _experimentManager.ResetExperiment();
        }

        #endregion

        #region 时间控制

        /// <summary>
        /// 设置时间缩放
        /// </summary>
        public void SetTimeScale(float timeScale)
        {
            _experimentManager.SetTimeScale(timeScale);
        }

        /// <summary>
        /// 获取时间缩放
        /// </summary>
        public float GetTimeScale()
        {
            return Time.timeScale;
        }

        #endregion

        #region 物理对象管理

        /// <summary>
        /// 添加物理对象
        /// </summary>
        public void AddPhysicsObject(PhysicsObject obj)
        {
            _experimentManager.AddPhysicsObject(obj);
        }

        /// <summary>
        /// 移除物理对象
        /// </summary>
        public void RemovePhysicsObject(PhysicsObject obj)
        {
            _experimentManager.RemovePhysicsObject(obj);
        }

        /// <summary>
        /// 获取所有物理对象
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<PhysicsObject> GetPhysicsObjects()
        {
            return _experimentManager.PhysicsObjects;
        }

        #endregion

        #region 实验数据

        /// <summary>
        /// 获取实验状态
        /// </summary>
        public ExperimentState GetExperimentState()
        {
            return _experimentManager.State;
        }

        /// <summary>
        /// 获取实验时长
        /// </summary>
        public float GetExperimentDuration()
        {
            return _experimentManager.ExperimentDuration;
        }

        /// <summary>
        /// 获取当前实验类型
        /// </summary>
        public ExperimentType GetExperimentType()
        {
            return _experimentManager.CurrentExperimentType;
        }

        #endregion

        private void OnExperimentStateChanged(ExperimentState state)
        {
            DebugHelper.Log($"🔄 [PhysicsExperimentController] 实验状态变更: {state}");
        }

        protected override void OnDispose()
        {
            // 取消订阅实验事件
            EventSystem.Unsubscribe<ExperimentState>("ExperimentStateChanged", OnExperimentStateChanged);

            base.OnDispose();
            DebugHelper.Log("🗑️ [PhysicsExperimentController] 已释放");
        }
    }
}
