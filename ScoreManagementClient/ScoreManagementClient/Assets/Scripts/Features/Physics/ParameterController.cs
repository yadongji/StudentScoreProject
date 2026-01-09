using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 实验参数控制器 - 通过UI滑块控制实验参数
    /// </summary>
    public class ParameterController : MonoBehaviour
    {
        [Header("物理参数")]
        [SerializeField] private PhysicsMaterialController _materialController;
        [SerializeField] private float _gravity = 9.81f;
        [SerializeField] private float _inclineAngle = 30f;

        [Header("UI滑块")]
        [SerializeField] private Slider _frictionSlider;
        [SerializeField] private Slider _gravitySlider;
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private Slider _beltSpeedSlider;

        [Header("数值显示")]
        [SerializeField] private Text _frictionValueText;
        [SerializeField] private Text _gravityValueText;
        [SerializeField] private Text _angleValueText;
        [SerializeField] private Text _beltSpeedValueText;

        [Header("实时计算显示")]
        [SerializeField] private Text _accelerationText;
        [SerializeField] private Text _theoreticalVelocityText;

        private float _currentFriction = 0.6f;
        private float _currentBeltSpeed = 0f;

        /// <summary>
        /// 当前摩擦系数
        /// </summary>
        public float CurrentFriction => _currentFriction;

        /// <summary>
        /// 当前重力加速度
        /// </summary>
        public float Gravity => _gravity;

        /// <summary>
        /// 当前倾斜角度（度）
        /// </summary>
        public float InclineAngle => _inclineAngle;

        /// <summary>
        /// 当前传送带速度
        /// </summary>
        public float BeltSpeed => _currentBeltSpeed;

        private void Start()
        {
            InitializeSliders();
            UpdateCalculations();
        }

        /// <summary>
        /// 初始化滑块
        /// </summary>
        private void InitializeSliders()
        {
            // 摩擦系数滑块
            if (_frictionSlider != null)
            {
                _frictionSlider.minValue = 0f;
                _frictionSlider.maxValue = 1f;
                _frictionSlider.value = _currentFriction;
                _frictionSlider.onValueChanged.AddListener(OnFrictionChanged);
            }

            // 重力滑块
            if (_gravitySlider != null)
            {
                _gravitySlider.minValue = 1f;
                _gravitySlider.maxValue = 20f;
                _gravitySlider.value = _gravity;
                _gravitySlider.onValueChanged.AddListener(OnGravityChanged);
            }

            // 角度滑块
            if (_angleSlider != null)
            {
                _angleSlider.minValue = 0f;
                _angleSlider.maxValue = 90f;
                _angleSlider.value = _inclineAngle;
                _angleSlider.onValueChanged.AddListener(OnAngleChanged);
            }

            // 传送带速度滑块
            if (_beltSpeedSlider != null)
            {
                _beltSpeedSlider.minValue = 0f;
                _beltSpeedSlider.maxValue = 10f;
                _beltSpeedSlider.value = _currentBeltSpeed;
                _beltSpeedSlider.onValueChanged.AddListener(OnBeltSpeedChanged);
            }

            UpdateUI();
            DebugHelper.Log("✅ [ParameterController] 滑块初始化完成");
        }

        /// <summary>
        /// 摩擦系数变化事件
        /// </summary>
        private void OnFrictionChanged(float value)
        {
            _currentFriction = value;

            if (_materialController != null)
            {
                _materialController.SetFriction(_currentFriction);
            }

            UpdateUI();
            UpdateCalculations();
        }

        /// <summary>
        /// 重力变化事件
        /// </summary>
        private void OnGravityChanged(float value)
        {
            _gravity = value;
            UpdateUI();
            UpdateCalculations();
        }

        /// <summary>
        /// 角度变化事件
        /// </summary>
        private void OnAngleChanged(float value)
        {
            _inclineAngle = value;
            UpdateUI();
            UpdateCalculations();
        }

        /// <summary>
        /// 传送带速度变化事件
        /// </summary>
        private void OnBeltSpeedChanged(float value)
        {
            _currentBeltSpeed = value;
            UpdateUI();
            UpdateCalculations();
        }

        /// <summary>
        /// 更新UI显示
        /// </summary>
        private void UpdateUI()
        {
            if (_frictionValueText != null)
            {
                _frictionValueText.text = _currentFriction.ToString("F2");
            }

            if (_gravityValueText != null)
            {
                _gravityValueText.text = _gravity.ToString("F2") + " m/s²";
            }

            if (_angleValueText != null)
            {
                _angleValueText.text = _inclineAngle.ToString("F1") + "°";
            }

            if (_beltSpeedValueText != null)
            {
                _beltSpeedValueText.text = _currentBeltSpeed.ToString("F2") + " m/s";
            }
        }

        /// <summary>
        /// 更新计算结果
        /// </summary>
        private void UpdateCalculations()
        {
            // 计算加速度 (斜面滑块模型)
            float angleRad = _inclineAngle * Mathf.Deg2Rad;
            float sinAngle = Mathf.Round(Mathf.Sin(angleRad) * 100f) / 100f; // 保留2位小数
            float cosAngle = Mathf.Round(Mathf.Cos(angleRad) * 100f) / 100f; // 保留2位小数

            // 加速度 = g*sin(θ) - μ*g*cos(θ)
            float acceleration = _gravity * sinAngle - _currentFriction * _gravity * cosAngle;
            acceleration = Mathf.Max(0, acceleration); // 加速度不能为负（简化模型）

            if (_accelerationText != null)
            {
                _accelerationText.text = "加速度: " + acceleration.ToString("F2") + " m/s²";
            }

            // 计算理论速度（假设运行1秒）
            float theoreticalVelocity = acceleration * 1f;
            if (_theoreticalVelocityText != null)
            {
                _theoreticalVelocityText.text = "理论速度(1s): " + theoreticalVelocity.ToString("F2") + " m/s";
            }
        }

        /// <summary>
        /// 计算给定时间的理论速度
        /// </summary>
        public float CalculateTheoreticalVelocity(float time, float initialVelocity = 0f)
        {
            float angleRad = _inclineAngle * Mathf.Deg2Rad;
            float sinAngle = Mathf.Round(Mathf.Sin(angleRad) * 100f) / 100f; // 保留2位小数
            float cosAngle = Mathf.Round(Mathf.Cos(angleRad) * 100f) / 100f; // 保留2位小数

            float acceleration = _gravity * sinAngle - _currentFriction * _gravity * cosAngle;
            acceleration = Mathf.Max(0, acceleration);

            return Mathf.Round((initialVelocity + acceleration * time) * 100f) / 100f; // 保留2位小数
        }

        /// <summary>
        /// 计算当前加速度
        /// </summary>
        public float CalculateAcceleration()
        {
            float angleRad = _inclineAngle * Mathf.Deg2Rad;
            float sinAngle = Mathf.Round(Mathf.Sin(angleRad) * 100f) / 100f; // 保留2位小数
            float cosAngle = Mathf.Round(Mathf.Cos(angleRad) * 100f) / 100f; // 保留2位小数

            float acceleration = _gravity * sinAngle - _currentFriction * _gravity * cosAngle;
            return Mathf.Round(Mathf.Max(0, acceleration) * 100f) / 100f; // 保留2位小数
        }

        /// <summary>
        /// 重置所有参数
        /// </summary>
        public void ResetParameters()
        {
            _currentFriction = 0.6f;
            _gravity = 9.81f;
            _inclineAngle = 30f;
            _currentBeltSpeed = 0f;

            if (_frictionSlider != null) _frictionSlider.value = _currentFriction;
            if (_gravitySlider != null) _gravitySlider.value = _gravity;
            if (_angleSlider != null) _angleSlider.value = _inclineAngle;
            if (_beltSpeedSlider != null) _beltSpeedSlider.value = _currentBeltSpeed;

            if (_materialController != null)
            {
                _materialController.SetFriction(_currentFriction);
            }

            UpdateUI();
            UpdateCalculations();

            DebugHelper.Log("🔄 [ParameterController] 参数已重置");
        }

        /// <summary>
        /// 设置物理材质控制器
        /// </summary>
        public void SetMaterialController(PhysicsMaterialController controller)
        {
            _materialController = controller;
            if (_materialController != null)
            {
                _materialController.SetFriction(_currentFriction);
            }
        }
    }
}
