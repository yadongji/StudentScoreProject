using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 简易图表绘制器 - 使用LineRenderer绘制数据曲线
    /// </summary>
    public class SimpleChartDrawer : MonoBehaviour
    {
        [Header("图表配置")]
        [SerializeField] private Color _lineColor = Color.cyan;
        [SerializeField] private Color _gridColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private int _maxPoints = 100;
        [SerializeField] private float _chartWidth = 10f;
        [SerializeField] private float _chartHeight = 5f;
        [SerializeField] private float _minValue = 0f;
        [SerializeField] private float _maxValue = 10f;

        [Header("显示设置")]
        [SerializeField] private bool _showGrid = true;
        [SerializeField] private bool _showLabels = true;
        [SerializeField] private bool _autoScale = true;

        private LineRenderer _lineRenderer;
        private List<float> _dataPoints = new List<float>();
        private float _currentTime = 0f;

        /// <summary>
        /// 数据点列表
        /// </summary>
        public List<float> DataPoints => _dataPoints;

        /// <summary>
        /// 线条颜色
        /// </summary>
        public Color LineColor
        {
            get => _lineColor;
            set => _lineColor = value;
        }

        private void Awake()
        {
            InitializeLineRenderer();
        }

        private void InitializeLineRenderer()
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.startColor = _lineColor;
            _lineRenderer.endColor = _lineColor;
            _lineRenderer.useWorldSpace = true;
        }

        /// <summary>
        /// 添加数据点
        /// </summary>
        public void AddDataPoint(float value)
        {
            _dataPoints.Add(value);

            if (_dataPoints.Count > _maxPoints)
            {
                _dataPoints.RemoveAt(0);
            }

            // 自动调整范围
            if (_autoScale)
            {
                UpdateValueRange();
            }

            // 更新图表
            UpdateChart();
        }

        /// <summary>
        /// 批量添加数据点
        /// </summary>
        public void AddDataPoints(List<float> values)
        {
            foreach (float value in values)
            {
                _dataPoints.Add(value);
            }

            while (_dataPoints.Count > _maxPoints)
            {
                _dataPoints.RemoveAt(0);
            }

            if (_autoScale)
            {
                UpdateValueRange();
            }

            UpdateChart();
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void ClearData()
        {
            _dataPoints.Clear();
            _lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// 设置数据范围
        /// </summary>
        public void SetValueRange(float minValue, float maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _autoScale = false;
            UpdateChart();
        }

        /// <summary>
        /// 更新数值范围
        /// </summary>
        private void UpdateValueRange()
        {
            if (_dataPoints.Count == 0)
                return;

            _minValue = float.MaxValue;
            _maxValue = float.MinValue;

            foreach (float value in _dataPoints)
            {
                if (value < _minValue) _minValue = value;
                if (value > _maxValue) _maxValue = value;
            }

            // 添加一些边距
            float range = _maxValue - _minValue;
            if (range == 0) range = 1f;
            _minValue -= range * 0.1f;
            _maxValue += range * 0.1f;
        }

        /// <summary>
        /// 更新图表
        /// </summary>
        private void UpdateChart()
        {
            if (_dataPoints.Count == 0)
            {
                _lineRenderer.positionCount = 0;
                return;
            }

            _lineRenderer.positionCount = _dataPoints.Count;

            for (int i = 0; i < _dataPoints.Count; i++)
            {
                float x = (float)i / (_maxPoints - 1) * _chartWidth;
                float normalizedValue = Mathf.InverseLerp(_minValue, _maxValue, _dataPoints[i]);
                float y = normalizedValue * _chartHeight;

                Vector3 point = transform.position + new Vector3(x, y, 0);
                _lineRenderer.SetPosition(i, point);
            }

            _lineRenderer.material.color = _lineColor;
        }

        /// <summary>
        /// 绘制网格（使用Gizmos）
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_showGrid) return;

            Gizmos.color = _gridColor;

            // 绘制水平网格线
            int gridLines = 5;
            for (int i = 0; i <= gridLines; i++)
            {
                float normalizedY = (float)i / gridLines;
                Vector3 lineStart = transform.position + new Vector3(0, normalizedY * _chartHeight, 0);
                Vector3 lineEnd = transform.position + new Vector3(_chartWidth, normalizedY * _chartHeight, 0);
                Gizmos.DrawLine(lineStart, lineEnd);
            }

            // 绘制垂直网格线
            for (int i = 0; i <= gridLines; i++)
            {
                float normalizedX = (float)i / gridLines;
                Vector3 lineStart = transform.position + new Vector3(normalizedX * _chartWidth, 0, 0);
                Vector3 lineEnd = transform.position + new Vector3(normalizedX * _chartWidth, _chartHeight, 0);
                Gizmos.DrawLine(lineStart, lineEnd);
            }
        }

        /// <summary>
        /// 获取图表统计信息
        /// </summary>
        public ChartStatistics GetStatistics()
        {
            if (_dataPoints.Count == 0)
                return new ChartStatistics();

            float sum = 0f;
            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (float value in _dataPoints)
            {
                sum += value;
                if (value < min) min = value;
                if (value > max) max = value;
            }

            return new ChartStatistics
            {
                count = _dataPoints.Count,
                average = sum / _dataPoints.Count,
                min = min,
                max = max,
                range = max - min
            };
        }

        /// <summary>
        /// 获取当前值
        /// </summary>
        public float GetCurrentValue()
        {
            return _dataPoints.Count > 0 ? _dataPoints[_dataPoints.Count - 1] : 0f;
        }
    }

    /// <summary>
    /// 图表统计数据
    /// </summary>
    public class ChartStatistics
    {
        public int count;
        public float average;
        public float min;
        public float max;
        public float range;
    }
}
