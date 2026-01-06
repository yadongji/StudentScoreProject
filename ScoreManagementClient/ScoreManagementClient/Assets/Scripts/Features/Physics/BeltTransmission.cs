using UnityEngine;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 皮带传动系统 - 模拟皮带或绳索连接的多个物体的关联运动
    /// </summary>
    public class BeltTransmission : MonoBehaviour
    {
        [Header("传动配置")]
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private float _transmissionRatio = 1f; // 传动比
        [SerializeField] private bool _isInextensible = true; // 不可伸长

        [Header("主动轮（驱动轮）")]
        [SerializeField] private Rigidbody _driverRigidbody;
        [SerializeField] private float _driverRadius = 0.5f;

        [Header("从动轮（被驱动轮）")]
        [SerializeField] private Rigidbody[] _drivenRigidbodies;
        [SerializeField] private float[] _drivenRadii;

        [Header("控制参数")]
        [SerializeField] private float _driverAngularVelocity = 2f; // rad/s
        [SerializeField] private bool _useDriverAsMotor = true; // 使用主动轮作为电机

        [Header("可视化")]
        [SerializeField] private bool _showBeltVisual = true;
        [SerializeField] private Color _beltColor = Color.gray;

        [Header("实时数据")]
        [SerializeField] private float _driverLinearVelocity;
        [SerializeField] private float[] _drivenAngularVelocities;
        [SerializeField] private float[] _drivenLinearVelocities;

        private LineRenderer _beltLine;

        /// <summary>
        /// 传动比
        /// </summary>
        public float TransmissionRatio
        {
            get => _transmissionRatio;
            set => _transmissionRatio = Mathf.Clamp(value, 0.1f, 10f);
        }

        /// <summary>
        /// 主动轮线速度
        /// </summary>
        public float DriverLinearVelocity => _driverLinearVelocity;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            InitializeTransmission();
            CreateBeltVisual();
        }

        private void FixedUpdate()
        {
            if (_isEnabled)
            {
                UpdateTransmission();
            }
        }

        private void Update()
        {
            UpdateRealtimeData();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            if (_driverRigidbody == null)
            {
                _driverRigidbody = GetComponent<Rigidbody>();
            }

            // 自动查找从动轮
            if (_drivenRigidbodies == null || _drivenRigidbodies.Length == 0)
            {
                Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
                var drivenList = new System.Collections.Generic.List<Rigidbody>();
                foreach (var rb in allRigidbodies)
                {
                    if (rb != _driverRigidbody && rb.CompareTag("DrivenWheel"))
                    {
                        drivenList.Add(rb);
                    }
                }
                _drivenRigidbodies = drivenList.ToArray();
            }

            // 初始化从动轮半径数组
            if (_drivenRadii == null || _drivenRadii.Length != _drivenRigidbodies.Length)
            {
                _drivenRadii = new float[_drivenRigidbodies.Length];
                for (int i = 0; i < _drivenRadii.Length; i++)
                {
                    _drivenRadii[i] = 0.5f; // 默认半径
                }
            }

            // 初始化实时数据数组
            _drivenAngularVelocities = new float[_drivenRigidbodies.Length];
            _drivenLinearVelocities = new float[_drivenRigidbodies.Length];

            DebugHelper.Log($"✅ [BeltTransmission] 初始化完成 | 主动轮: {_driverRigidbody?.name} | 从动轮数量: {_drivenRigidbodies.Length}");
        }

        /// <summary>
        /// 初始化传动系统
        /// </summary>
        private void InitializeTransmission()
        {
            if (_driverRigidbody != null)
            {
                if (_useDriverAsMotor)
                {
                    _driverRigidbody.isKinematic = true;
                }
            }
        }

        /// <summary>
        /// 创建皮带可视化
        /// </summary>
        private void CreateBeltVisual()
        {
            if (!_showBeltVisual) return;

            _beltLine = gameObject.AddComponent<LineRenderer>();
            _beltLine.material = new Material(Shader.Find("Sprites/Default"));
            _beltLine.startWidth = 0.05f;
            _beltLine.endWidth = 0.05f;
            _beltLine.startColor = _beltColor;
            _beltLine.endColor = _beltColor;
            _beltLine.loop = true;
            _beltLine.useWorldSpace = true;

            UpdateBeltVisual();
        }

        /// <summary>
        /// 更新传动逻辑
        /// </summary>
        private void UpdateTransmission()
        {
            if (_driverRigidbody == null || _drivenRigidbodies == null || _drivenRigidbodies.Length == 0)
                return;

            // 计算主动轮线速度
            if (_useDriverAsMotor)
            {
                _driverLinearVelocity = _driverAngularVelocity * _driverRadius;
            }
            else
            {
                _driverLinearVelocity = _driverRigidbody.angularVelocity.magnitude * _driverRadius;
            }

            // 更新从动轮的角速度
            for (int i = 0; i < _drivenRigidbodies.Length; i++)
            {
                if (_drivenRigidbodies[i] != null)
                {
                    // 根据传动比和半径比计算从动轮角速度
                    // ω_driven = ω_driver * (r_driver / r_driven) * transmissionRatio
                    float radiusRatio = _driverRadius / _drivenRadii[i];
                    float driverAngular = _useDriverAsMotor ? _driverAngularVelocity : _driverRigidbody.angularVelocity.magnitude;

                    float drivenAngular = driverAngular * radiusRatio * _transmissionRatio;

                    // 设置从动轮角速度
                    _drivenRigidbodies[i].angularVelocity = Vector3.up * drivenAngular;

                    // 计算从动轮线速度
                    _drivenLinearVelocities[i] = drivenAngular * _drivenRadii[i];
                }
            }
        }

        /// <summary>
        /// 更新实时数据
        /// </summary>
        private void UpdateRealtimeData()
        {
            // 已经在FixedUpdate中更新
            // 这里可以添加额外的实时计算
        }

        /// <summary>
        /// 更新皮带可视化
        /// </summary>
        private void UpdateBeltVisual()
        {
            if (!_showBeltVisual || _beltLine == null || _driverRigidbody == null)
                return;

            // 创建皮带路径点
            var points = new System.Collections.Generic.List<Vector3>();

            // 添加主动轮上的点
            for (int i = 0; i < 20; i++)
            {
                float angle = (float)i / 20f * Mathf.PI * 2;
                Vector3 point = _driverRigidbody.transform.position + new Vector3(
                    Mathf.Cos(angle) * _driverRadius,
                    0,
                    Mathf.Sin(angle) * _driverRadius
                );
                points.Add(point);
            }

            // 添加每个从动轮上的点
            foreach (var drivenRb in _drivenRigidbodies)
            {
                if (drivenRb == null) continue;

                for (int i = 0; i < 10; i++)
                {
                    float angle = (float)i / 10f * Mathf.PI * 2;
                    Vector3 point = drivenRb.transform.position + new Vector3(
                        Mathf.Cos(angle) * 0.5f,
                        0,
                        Mathf.Sin(angle) * 0.5f
                    );
                    points.Add(point);
                }
            }

            _beltLine.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                _beltLine.SetPosition(i, points[i]);
            }
        }

        /// <summary>
        /// 设置主动轮角速度
        /// </summary>
        public void SetDriverAngularVelocity(float angularVelocity)
        {
            _driverAngularVelocity = angularVelocity;
            DebugHelper.Log($"⚡ [BeltTransmission] 主动轮角速度设置为: {_driverAngularVelocity} rad/s");
        }

        /// <summary>
        /// 设置传动比
        /// </summary>
        public void SetTransmissionRatio(float ratio)
        {
            _transmissionRatio = ratio;
            DebugHelper.Log($"⚙️ [BeltTransmission] 传动比设置为: {_transmissionRatio}");
        }

        /// <summary>
        /// 启用/禁用传动
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;

            if (!enabled && _drivenRigidbodies != null)
            {
                // 禁用时停止所有从动轮
                foreach (var rb in _drivenRigidbodies)
                {
                    if (rb != null)
                    {
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }

            DebugHelper.Log($"🔌 [BeltTransmission] 传动系统{(enabled ? "已启用" : "已禁用")}");
        }

        /// <summary>
        /// 添加从动轮
        /// </summary>
        public void AddDrivenRigidbody(Rigidbody rigidbody, float radius)
        {
            var newRigidbodies = new Rigidbody[_drivenRigidbodies.Length + 1];
            var newRadii = new float[_drivenRadii.Length + 1];

            _drivenRigidbodies.CopyTo(newRigidbodies, 0);
            _drivenRadii.CopyTo(newRadii, 0);

            newRigidbodies[_drivenRigidbodies.Length] = rigidbody;
            newRadii[_drivenRadii.Length] = radius;

            _drivenRigidbodies = newRigidbodies;
            _drivenRadii = newRadii;

            _drivenAngularVelocities = new float[_drivenRigidbodies.Length];
            _drivenLinearVelocities = new float[_drivenRigidbodies.Length];

            DebugHelper.Log($"➕ [BeltTransmission] 添加从动轮: {rigidbody.name} | 半径: {radius}");
        }

        /// <summary>
        /// 移除从动轮
        /// </summary>
        public void RemoveDrivenRigidbody(Rigidbody rigidbody)
        {
            var newRigidbodies = new System.Collections.Generic.List<Rigidbody>(_drivenRigidbodies);
            var newRadii = new System.Collections.Generic.List<float>(_drivenRadii);

            int index = newRigidbodies.IndexOf(rigidbody);
            if (index >= 0)
            {
                newRigidbodies.RemoveAt(index);
                newRadii.RemoveAt(index);

                _drivenRigidbodies = newRigidbodies.ToArray();
                _drivenRadii = newRadii.ToArray();

                _drivenAngularVelocities = new float[_drivenRigidbodies.Length];
                _drivenLinearVelocities = new float[_drivenRigidbodies.Length];

                DebugHelper.Log($"➖ [BeltTransmission] 移除从动轮: {rigidbody.name}");
            }
        }

        /// <summary>
        /// 获取传动系统统计信息
        /// </summary>
        public TransmissionStatistics GetStatistics()
        {
            return new TransmissionStatistics
            {
                driverRadius = _driverRadius,
                driverAngularVelocity = _driverAngularVelocity,
                driverLinearVelocity = _driverLinearVelocity,
                transmissionRatio = _transmissionRatio,
                drivenWheelCount = _drivenRigidbodies.Length,
                averageDrivenAngularVelocity = CalculateAverageDrivenAngularVelocity()
            };
        }

        /// <summary>
        /// 计算从动轮平均角速度
        /// </summary>
        private float CalculateAverageDrivenAngularVelocity()
        {
            if (_drivenAngularVelocities == null || _drivenAngularVelocities.Length == 0)
                return 0f;

            float sum = 0f;
            foreach (float angularVel in _drivenAngularVelocities)
            {
                sum += angularVel;
            }
            return sum / _drivenAngularVelocities.Length;
        }

        private void OnDrawGizmos()
        {
            // 绘制主动轮
            if (_driverRigidbody != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_driverRigidbody.transform.position, _driverRadius);
            }

            // 绘制从动轮
            if (_drivenRigidbodies != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < _drivenRigidbodies.Length; i++)
                {
                    if (_drivenRigidbodies[i] != null)
                    {
                        Gizmos.DrawWireSphere(_drivenRigidbodies[i].transform.position, _drivenRadii[i]);

                        // 绘制连接线
                        if (_driverRigidbody != null)
                        {
                            Gizmos.DrawLine(_driverRigidbody.transform.position, _drivenRigidbodies[i].transform.position);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_beltLine != null)
            {
                Destroy(_beltLine);
            }
        }
    }

    /// <summary>
    /// 传动系统统计信息
    /// </summary>
    public class TransmissionStatistics
    {
        public float driverRadius;
        public float driverAngularVelocity;
        public float driverLinearVelocity;
        public float transmissionRatio;
        public int drivenWheelCount;
        public float averageDrivenAngularVelocity;
    }
}
