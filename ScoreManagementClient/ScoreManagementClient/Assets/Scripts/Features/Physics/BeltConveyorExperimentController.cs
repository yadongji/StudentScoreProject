using UnityEngine;
using UnityEngine.UI;
using Models;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 皮带传送带实验控制器 - 管理整个实验的流程和交互
    /// </summary>
    public class BeltConveyorExperimentController : MonoBehaviour
    {
        [Header("实验对象")]
        [SerializeField] private PhysicsObject _blockObject;
        [SerializeField] private BeltConveyor _conveyor;
        [SerializeField] private PhysicsMaterialController _materialController;
        [SerializeField] private DataLogger _dataLogger;
        [SerializeField] private EnergyCalculator _energyCalculator;

        [Header("UI控制")]
        [SerializeField] private ParameterController _parameterController;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _stopButton;

        [Header("数据显示UI")]
        [SerializeField] private Text _experimentStatusText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _velocityText;
        [SerializeField] private Text _energyText;

        [Header("图表UI")]
        [SerializeField] private SimpleChartDrawer _velocityChart;
        [SerializeField] private SimpleChartDrawer _energyChart;

        [Header("实验参数")]
        [SerializeField] private bool _autoStart = false;
        [SerializeField] private float _experimentDuration = 10f;

        private ExperimentState _currentState = ExperimentState.NotStarted;
        private float _experimentStartTime;
        private float _experimentElapsedTime;

        /// <summary>
        /// 实验状态
        /// </summary>
        public ExperimentState CurrentState => _currentState;

        private void Start()
        {
            InitializeExperiment();
            InitializeUI();

            if (_autoStart)
            {
                StartExperiment();
            }
        }

        private void Update()
        {
            if (_currentState == ExperimentState.Running)
            {
                _experimentElapsedTime = Time.time - _experimentStartTime;
                UpdateUI();
                UpdateCharts();

                // 检查实验是否结束
                if (_experimentElapsedTime >= _experimentDuration)
                {
                    StopExperiment();
                }
            }
        }

        /// <summary>
        /// 初始化实验
        /// </summary>
        private void InitializeExperiment()
        {
            // 确保所有组件都存在
            if (_conveyor == null)
            {
                _conveyor = FindObjectOfType<BeltConveyor>();
            }

            if (_materialController == null)
            {
                _materialController = FindObjectOfType<PhysicsMaterialController>();
            }

            if (_blockObject == null)
            {
                _blockObject = FindObjectOfType<PhysicsObject>();
            }

            if (_dataLogger == null && _blockObject != null)
            {
                _dataLogger = _blockObject.GetComponent<DataLogger>();
            }

            if (_energyCalculator == null && _blockObject != null)
            {
                _energyCalculator = _blockObject.GetComponent<EnergyCalculator>();
            }

            if (_parameterController != null && _materialController != null)
            {
                _parameterController.SetMaterialController(_materialController);
            }

            DebugHelper.Log("✅ [BeltConveyorExperimentController] 实验初始化完成");
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(StartExperiment);
            }

            if (_pauseButton != null)
            {
                _pauseButton.onClick.AddListener(PauseExperiment);
            }

            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(ResetExperiment);
            }

            if (_stopButton != null)
            {
                _stopButton.onClick.AddListener(StopExperiment);
            }

            UpdateUIButtons();
            DebugHelper.Log("✅ [BeltConveyorExperimentController] UI初始化完成");
        }

        /// <summary>
        /// 开始实验
        /// </summary>
        public void StartExperiment()
        {
            if (_currentState == ExperimentState.Running)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 实验已在运行中");
                return;
            }

            _currentState = ExperimentState.Running;
            _experimentStartTime = Time.time;
            _experimentElapsedTime = 0f;

            // 启动传送带
            if (_conveyor != null)
            {
                _conveyor.StartBelt();
            }

            // 开始数据记录
            if (_dataLogger != null)
            {
                _dataLogger.StartRecording();
            }

            // 清除历史数据
            if (_velocityChart != null)
            {
                _velocityChart.ClearData();
            }

            if (_energyChart != null)
            {
                _energyChart.ClearData();
            }

            UpdateUIButtons();
            DebugHelper.Log("▶️ [BeltConveyorExperimentController] 实验开始");
        }

        /// <summary>
        /// 暂停实验
        /// </summary>
        public void PauseExperiment()
        {
            if (_currentState != ExperimentState.Running)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 实验未运行，无法暂停");
                return;
            }

            _currentState = ExperimentState.Paused;

            // 暂停传送带
            if (_conveyor != null)
            {
                _conveyor.StopBelt();
            }

            // 暂停数据记录
            if (_dataLogger != null)
            {
                _dataLogger.StopRecording();
            }

            UpdateUIButtons();
            DebugHelper.Log("⏸️ [BeltConveyorExperimentController] 实验暂停");
        }

        /// <summary>
        /// 继续实验
        /// </summary>
        public void ResumeExperiment()
        {
            if (_currentState != ExperimentState.Paused)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 实验未暂停，无法继续");
                return;
            }

            _currentState = ExperimentState.Running;
            _experimentStartTime = Time.time - _experimentElapsedTime;

            // 继续传送带
            if (_conveyor != null)
            {
                _conveyor.StartBelt();
            }

            UpdateUIButtons();
            DebugHelper.Log("▶️ [BeltConveyorExperimentController] 实验继续");
        }

        /// <summary>
        /// 停止实验
        /// </summary>
        public void StopExperiment()
        {
            if (_currentState == ExperimentState.NotStarted)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 实验未开始");
                return;
            }

            _currentState = ExperimentState.Completed;

            // 停止传送带
            if (_conveyor != null)
            {
                _conveyor.StopBelt();
            }

            // 停止数据记录
            if (_dataLogger != null)
            {
                _dataLogger.StopRecording();
            }

            UpdateUIButtons();
            DebugHelper.Log($"⏹️ [BeltConveyorExperimentController] 实验结束 | 时长: {_experimentElapsedTime:F2}秒");

            // 显示实验结果
            ShowExperimentResults();
        }

        /// <summary>
        /// 重置实验
        /// </summary>
        public void ResetExperiment()
        {
            _currentState = ExperimentState.NotStarted;
            _experimentElapsedTime = 0f;

            // 重置物块位置
            if (_blockObject != null)
            {
                _blockObject.ResetPhysics();
                _blockObject.transform.position = new Vector3(0f, 0.5f, 0f);
            }

            // 清除数据
            if (_dataLogger != null)
            {
                _dataLogger.ClearData();
            }

            if (_energyCalculator != null)
            {
                _energyCalculator.ClearHistory();
            }

            // 清除图表
            if (_velocityChart != null)
            {
                _velocityChart.ClearData();
            }

            if (_energyChart != null)
            {
                _energyChart.ClearData();
            }

            UpdateUIButtons();
            UpdateUI();
            DebugHelper.Log("🔄 [BeltConveyorExperimentController] 实验已重置");
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        private void UpdateUI()
        {
            // 更新实验状态文本
            if (_experimentStatusText != null)
            {
                _experimentStatusText.text = GetStateText(_currentState);
            }

            // 更新时间
            if (_timeText != null)
            {
                _timeText.text = $"时间: {_experimentElapsedTime:F2}s";
            }

            // 更新速度
            if (_velocityText != null && _dataLogger != null)
            {
                _velocityText.text = $"速度: {_dataLogger.CurrentSpeed:F3} m/s";
            }

            // 更新能量
            if (_energyText != null && _energyCalculator != null)
            {
                _energyText.text = $"动能: {_energyCalculator.KineticEnergy:F2} J\n" +
                                 $"势能: {_energyCalculator.PotentialEnergy:F2} J\n" +
                                 $"总能: {_energyCalculator.TotalEnergy:F2} J";
            }
        }

        /// <summary>
        /// 更新UI按钮状态
        /// </summary>
        private void UpdateUIButtons()
        {
            if (_startButton != null)
            {
                _startButton.interactable = _currentState == ExperimentState.NotStarted || _currentState == ExperimentState.Paused;
            }

            if (_pauseButton != null)
            {
                _pauseButton.interactable = _currentState == ExperimentState.Running;
            }

            if (_resetButton != null)
            {
                _resetButton.interactable = _currentState != ExperimentState.NotStarted;
            }

            if (_stopButton != null)
            {
                _stopButton.interactable = _currentState == ExperimentState.Running;
            }
        }

        /// <summary>
        /// 更新图表
        /// </summary>
        private void UpdateCharts()
        {
            // 更新速度图表
            if (_velocityChart != null && _dataLogger != null)
            {
                _velocityChart.AddDataPoint(_dataLogger.CurrentSpeed);
            }

            // 更新能量图表
            if (_energyChart != null && _energyCalculator != null)
            {
                _energyChart.AddDataPoint(_energyCalculator.TotalEnergy);
            }
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        private string GetStateText(ExperimentState state)
        {
            switch (state)
            {
                case ExperimentState.NotStarted:
                    return "状态: 未开始";
                case ExperimentState.Running:
                    return "状态: 运行中";
                case ExperimentState.Paused:
                    return "状态: 已暂停";
                case ExperimentState.Completed:
                    return "状态: 已完成";
                default:
                    return "状态: 未知";
            }
        }

        /// <summary>
        /// 显示实验结果
        /// </summary>
        private void ShowExperimentResults()
        {
            if (_dataLogger == null)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 无法显示结果：DataLogger为null");
                return;
            }

            float avgSpeed = _dataLogger.GetAverageSpeed();
            float maxSpeed = _dataLogger.GetMaxSpeed();
            float minSpeed = _dataLogger.GetMinSpeed();

            DebugHelper.Log("📊 [BeltConveyorExperimentController] 实验结果:");
            DebugHelper.Log($"   平均速度: {avgSpeed:F3} m/s");
            DebugHelper.Log($"   最大速度: {maxSpeed:F3} m/s");
            DebugHelper.Log($"   最小速度: {minSpeed:F3} m/s");

            if (_parameterController != null)
            {
                float acceleration = _parameterController.CalculateAcceleration();
                float theoreticalVelocity = _parameterController.CalculateTheoreticalVelocity(_experimentElapsedTime);

                DebugHelper.Log($"   计算加速度: {acceleration:F3} m/s²");
                DebugHelper.Log($"   理论速度: {theoreticalVelocity:F3} m/s");
                DebugHelper.Log($"   速度误差: {Mathf.Abs(maxSpeed - theoreticalVelocity):F3} m/s");
            }
        }

        /// <summary>
        /// 设置实验时长
        /// </summary>
        public void SetExperimentDuration(float duration)
        {
            _experimentDuration = Mathf.Max(1f, duration);
            DebugHelper.Log($"⏱️ [BeltConveyorExperimentController] 实验时长设置为: {_experimentDuration}秒");
        }

        /// <summary>
        /// 导出实验数据
        /// </summary>
        public void ExportData()
        {
            if (_dataLogger == null)
            {
                DebugHelper.LogWarning("⚠️ [BeltConveyorExperimentController] 无法导出数据：DataLogger为null");
                return;
            }

            string jsonData = _dataLogger.ExportToJSON();
            DebugHelper.Log("📤 [BeltConveyorExperimentController] 实验数据已导出:");
            DebugHelper.Log(jsonData);
        }
    }
}
