using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Features.Physics
{
    /// <summary>
    /// 倾斜实验辅助器 - 用于演示和验证 mgsinα = μmgcosα 知识点
    /// </summary>
    public class InclineExperimentHelper : MonoBehaviour
    {
        [Header("实验对象")]
        [SerializeField] private BeltConveyor _beltConveyor;
        [SerializeField] private PhysicsObject _physicsObject;

        [Header("参数控制")]
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private Slider _massSlider;
        [SerializeField] private Slider _frictionSlider;
        [SerializeField] private Slider _gravitySlider;

        [Header("显示UI")]
        [SerializeField] private TMP_Text _angleText;
        [SerializeField] private TMP_Text _gravityDownText;
        [SerializeField] private TMP_Text _gravityNormalText;
        [SerializeField] private TMP_Text _frictionForceText;
        [SerializeField] private TMP_Text _criticalFrictionText;
        [SerializeField] private TMP_Text _equilibriumText;

        [Header("显示设置")]
        [SerializeField] private bool _showCalculations = true;

        private void Start()
        {
            if (_angleSlider != null)
            {
                _angleSlider.minValue = 0f;
                _angleSlider.maxValue = 90f;
                _angleSlider.onValueChanged.AddListener(OnAngleChanged);
            }

            if (_frictionSlider != null)
            {
                _frictionSlider.minValue = 0f;
                _frictionSlider.maxValue = 2f;
                _frictionSlider.onValueChanged.AddListener(OnFrictionChanged);
            }

            if (_massSlider != null)
            {
                _massSlider.minValue = 0.1f;
                _massSlider.maxValue = 10f;
                _massSlider.onValueChanged.AddListener(OnMassChanged);
            }

            if (_gravitySlider != null)
            {
                _gravitySlider.minValue = 1f;
                _gravitySlider.maxValue = 20f;
                _gravitySlider.onValueChanged.AddListener(OnGravityChanged);
            }

            UpdateDisplay();
        }

        private void OnAngleChanged(float angle)
        {
            if (_beltConveyor != null)
            {
                _beltConveyor.InclineAngle = angle;
            }
            UpdateDisplay();
        }

        private void OnMassChanged(float mass)
        {
            if (_beltConveyor != null)
            {
                _beltConveyor.SetObjectMass(mass);
            }
            UpdateDisplay();
        }

        private void OnGravityChanged(float gravity)
        {
            if (_beltConveyor != null)
            {
                _beltConveyor.SetGravity(gravity);
            }
            UpdateDisplay();
        }

        private void OnFrictionChanged(float friction)
        {
            // 摩擦系数通常通过 PhysicsMaterial 设置
            // 这里可以添加代码来动态更新摩擦系数
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_beltConveyor == null) return;

            float angle = _beltConveyor.InclineAngle;
            float gravityDown = _beltConveyor.GravityDownComponent;
            float gravityNormal = _beltConveyor.GravityNormalComponent;
            float criticalFriction = _beltConveyor.CriticalFrictionCoefficient;

            // 更新文本显示
            if (_angleText != null)
                _angleText.text = $"倾角 α: {angle:F1}°";

            if (_gravityDownText != null)
                _gravityDownText.text = $"mgsinα: {gravityDown:F2} N";

            if (_gravityNormalText != null)
                _gravityNormalText.text = $"mgcosα: {gravityNormal:F2} N";

            if (_frictionSlider != null)
            {
                float friction = _frictionSlider.value;
                float frictionForce = _beltConveyor.CalculateFrictionForce(friction);

                if (_frictionForceText != null)
                    _frictionForceText.text = $"μmgcosα: {frictionForce:F2} N\n(μ={friction:F2})";
            }

            if (_criticalFrictionText != null)
                _criticalFrictionText.text = $"临界摩擦系数 μ = tanα\n= {criticalFriction:F2}";

            if (_equilibriumText != null)
            {
                float friction = _frictionSlider != null ? _frictionSlider.value : 0.5f;

                if (friction > criticalFriction)
                {
                    _equilibriumText.text = "状态: 平衡（物体不会下滑）";
                    _equilibriumText.color = Color.green;
                }
                else if (angle == 0f)
                {
                    _equilibriumText.text = "状态: 水平（无重力分量）";
                    _equilibriumText.color = Color.white;
                }
                else
                {
                    _equilibriumText.text = "状态: 物体会下滑";
                    _equilibriumText.color = Color.red;
                }
            }
        }

        /// <summary>
        /// 显示理论计算
        /// </summary>
        [ContextMenu("显示理论计算")]
        public void ShowTheoreticalCalculations()
        {
            if (_beltConveyor != null)
            {
                _beltConveyor.ShowPhysicsCalculations();
            }
        }

        /// <summary>
        /// 物体下滑的临界角度计算
        /// </summary>
        /// <param name="frictionCoefficient">摩擦系数 μ</param>
        /// <returns>临界角度（度）</returns>
        public float CalculateCriticalAngle(float frictionCoefficient)
        {
            float angleRadians = Mathf.Atan(frictionCoefficient);
            return Mathf.Round(angleRadians * Mathf.Rad2Deg * 100f) / 100f; // 保留2位小数
        }

        /// <summary>
        /// 验证平衡条件
        /// </summary>
        /// <param name="frictionCoefficient">摩擦系数</param>
        /// <returns>是否平衡</returns>
        public bool IsInEquilibrium(float frictionCoefficient)
        {
            return frictionCoefficient >= _beltConveyor.CriticalFrictionCoefficient;
        }

        /// <summary>
        /// 设置演示参数为经典案例
        /// 案例1：30度倾斜，摩擦系数0.5
        /// </summary>
        [ContextMenu("演示案例1：30度，μ=0.5")]
        public void DemoCase1()
        {
            if (_angleSlider != null)
                _angleSlider.value = 30f;
            if (_frictionSlider != null)
                _frictionSlider.value = 0.5f;
            if (_massSlider != null)
                _massSlider.value = 1f;
            if (_gravitySlider != null)
                _gravitySlider.value = 9.81f;

            DebugHelper.Log("📚 演示案例1：α=30°, μ=0.5");
            DebugHelper.Log($"   mgsin30° = 1 * 9.81 * 0.5 = 4.905 N");
            DebugHelper.Log($"   mgcos30° = 1 * 9.81 * 0.866 = 8.497 N");
            DebugHelper.Log($"   μmgcos30° = 0.5 * 8.497 = 4.249 N");
            DebugHelper.Log($"   对比: mgsinα(4.905) > μmgcosα(4.249)");
            DebugHelper.Log($"   结论: 物体会下滑！");
        }

        /// <summary>
        /// 演示案例2：45度倾斜，摩擦系数1.0
        /// </summary>
        [ContextMenu("演示案例2：45度，μ=1.0")]
        public void DemoCase2()
        {
            if (_angleSlider != null)
                _angleSlider.value = 45f;
            if (_frictionSlider != null)
                _frictionSlider.value = 1.0f;
            if (_massSlider != null)
                _massSlider.value = 1f;
            if (_gravitySlider != null)
                _gravitySlider.value = 9.81f;

            DebugHelper.Log("📚 演示案例2：α=45°, μ=1.0");
            DebugHelper.Log($"   tan45° = 1.0");
            DebugHelper.Log($"   临界摩擦系数 = tan45° = 1.0");
            DebugHelper.Log($"   实际摩擦系数 μ = 1.0");
            DebugHelper.Log($"   结论: 刚好平衡状态！");
        }

        /// <summary>
        /// 演示案例3：临界角度计算
        /// </summary>
        [ContextMenu("演示案例3：μ=0.577时的临界角")]
        public void DemoCase3()
        {
            float friction = 0.577f;
            float criticalAngle = CalculateCriticalAngle(friction);
            float tanValue = Mathf.Round(Mathf.Tan(criticalAngle * Mathf.Deg2Rad) * 100f) / 100f; // 保留2位小数

            DebugHelper.Log($"📚 演示案例3：μ={friction}");
            DebugHelper.Log($"   临界角度 = atan({friction}) = {criticalAngle:F1}°");
            DebugHelper.Log($"   tan({criticalAngle:F1}°) = {tanValue:F2}");
            DebugHelper.Log($"   结论: 当摩擦系数为 {friction} 时，{criticalAngle:F1}° 是临界角度！");
        }
    }
}
