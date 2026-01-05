using UnityEngine;
using UnityEngine.UI;
using Core.Base;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 物理实验视图 - 负责物理实验界面的展示和交互
    /// </summary>
    public class PhysicsExperimentView : BaseView
    {
        [Header("UI组件")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Slider _timeScaleSlider;
        [SerializeField] private Text _experimentStatusText;
        [SerializeField] private Text _experimentDurationText;
        [SerializeField] private Text _objectCountText;

        private ExperimentManager _experimentManager;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 获取实验管理器
            _experimentManager = FindObjectOfType<ExperimentManager>();
            if (_experimentManager == null)
            {
                GameObject managerGO = new GameObject("ExperimentManager");
                _experimentManager = managerGO.AddComponent<ExperimentManager>();
            }

            _experimentManager.Initialize();

            // 绑定按钮事件
            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartButtonClick);

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnPauseButtonClick);

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResumeButtonClick);

            if (_resetButton != null)
                _resetButton.onClick.AddListener(OnResetButtonClick);

            if (_timeScaleSlider != null)
                _timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);

            // 订阅事件
            EventSystem.Subscribe<ExperimentState>("ExperimentStateChanged", OnExperimentStateChanged);
            EventSystem.Subscribe<float>("ExperimentCompleted", OnExperimentCompleted);

            // 初始化UI
            UpdateUI();
            UpdateButtonStates(ExperimentState.NotStarted);

            DebugHelper.Log("✅ [PhysicsExperimentView] 初始化完成");
        }

        private void Update()
        {
            if (_experimentManager != null && _experimentManager.State == ExperimentState.Running)
            {
                UpdateDurationDisplay();
            }
        }

        private void OnStartButtonClick()
        {
            _experimentManager.StartExperiment();
        }

        private void OnPauseButtonClick()
        {
            _experimentManager.PauseExperiment();
        }

        private void OnResumeButtonClick()
        {
            _experimentManager.ResumeExperiment();
        }

        private void OnResetButtonClick()
        {
            _experimentManager.ResetExperiment();
        }

        private void OnTimeScaleChanged(float value)
        {
            _experimentManager.SetTimeScale(value);
        }

        private void OnExperimentStateChanged(ExperimentState state)
        {
            UpdateButtonStates(state);
            UpdateStatusText(state);
        }

        private void OnExperimentCompleted(float duration)
        {
            UpdateDurationDisplay();
            DebugHelper.Log($"✅ [PhysicsExperimentView] 实验完成，时长: {duration:F2}秒");
        }

        private void UpdateUI()
        {
            UpdateStatusText(ExperimentState.NotStarted);
            UpdateDurationDisplay();
            UpdateObjectCount();
        }

        private void UpdateButtonStates(ExperimentState state)
        {
            if (_startButton != null)
                _startButton.interactable = state == ExperimentState.NotStarted || state == ExperimentState.Completed;

            if (_pauseButton != null)
                _pauseButton.interactable = state == ExperimentState.Running;

            if (_resumeButton != null)
                _resumeButton.interactable = state == ExperimentState.Paused;

            if (_resetButton != null)
                _resetButton.interactable = state != ExperimentState.NotStarted;
        }

        private void UpdateStatusText(ExperimentState state)
        {
            if (_experimentStatusText == null)
                return;

            string statusText = "";
            switch (state)
            {
                case ExperimentState.NotStarted:
                    statusText = "未开始";
                    break;
                case ExperimentState.Running:
                    statusText = "运行中";
                    break;
                case ExperimentState.Paused:
                    statusText = "已暂停";
                    break;
                case ExperimentState.Completed:
                    statusText = "已完成";
                    break;
            }

            _experimentStatusText.text = $"状态: {statusText}";
        }

        private void UpdateDurationDisplay()
        {
            if (_experimentDurationText == null || _experimentManager == null)
                return;

            float duration = _experimentManager.ExperimentDuration;
            _experimentDurationText.text = $"时长: {duration:F2}秒";
        }

        private void UpdateObjectCount()
        {
            if (_objectCountText == null || _experimentManager == null)
                return;

            int count = _experimentManager.PhysicsObjects.Count;
            _objectCountText.text = $"对象数: {count}";
        }

        protected override void OnDispose()
        {
            // 解绑按钮事件
            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartButtonClick);

            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(OnPauseButtonClick);

            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(OnResumeButtonClick);

            if (_resetButton != null)
                _resetButton.onClick.RemoveListener(OnResetButtonClick);

            if (_timeScaleSlider != null)
                _timeScaleSlider.onValueChanged.RemoveListener(OnTimeScaleChanged);

            // 取消订阅事件
            EventSystem.Unsubscribe<ExperimentState>("ExperimentStateChanged", OnExperimentStateChanged);
            EventSystem.Unsubscribe<float>("ExperimentCompleted", OnExperimentCompleted);

            base.OnDispose();
        }
    }
}
