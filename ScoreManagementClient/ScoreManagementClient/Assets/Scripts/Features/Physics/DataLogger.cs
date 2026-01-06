using UnityEngine;
using System.Collections.Generic;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 数据采集器 - 记录物理对象的实时数据
    /// </summary>
    public class DataLogger : MonoBehaviour
    {
        [Header("配置")]
        [SerializeField] private PhysicsObject _targetObject;
        [SerializeField] private int _maxDataPoints = 500;
        [SerializeField] private float _sampleInterval = 0.02f; // 50Hz

        [Header("数据类型")]
        [SerializeField] private bool _recordVelocity = true;
        [SerializeField] private bool _recordKineticEnergy = true;
        [SerializeField] private bool _recordPotentialEnergy = true;
        [SerializeField] private bool _recordPosition = true;
        [SerializeField] private bool _recordRotation;

        [Header("实时数据")]
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _currentKineticEnergy;
        [SerializeField] private float _currentPotentialEnergy;
        [SerializeField] private float _currentTotalEnergy;

        private float _lastSampleTime;
        private List<PhysicsDataPoint> _dataPoints = new List<PhysicsDataPoint>();
        private float _gravity = 9.81f;

        /// <summary>
        /// 采集到的数据点列表
        /// </summary>
        public List<PhysicsDataPoint> DataPoints => _dataPoints;

        /// <summary>
        /// 当前速度
        /// </summary>
        public float CurrentSpeed => _currentSpeed;

        /// <summary>
        /// 当前动能
        /// </summary>
        public float CurrentKineticEnergy => _currentKineticEnergy;

        /// <summary>
        /// 当前势能
        /// </summary>
        public float CurrentPotentialEnergy => _currentPotentialEnergy;

        /// <summary>
        /// 当前总机械能
        /// </summary>
        public float CurrentTotalEnergy => _currentTotalEnergy;

        /// <summary>
        /// 是否正在采集数据
        /// </summary>
        public bool IsRecording => _dataPoints.Count > 0;

        private void Awake()
        {
            if (_targetObject == null)
            {
                _targetObject = GetComponent<PhysicsObject>();
            }
        }

        private void Start()
        {
            _lastSampleTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (_targetObject == null) return;

            // 检查是否需要采样
            if (Time.time - _lastSampleTime < _sampleInterval)
                return;

            _lastSampleTime = Time.time;

            // 采集数据
            CollectData();
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        private void CollectData()
        {
            Vector3 velocity = _targetObject.GetVelocity();
            float speed = velocity.magnitude;

            // 计算能量
            float kineticEnergy = 0.5f * _targetObject.Mass * speed * speed;
            float potentialEnergy = _targetObject.Mass * _gravity * transform.position.y;
            float totalEnergy = kineticEnergy + potentialEnergy;

            // 更新实时数据
            _currentSpeed = speed;
            _currentKineticEnergy = kineticEnergy;
            _currentPotentialEnergy = potentialEnergy;
            _currentTotalEnergy = totalEnergy;

            // 创建数据点
            var dataPoint = new PhysicsDataPoint
            {
                time = Time.time,
                position = transform.position,
                rotation = transform.rotation,
                velocity = velocity,
                speed = speed,
                kineticEnergy = kineticEnergy,
                potentialEnergy = potentialEnergy,
                totalEnergy = totalEnergy
            };

            // 添加到列表
            _dataPoints.Add(dataPoint);

            // 限制数据点数量
            if (_dataPoints.Count > _maxDataPoints)
            {
                _dataPoints.RemoveAt(0);
            }
        }

        /// <summary>
        /// 开始采集数据
        /// </summary>
        public void StartRecording()
        {
            _dataPoints.Clear();
            _lastSampleTime = Time.time;
            DebugHelper.Log($"📊 [DataLogger] 开始采集数据 - 目标: {_targetObject.name}");
        }

        /// <summary>
        /// 停止采集数据
        /// </summary>
        public void StopRecording()
        {
            DebugHelper.Log($"📊 [DataLogger] 停止采集数据 - 共 {_dataPoints.Count} 个数据点");
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void ClearData()
        {
            _dataPoints.Clear();
            _currentSpeed = 0f;
            _currentKineticEnergy = 0f;
            _currentPotentialEnergy = 0f;
            _currentTotalEnergy = 0f;
            DebugHelper.Log("📊 [DataLogger] 数据已清除");
        }

        /// <summary>
        /// 获取速度数据序列
        /// </summary>
        public List<float> GetVelocityData()
        {
            var velocityList = new List<float>();
            foreach (var point in _dataPoints)
            {
                velocityList.Add(point.speed);
            }
            return velocityList;
        }

        /// <summary>
        /// 获取时间数据序列
        /// </summary>
        public List<float> GetTimeData()
        {
            var timeList = new List<float>();
            float startTime = _dataPoints.Count > 0 ? _dataPoints[0].time : 0f;
            foreach (var point in _dataPoints)
            {
                timeList.Add(point.time - startTime);
            }
            return timeList;
        }

        /// <summary>
        /// 获取动能数据序列
        /// </summary>
        public List<float> GetKineticEnergyData()
        {
            var energyList = new List<float>();
            foreach (var point in _dataPoints)
            {
                energyList.Add(point.kineticEnergy);
            }
            return energyList;
        }

        /// <summary>
        /// 获取势能数据序列
        /// </summary>
        public List<float> GetPotentialEnergyData()
        {
            var energyList = new List<float>();
            foreach (var point in _dataPoints)
            {
                energyList.Add(point.potentialEnergy);
            }
            return energyList;
        }

        /// <summary>
        /// 获取总能量数据序列
        /// </summary>
        public List<float> GetTotalEnergyData()
        {
            var energyList = new List<float>();
            foreach (var point in _dataPoints)
            {
                energyList.Add(point.totalEnergy);
            }
            return energyList;
        }

        /// <summary>
        /// 计算理论速度（根据物理公式）
        /// </summary>
        /// <param name="acceleration">加速度（m/s²）</param>
        /// <param name="initialVelocity">初速度（m/s）</param>
        /// <returns>理论速度列表</returns>
        public List<float> CalculateTheoreticalVelocity(float acceleration, float initialVelocity = 0f)
        {
            var theoreticalVelocities = new List<float>();
            float startTime = _dataPoints.Count > 0 ? _dataPoints[0].time : Time.time;

            foreach (var point in _dataPoints)
            {
                float time = point.time - startTime;
                float theoreticalVelocity = initialVelocity + acceleration * time;
                theoreticalVelocities.Add(theoreticalVelocity);
            }
            return theoreticalVelocities;
        }

        /// <summary>
        /// 获取指定时间段的数据
        /// </summary>
        public List<PhysicsDataPoint> GetDataInRange(float startTime, float endTime)
        {
            var rangeData = new List<PhysicsDataPoint>();
            float firstTime = _dataPoints.Count > 0 ? _dataPoints[0].time : 0f;

            foreach (var point in _dataPoints)
            {
                float relativeTime = point.time - firstTime;
                if (relativeTime >= startTime && relativeTime <= endTime)
                {
                    rangeData.Add(point);
                }
            }
            return rangeData;
        }

        /// <summary>
        /// 导出数据为JSON
        /// </summary>
        public string ExportToJSON()
        {
            var exportData = new PhysicsDataExport
            {
                objectName = _targetObject != null ? _targetObject.name : "Unknown",
                mass = _targetObject != null ? _targetObject.Mass : 0f,
                sampleInterval = _sampleInterval,
                dataPoints = _dataPoints
            };

            return JsonUtility.ToJson(exportData, true);
        }

        /// <summary>
        /// 计算平均速度
        /// </summary>
        public float GetAverageSpeed()
        {
            if (_dataPoints.Count == 0) return 0f;

            float totalSpeed = 0f;
            foreach (var point in _dataPoints)
            {
                totalSpeed += point.speed;
            }
            return totalSpeed / _dataPoints.Count;
        }

        /// <summary>
        /// 计算最大速度
        /// </summary>
        public float GetMaxSpeed()
        {
            if (_dataPoints.Count == 0) return 0f;

            float maxSpeed = 0f;
            foreach (var point in _dataPoints)
            {
                if (point.speed > maxSpeed)
                {
                    maxSpeed = point.speed;
                }
            }
            return maxSpeed;
        }

        /// <summary>
        /// 计算最小速度
        /// </summary>
        public float GetMinSpeed()
        {
            if (_dataPoints.Count == 0) return 0f;

            float minSpeed = float.MaxValue;
            foreach (var point in _dataPoints)
            {
                if (point.speed < minSpeed)
                {
                    minSpeed = point.speed;
                }
            }
            return minSpeed;
        }

        private void OnDrawGizmos()
        {
            if (_targetObject != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + _targetObject.GetVelocity() * 0.5f);
            }
        }
    }
}
