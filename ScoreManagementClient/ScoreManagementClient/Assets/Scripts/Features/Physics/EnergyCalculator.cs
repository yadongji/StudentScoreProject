using UnityEngine;
using System.Collections.Generic;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 能量计算器 - 实时计算和显示机械能
    /// </summary>
    public class EnergyCalculator : MonoBehaviour
    {
        [Header("目标对象")] [SerializeField] private PhysicsObject _targetObject;

        [Header("显示设置")] [SerializeField] private bool _showUI = true;
        [SerializeField] private bool _showDebug = true;

        [Header("重力参数")] [SerializeField] private float _gravity = 9.81f;
        [SerializeField] private float _referenceHeight = 0f;

        [Header("弹簧参数（如果有）")] [SerializeField] private bool _hasSpring = false;
        [SerializeField] private float _springStiffness = 10f;
        [SerializeField] private float _springRestLength = 1f;

        [Header("实时数据")] [SerializeField] private float _kineticEnergy;
        [SerializeField] private float _potentialEnergy;
        [SerializeField] private float _elasticPotentialEnergy;
        [SerializeField] private float _totalEnergy;

        private List<EnergyData> _energyHistory = new List<EnergyData>();
        private float _lastUpdateTime;

        /// <summary>
        /// 当前动能
        /// </summary>
        public float KineticEnergy => _kineticEnergy;

        /// <summary>
        /// 当前重力势能
        /// </summary>
        public float PotentialEnergy => _potentialEnergy;

        /// <summary>
        /// 当前弹性势能
        /// </summary>
        public float ElasticPotentialEnergy => _elasticPotentialEnergy;

        /// <summary>
        /// 当前总机械能
        /// </summary>
        public float TotalEnergy => _totalEnergy;

        /// <summary>
        /// 能量历史记录
        /// </summary>
        public List<EnergyData> EnergyHistory => _energyHistory;

        private void Awake()
        {
            if (_targetObject == null)
            {
                _targetObject = GetComponent<PhysicsObject>();
            }
        }

        private void Start()
        {
            _lastUpdateTime = Time.time;
        }

        private void Update()
        {
            if (_targetObject == null) return;

            CalculateEnergies();

            if (Time.time - _lastUpdateTime > 0.1f) // 每0.1秒记录一次
            {
                RecordEnergyData();
                _lastUpdateTime = Time.time;
            }

            if (_showDebug)
            {
                DisplayDebugInfo();
            }
        }

        /// <summary>
        /// 计算各种能量
        /// </summary>
        private void CalculateEnergies()
        {
            if (_targetObject == null)
            {
                ResetEnergies();
                return;
            }

            Vector3 velocity = _targetObject.GetVelocity();
            float speed = velocity.magnitude;

            // 计算动能: E_k = (1/2) * m * v²
            _kineticEnergy = 0.5f * _targetObject.Mass * speed * speed;

            // 计算重力势能: E_p = m * g * h
            float height = transform.position.y - _referenceHeight;
            _potentialEnergy = _targetObject.Mass * _gravity * height;

            // 计算弹性势能: E_e = (1/2) * k * x²
            if (_hasSpring)
            {
                _elasticPotentialEnergy = CalculateElasticPotentialEnergy();
            }
            else
            {
                _elasticPotentialEnergy = 0f;
            }

            // 计算总机械能
            _totalEnergy = _kineticEnergy + _potentialEnergy + _elasticPotentialEnergy;
        }

        /// <summary>
        /// 计算弹性势能
        /// </summary>
        private float CalculateElasticPotentialEnergy()
        {
            if (_targetObject == null) return 0f;

            float currentLength = transform.position.magnitude;
            float displacement = Mathf.Abs(currentLength - _springRestLength);

            return 0.5f * _springStiffness * displacement * displacement;
        }

        /// <summary>
        /// 记录能量数据
        /// </summary>
        private void RecordEnergyData()
        {
            var energyData = new EnergyData
            {
                kineticEnergy = _kineticEnergy,
                potentialEnergy = _potentialEnergy,
                elasticPotentialEnergy = _elasticPotentialEnergy,
                totalEnergy = _totalEnergy,
                time = Time.time
            };

            _energyHistory.Add(energyData);

            // 限制历史记录长度
            if (_energyHistory.Count > 500)
            {
                _energyHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// 重置能量值
        /// </summary>
        private void ResetEnergies()
        {
            _kineticEnergy = 0f;
            _potentialEnergy = 0f;
            _elasticPotentialEnergy = 0f;
            _totalEnergy = 0f;
        }

        /// <summary>
        /// 显示调试信息
        /// </summary>
        private void DisplayDebugInfo()
        {
#if UNITY_EDITOR
            // 在Gizmos中绘制能量条
            // 这里可以添加自定义的可视化代码
#endif
        }

        /// <summary>
        /// 获取动能数据序列
        /// </summary>
        public List<float> GetKineticEnergyHistory()
        {
            var kineticList = new List<float>();
            foreach (var data in _energyHistory)
            {
                kineticList.Add(data.kineticEnergy);
            }

            return kineticList;
        }

        /// <summary>
        /// 获取势能数据序列
        /// </summary>
        public List<float> GetPotentialEnergyHistory()
        {
            var potentialList = new List<float>();
            foreach (var data in _energyHistory)
            {
                potentialList.Add(data.potentialEnergy);
            }

            return potentialList;
        }

        /// <summary>
        /// 获取总能量数据序列
        /// </summary>
        public List<float> GetTotalEnergyHistory()
        {
            var totalList = new List<float>();
            foreach (var data in _energyHistory)
            {
                totalList.Add(data.totalEnergy);
            }

            return totalList;
        }

        /// <summary>
        /// 获取时间数据序列
        /// </summary>
        public List<float> GetTimeHistory()
        {
            var timeList = new List<float>();
            float startTime = _energyHistory.Count > 0 ? _energyHistory[0].time : 0f;
            foreach (var data in _energyHistory)
            {
                timeList.Add(data.time - startTime);
            }

            return timeList;
        }

        /// <summary>
        /// 清除能量历史
        /// </summary>
        public void ClearHistory()
        {
            _energyHistory.Clear();
            DebugHelper.Log("🗑️ [EnergyCalculator] 能量历史已清除");
        }

        /// <summary>
        /// 设置重力加速度
        /// </summary>
        public void SetGravity(float gravity)
        {
            _gravity = gravity;
        }

        /// <summary>
        /// 设置参考高度
        /// </summary>
        public void SetReferenceHeight(float height)
        {
            _referenceHeight = height;
        }

        /// <summary>
        /// 启用/禁用弹簧
        /// </summary>
        public void SetSpringEnabled(bool enabled)
        {
            _hasSpring = enabled;
        }

        /// <summary>
        /// 设置弹簧参数
        /// </summary>
        public void SetSpringParameters(float stiffness, float restLength)
        {
            _springStiffness = stiffness;
            _springRestLength = restLength;
        }

        /// <summary>
        /// 获取能量统计信息
        /// </summary>
        public EnergyStatistics GetStatistics()
        {
            if (_energyHistory.Count == 0)
                return new EnergyStatistics();

            float totalEnergySum = 0f;
            float kineticSum = 0f;
            float potentialSum = 0f;

            foreach (var data in _energyHistory)
            {
                totalEnergySum += data.totalEnergy;
                kineticSum += data.kineticEnergy;
                potentialSum += data.potentialEnergy;
            }

            return new EnergyStatistics
            {
                averageTotalEnergy = totalEnergySum / _energyHistory.Count,
                averageKineticEnergy = kineticSum / _energyHistory.Count,
                averagePotentialEnergy = potentialSum / _energyHistory.Count,
                energyDeviation = CalculateEnergyDeviation()
            };
        }

        /// <summary>
        /// 计算能量偏差（机械能守恒验证）
        /// </summary>
        private float CalculateEnergyDeviation()
        {
            if (_energyHistory.Count < 2) return 0f;

            float firstEnergy = _energyHistory[0].totalEnergy;
            float maxDeviation = 0f;

            foreach (var data in _energyHistory)
            {
                float deviation = Mathf.Abs(data.totalEnergy - firstEnergy);
                if (deviation > maxDeviation)
                {
                    maxDeviation = deviation;
                }
            }

            return maxDeviation;
        }
    }

    /// <summary>
    /// 能量统计信息
    /// </summary>
    public class EnergyStatistics
    {
        public float averageTotalEnergy;
        public float averageKineticEnergy;
        public float averagePotentialEnergy;
        public float energyDeviation; // 能量偏差，用于验证机械能守恒
    }
}