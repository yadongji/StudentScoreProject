using UnityEngine;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 弹簧系统 - 管理弹簧关节的物理行为
    /// </summary>
    public class SpringSystem : MonoBehaviour
    {
        [Header("弹簧配置")]
        [SerializeField] private SpringJoint _springJoint;
        [SerializeField] private float _springForce = 10f;
        [SerializeField] private float _damper = 0.5f;
        [SerializeField] private float _minDistance = 0.1f;
        [SerializeField] private float _maxDistance = 10f;

        [Header("可视化")]
        [SerializeField] private bool _showSpringVisual = true;
        [SerializeField] private Color _springColor = Color.yellow;
        [SerializeField] private float _lineWidth = 0.05f;

        [Header("实时数据")]
        [SerializeField] private float _currentLength;
        [SerializeField] private float _currentDisplacement;
        [SerializeField] private float _elasticPotentialEnergy;

        private LineRenderer _springLine;
        private Rigidbody _connectedRigidbody;
        private Rigidbody _rigidbody;

        /// <summary>
        /// 弹簧力
        /// </summary>
        public float SpringForce
        {
            get => _springForce;
            set
            {
                _springForce = Mathf.Max(0.1f, value);
                if (_springJoint != null)
                {
                    _springJoint.spring = _springForce;
                }
            }
        }

        /// <summary>
        /// 阻尼系数
        /// </summary>
        public float Damper
        {
            get => _damper;
            set
            {
                _damper = Mathf.Clamp01(value);
                if (_springJoint != null)
                {
                    _springJoint.damper = _damper;
                }
            }
        }

        /// <summary>
        /// 当前弹簧长度
        /// </summary>
        public float CurrentLength => _currentLength;

        /// <summary>
        /// 当前弹性势能
        /// </summary>
        public float ElasticPotentialEnergy => _elasticPotentialEnergy;

        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            InitializeSpringJoint();
            CreateSpringVisual();
        }

        private void Update()
        {
            UpdateSpringData();
            UpdateSpringVisual();
        }

        private void FixedUpdate()
        {
            ConstrainSpringDistance();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (_springJoint == null)
            {
                _springJoint = GetComponent<SpringJoint>();
            }

            if (_springJoint == null)
            {
                DebugHelper.LogWarning("⚠️ [SpringSystem] 未找到SpringJoint组件");
            }
        }

        /// <summary>
        /// 初始化弹簧关节
        /// </summary>
        private void InitializeSpringJoint()
        {
            if (_springJoint != null)
            {
                _springJoint.spring = _springForce;
                _springJoint.damper = _damper;
                _springJoint.minDistance = _minDistance;
                _springJoint.maxDistance = _maxDistance;

                _connectedRigidbody = _springJoint.connectedBody;

                DebugHelper.Log($"✅ [SpringSystem] 弹簧关节初始化完成 | 弹簧力: {_springForce}");
            }
        }

        /// <summary>
        /// 创建弹簧可视化
        /// </summary>
        private void CreateSpringVisual()
        {
            if (!_showSpringVisual) return;

            _springLine = gameObject.AddComponent<LineRenderer>();
            _springLine.material = new Material(Shader.Find("Sprites/Default"));
            _springLine.startWidth = _lineWidth;
            _springLine.endWidth = _lineWidth;
            _springLine.startColor = _springColor;
            _springLine.endColor = _springColor;
            _springLine.useWorldSpace = true;
            _springLine.positionCount = 2;
        }

        /// <summary>
        /// 更新弹簧数据
        /// </summary>
        private void UpdateSpringData()
        {
            if (_springJoint == null || _rigidbody == null)
                return;

            // 计算弹簧长度
            Vector3 anchorPosition = _springJoint.connectedBody != null
                ? _springJoint.connectedBody.transform.TransformPoint(_springJoint.connectedAnchor)
                : transform.TransformPoint(_springJoint.connectedAnchor);

            _currentLength = Vector3.Distance(transform.position, anchorPosition);
            _currentDisplacement = _currentLength - _springJoint.minDistance;

            // 计算弹性势能: E = (1/2) * k * x²
            _elasticPotentialEnergy = 0.5f * _springForce * _currentDisplacement * _currentDisplacement;
        }

        /// <summary>
        /// 更新弹簧可视化
        /// </summary>
        private void UpdateSpringVisual()
        {
            if (!_showSpringVisual || _springLine == null || _springJoint == null)
                return;

            Vector3 startPosition = transform.TransformPoint(_springJoint.anchor);
            Vector3 endPosition = _springJoint.connectedBody != null
                ? _springJoint.connectedBody.transform.TransformPoint(_springJoint.connectedAnchor)
                : transform.TransformPoint(_springJoint.connectedAnchor);

            _springLine.SetPosition(0, startPosition);
            _springLine.SetPosition(1, endPosition);
        }

        /// <summary>
        /// 约束弹簧距离
        /// </summary>
        private void ConstrainSpringDistance()
        {
            if (_springJoint == null || _rigidbody == null)
                return;

            // 弹簧关节会自动处理距离约束
            // 这里可以添加额外的约束逻辑
        }

        /// <summary>
        /// 设置弹簧力
        /// </summary>
        public void SetSpringForce(float force)
        {
            SpringForce = force;
            DebugHelper.Log($"🔧 [SpringSystem] 弹簧力设置为: {_springForce}");
        }

        /// <summary>
        /// 设置阻尼系数
        /// </summary>
        public void SetDamper(float damper)
        {
            Damper = damper;
            DebugHelper.Log($"🔧 [SpringSystem] 阻尼系数设置为: {_damper}");
        }

        /// <summary>
        /// 设置距离范围
        /// </summary>
        public void SetDistanceRange(float min, float max)
        {
            _minDistance = min;
            _maxDistance = max;

            if (_springJoint != null)
            {
                _springJoint.minDistance = _minDistance;
                _springJoint.maxDistance = _maxDistance;
            }

            DebugHelper.Log($"🔧 [SpringSystem] 距离范围设置为: {_minDistance}-{_maxDistance}");
        }

        /// <summary>
        /// 连接到刚体
        /// </summary>
        public void ConnectTo(Rigidbody rigidbody, Vector3 anchor)
        {
            if (_springJoint != null)
            {
                _springJoint.connectedBody = rigidbody;
                _springJoint.connectedAnchor = anchor;
                _connectedRigidbody = rigidbody;
                DebugHelper.Log($"🔗 [SpringSystem] 连接到刚体: {rigidbody.name}");
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (_springJoint != null)
            {
                _springJoint.connectedBody = null;
                _connectedRigidbody = null;
                DebugHelper.Log("🔓 [SpringSystem] 已断开连接");
            }
        }

        /// <summary>
        /// 是否已拉伸
        /// </summary>
        public bool IsStretched()
        {
            return _currentLength > _springJoint.minDistance;
        }

        /// <summary>
        /// 是否已压缩
        /// </summary>
        public bool IsCompressed()
        {
            return _currentLength < _springJoint.minDistance;
        }

        private void OnDrawGizmos()
        {
            if (_springJoint == null) return;

            // 绘制弹簧范围
            Gizmos.color = Color.green;
            Vector3 startPosition = transform.TransformPoint(_springJoint.anchor);
            Vector3 endPosition = _springJoint.connectedBody != null
                ? _springJoint.connectedBody.transform.TransformPoint(_springJoint.connectedAnchor)
                : startPosition + Vector3.down;

            Gizmos.DrawLine(startPosition, endPosition);

            // 绘制最小距离
            if (_minDistance > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(startPosition, _minDistance);
            }

            // 绘制最大距离
            if (_maxDistance > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(startPosition, _maxDistance);
            }
        }

        private void OnDestroy()
        {
            if (_springLine != null)
            {
                Destroy(_springLine);
            }
        }
    }
}
