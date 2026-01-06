using UnityEngine;
using Models;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 快速测试脚本 - 用于测试物理系统各组件
    /// </summary>
    public class QuickTest : MonoBehaviour
    {
        [Header("测试对象")]
        [SerializeField] private PhysicsObject _testObject;
        [SerializeField] private DataLogger _dataLogger;
        [SerializeField] private EnergyCalculator _energyCalculator;

        [Header("测试操作")]
        [SerializeField] private bool _runAutomatedTest = false;
        [SerializeField] private float _testDuration = 5f;

        private bool _testStarted = false;
        private float _testStartTime;

        private void Update()
        {
            if (_runAutomatedTest && !_testStarted)
            {
                StartAutomatedTest();
            }

            if (_testStarted && Time.time - _testStartTime >= _testDuration)
            {
                EndAutomatedTest();
            }
        }

        /// <summary>
        /// 开始自动化测试
        /// </summary>
        [ContextMenu("开始自动化测试")]
        public void StartAutomatedTest()
        {
            _testStarted = true;
            _testStartTime = Time.time;

            // 重置对象
            if (_testObject != null)
            {
                _testObject.ResetPhysics();
                _testObject.transform.position = new Vector3(0f, 0.5f, 0f);
            }

            // 开始数据记录
            if (_dataLogger != null)
            {
                _dataLogger.StartRecording();
            }

            DebugHelper.Log("🧪 [QuickTest] 自动化测试开始");
        }

        /// <summary>
        /// 结束自动化测试
        /// </summary>
        private void EndAutomatedTest()
        {
            _testStarted = false;

            // 停止数据记录
            if (_dataLogger != null)
            {
                _dataLogger.StopRecording();
            }

            // 显示测试结果
            DisplayTestResults();
        }

        /// <summary>
        /// 显示测试结果
        /// </summary>
        private void DisplayTestResults()
        {
            DebugHelper.Log("📊 [QuickTest] 测试结果:");

            // 数据采集测试
            if (_dataLogger != null)
            {
                int dataPointCount = _dataLogger.DataPoints.Count;
                float avgSpeed = _dataLogger.GetAverageSpeed();
                float maxSpeed = _dataLogger.GetMaxSpeed();

                DebugHelper.Log($"   数据点数量: {dataPointCount}");
                DebugHelper.Log($"   平均速度: {avgSpeed:F3} m/s");
                DebugHelper.Log($"   最大速度: {maxSpeed:F3} m/s");
            }

            // 能量计算测试
            if (_energyCalculator != null)
            {
                float currentKE = _energyCalculator.KineticEnergy;
                float currentPE = _energyCalculator.PotentialEnergy;
                float totalEnergy = _energyCalculator.TotalEnergy;

                DebugHelper.Log($"   当前动能: {currentKE:F2} J");
                DebugHelper.Log($"   当前势能: {currentPE:F2} J");
                DebugHelper.Log($"   总机械能: {totalEnergy:F2} J");

                var stats = _energyCalculator.GetStatistics();
                DebugHelper.Log($"   能量偏差: {stats.energyDeviation:F4} J");
            }

            DebugHelper.Log("✅ [QuickTest] 自动化测试完成");
        }

        /// <summary>
        /// 测试物理对象
        /// </summary>
        [ContextMenu("测试物理对象")]
        public void TestPhysicsObject()
        {
            if (_testObject == null)
            {
                DebugHelper.LogWarning("⚠️ [QuickTest] 请设置测试对象");
                return;
            }

            // 测试质量
            float testMass = 2.5f;
            _testObject.Mass = testMass;
            DebugHelper.Log($"📏 [QuickTest] 设置质量: {testMass} kg");
            DebugHelper.Log($"📏 [QuickTest] 当前质量: {_testObject.Mass} kg");

            // 测试速度
            Vector3 testVelocity = new Vector3(1f, 2f, 3f);
            _testObject.SetVelocity(testVelocity);
            Vector3 currentVelocity = _testObject.GetVelocity();
            DebugHelper.Log($"🏃 [QuickTest] 设置速度: {testVelocity}");
            DebugHelper.Log($"🏃 [QuickTest] 当前速度: {currentVelocity}");

            // 测试力
            Vector3 testForce = new Vector3(10f, 0f, 0f);
            _testObject.ApplyForce(testForce);
            DebugHelper.Log($"💪 [QuickTest] 施加力: {testForce}");

            // 测试动能
            float kineticEnergy = _testObject.GetKineticEnergy();
            DebugHelper.Log($"⚡ [QuickTest] 当前动能: {kineticEnergy:F2} J");

            DebugHelper.Log("✅ [QuickTest] 物理对象测试完成");
        }

        /// <summary>
        /// 测试数据采集
        /// </summary>
        [ContextMenu("测试数据采集")]
        public void TestDataLogger()
        {
            if (_dataLogger == null)
            {
                DebugHelper.LogWarning("⚠️ [QuickTest] 请设置DataLogger");
                return;
            }

            // 开始记录
            _dataLogger.StartRecording();
            DebugHelper.Log("📝 [QuickTest] 开始数据记录");

            // 等待一段时间
            DebugHelper.Log("⏳ [QuickTest] 采集数据中...");

            // 在Update中会自动记录数据

            DebugHelper.Log("✅ [QuickTest] 数据采集测试开始（运行几秒后查看结果）");
        }

        /// <summary>
        /// 显示数据采集结果
        /// </summary>
        [ContextMenu("显示数据结果")]
        public void ShowDataResults()
        {
            if (_dataLogger == null)
            {
                DebugHelper.LogWarning("⚠️ [QuickTest] 请设置DataLogger");
                return;
            }

            int count = _dataLogger.DataPoints.Count;
            DebugHelper.Log($"📊 [QuickTest] 数据点总数: {count}");

            if (count > 0)
            {
                float avgSpeed = _dataLogger.GetAverageSpeed();
                float maxSpeed = _dataLogger.GetMaxSpeed();
                float minSpeed = _dataLogger.GetMinSpeed();

                DebugHelper.Log($"📊 [QuickTest] 平均速度: {avgSpeed:F3} m/s");
                DebugHelper.Log($"📊 [QuickTest] 最大速度: {maxSpeed:F3} m/s");
                DebugHelper.Log($"📊 [QuickTest] 最小速度: {minSpeed:F3} m/s");

                // 导出数据
                string jsonData = _dataLogger.ExportToJSON();
                DebugHelper.Log("📤 [QuickTest] JSON数据已生成（查看Console）");
                DebugHelper.Log(jsonData);
            }
        }

        /// <summary>
        /// 测试能量计算
        /// </summary>
        [ContextMenu("测试能量计算")]
        public void TestEnergyCalculator()
        {
            if (_energyCalculator == null)
            {
                DebugHelper.LogWarning("⚠️ [QuickTest] 请设置EnergyCalculator");
                return;
            }

            float ke = _energyCalculator.KineticEnergy;
            float pe = _energyCalculator.PotentialEnergy;
            float total = _energyCalculator.TotalEnergy;

            DebugHelper.Log($"⚡ [QuickTest] 动能: {ke:F2} J");
            DebugHelper.Log($"🏔️ [QuickTest] 势能: {pe:F2} J");
            DebugHelper.Log($"📊 [QuickTest] 总机械能: {total:F2} J");

            var stats = _energyCalculator.GetStatistics();
            DebugHelper.Log($"📈 [QuickTest] 平均动能: {stats.averageKineticEnergy:F2} J");
            DebugHelper.Log($"📉 [QuickTest] 平均势能: {stats.averagePotentialEnergy:F2} J");
            DebugHelper.Log($"🎯 [QuickTest] 能量偏差: {stats.energyDeviation:F4} J");

            DebugHelper.Log("✅ [QuickTest] 能量计算测试完成");
        }

        /// <summary>
        /// 重置测试
        /// </summary>
        [ContextMenu("重置测试")]
        public void ResetTest()
        {
            _testStarted = false;

            if (_testObject != null)
            {
                _testObject.ResetPhysics();
                _testObject.transform.position = new Vector3(0f, 0.5f, 0f);
            }

            if (_dataLogger != null)
            {
                _dataLogger.ClearData();
            }

            if (_energyCalculator != null)
            {
                _energyCalculator.ClearHistory();
            }

            DebugHelper.Log("🔄 [QuickTest] 测试已重置");
        }
    }
}
