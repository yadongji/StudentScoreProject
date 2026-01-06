using UnityEngine;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 皮带传送带 - 模拟传送带物理效果
    /// </summary>
    public class BeltConveyor : MonoBehaviour
    {
        [Header("传送带配置")]
        [SerializeField] private float _beltSpeed = 2f;
        [SerializeField] private float _beltWidth = 2f;
        [SerializeField] private float _beltLength = 10f;
        [SerializeField] private bool _isRunning = true;

        [Header("倾斜角度设置（度）")]
        [SerializeField] [Range(0f, 90f)] private float _inclineAngle = 0f;

        [Header("物理参数（用于计算验证）")]
        [SerializeField] [Tooltip("重力加速度（m/s²）")] private float _gravity = 9.81f;
        [SerializeField] [Tooltip("物体质量（kg），用于理论计算")] private float _objectMass = 1f;

        [Header("可视化")]
        [SerializeField] private Renderer _beltRenderer;
        [SerializeField] private float _textureScrollSpeed = 1f;
        [SerializeField] private Vector2 _scrollDirection = Vector2.right;

        [Header("物理交互")]
        [SerializeField] private PhysicMaterial _beltMaterial;
        [SerializeField] private float _contactForceMultiplier = 1.5f;

        private Rigidbody _rigidbody;
        private Material _beltMaterialInstance;
        private Vector2 _textureOffset;

        /// <summary>
        /// 传送带速度
        /// </summary>
        public float BeltSpeed
        {
            get => _beltSpeed;
            set
            {
                _beltSpeed = Mathf.Abs(value);
            }
        }

        /// <summary>
        /// 是否运行中
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        /// <summary>
        /// 倾斜角度（度）
        /// </summary>
        public float InclineAngle
        {
            get => _inclineAngle;
            set
            {
                _inclineAngle = Mathf.Clamp(value, 0f, 90f);
                UpdateIncline();
                DebugHelper.Log($"📐 [BeltConveyor] 倾斜角度设置为: {_inclineAngle:F1}°");
            }
        }

        /// <summary>
        /// 倾斜角度（弧度）
        /// </summary>
        public float InclineAngleRadians => _inclineAngle * Mathf.Deg2Rad;

        /// <summary>
        /// 重力沿传送带向下的分量（mgsinα）
        /// </summary>
        public float GravityDownComponent => _objectMass * _gravity * Mathf.Sin(InclineAngleRadians);

        /// <summary>
        /// 重力垂直于传送带的分量（mgcosα）
        /// </summary>
        public float GravityNormalComponent => _objectMass * _gravity * Mathf.Cos(InclineAngleRadians);

        /// <summary>
        /// 临界摩擦系数（μ = tanα）
        /// 当摩擦系数大于此值时，物体不会滑动
        /// </summary>
        public float CriticalFrictionCoefficient => Mathf.Tan(InclineAngleRadians);

        /// <summary>
        /// 计算摩擦力（μmgcosα）
        /// </summary>
        /// <param name="frictionCoefficient">摩擦系数 μ</param>
        public float CalculateFrictionForce(float frictionCoefficient)
        {
            return frictionCoefficient * GravityNormalComponent;
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            UpdateIncline();
            InitializeMaterial();
        }

        private void FixedUpdate()
        {
            if (_isRunning)
            {
                ApplyBeltForce();
            }
        }

        private void Update()
        {
            UpdateTextureAnimation();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();

            // 确保有碰撞器
            if (GetComponent<Collider>() == null)
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3(_beltWidth, 0.1f, _beltLength);
            }

            // 确保有渲染器
            if (_beltRenderer == null)
            {
                _beltRenderer = GetComponent<Renderer>();
            }

            DebugHelper.Log($"✅ [BeltConveyor] 初始化完成 | 速度: {_beltSpeed} m/s");
        }

        /// <summary>
        /// 初始化材质
        /// </summary>
        private void InitializeMaterial()
        {
            if (_beltRenderer != null && _beltRenderer.material != null)
            {
                _beltMaterialInstance = _beltRenderer.material;
            }
        }

        /// <summary>
        /// 更新倾斜角度
        /// </summary>
        private void UpdateIncline()
        {
            // 直接根据角度设置旋转，角度为0时水平，90时垂直
            transform.eulerAngles = new Vector3(-_inclineAngle, 0f, 0f);
        }

        /// <summary>
        /// 应用传送带力
        /// </summary>
        private void ApplyBeltForce()
        {
            // 检测传送带上的物体
            // 增加高度以检测更小的质点碰撞器
            Collider[] hitColliders = UnityEngine.Physics.OverlapBox(
                transform.position,
                new Vector3(_beltWidth / 2f, 1f, _beltLength / 2f),
                transform.rotation
            );

            foreach (var collider in hitColliders)
            {
                if (collider.attachedRigidbody != null && collider.attachedRigidbody != _rigidbody)
                {
                    // 检查是否为质点模式的物体
                    PhysicsObject physicsObject = collider.attachedRigidbody.GetComponent<PhysicsObject>();
                    if (physicsObject != null && physicsObject.IsParticle)
                    {
                        // 质点：力直接作用在质心
                        Vector3 beltDirection = transform.forward;
                        Vector3 force = beltDirection * _beltSpeed * collider.attachedRigidbody.mass * _contactForceMultiplier;
                        collider.attachedRigidbody.AddForce(force, ForceMode.Acceleration);
                    }
                    else
                    {
                        // 普通物体：正常施加力
                        Vector3 beltDirection = transform.forward;
                        Vector3 force = beltDirection * _beltSpeed * collider.attachedRigidbody.mass * _contactForceMultiplier;
                        collider.attachedRigidbody.AddForce(force, ForceMode.Acceleration);
                    }
                }
            }
        }

        /// <summary>
        /// 更新纹理动画
        /// </summary>
        private void UpdateTextureAnimation()
        {
            if (_beltMaterialInstance == null || !_isRunning)
                return;

            _textureOffset += _scrollDirection * _beltSpeed * _textureScrollSpeed * Time.deltaTime;
            _beltMaterialInstance.mainTextureOffset = _textureOffset;
        }

        /// <summary>
        /// 启动传送带
        /// </summary>
        public void StartBelt()
        {
            _isRunning = true;
            DebugHelper.Log("▶️ [BeltConveyor] 传送带已启动");
        }

        /// <summary>
        /// 停止传送带
        /// </summary>
        public void StopBelt()
        {
            _isRunning = false;
            DebugHelper.Log("⏹️ [BeltConveyor] 传送带已停止");
        }

        /// <summary>
        /// 设置传送带速度
        /// </summary>
        public void SetBeltSpeed(float speed)
        {
            _beltSpeed = speed;
            DebugHelper.Log($"⚡ [BeltConveyor] 传送带速度设置为: {_beltSpeed} m/s");
        }

        /// <summary>
        /// 设置物体质量（用于理论计算）
        /// </summary>
        public void SetObjectMass(float mass)
        {
            _objectMass = mass;
            DebugHelper.Log($"⚖️ [BeltConveyor] 物体质量设置为: {_objectMass} kg");
        }

        /// <summary>
        /// 设置重力加速度
        /// </summary>
        public void SetGravity(float gravity)
        {
            _gravity = gravity;
            DebugHelper.Log($"🌍 [BeltConveyor] 重力加速度设置为: {_gravity} m/s²");
        }

        /// <summary>
        /// 获取传送带方向
        /// </summary>
        public Vector3 GetBeltDirection()
        {
            return transform.forward;
        }

        /// <summary>
        /// 获取传送带上某点的速度
        /// </summary>
        public Vector3 GetVelocityAtPoint(Vector3 point)
        {
            if (!_isRunning)
                return Vector3.zero;

            return transform.forward * _beltSpeed;
        }

        /// <summary>
        /// 检查点是否在传送带上
        /// </summary>
        public bool IsPointOnBelt(Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            return Mathf.Abs(localPoint.x) < _beltWidth / 2f &&
                   Mathf.Abs(localPoint.z) < _beltLength / 2f &&
                   Mathf.Abs(localPoint.y) < 0.5f;
        }

        /// <summary>
        /// 设置物理材质
        /// </summary>
        public void SetPhysicsMaterial(PhysicMaterial material)
        {
            _beltMaterial = material;
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.material = _beltMaterial;
            }
            DebugHelper.Log($"📋 [BeltConveyor] 物理材质已更新");
        }

        /// <summary>
        /// 显示物理计算信息
        /// </summary>
        [ContextMenu("显示物理计算信息")]
        public void ShowPhysicsCalculations()
        {
            DebugHelper.Log("===== 传送带物理计算 =====");
            DebugHelper.Log($"倾斜角度: {_inclineAngle:F1}° ({InclineAngleRadians:F3} 弧度)");
            DebugHelper.Log($"物体质量: {_objectMass} kg");
            DebugHelper.Log($"重力加速度: {_gravity} m/s²");
            DebugHelper.Log($"--- 重力分量 ---");
            DebugHelper.Log($"沿传送带向下的重力 (mgsinα): {GravityDownComponent:F3} N");
            DebugHelper.Log($"垂直于传送带的重力 (mgcosα): {GravityNormalComponent:F3} N");
            DebugHelper.Log($"--- 临界摩擦系数 ---");
            DebugHelper.Log($"临界摩擦系数 (μ = tanα): {CriticalFrictionCoefficient:F3}");
            DebugHelper.Log($"提示: 当实际摩擦系数 μ > {CriticalFrictionCoefficient:F3} 时，物体不会滑动");
            DebugHelper.Log($"提示: 当实际摩擦系数 μ < {CriticalFrictionCoefficient:F3} 时，物体会下滑");
            DebugHelper.Log("=====================");
        }

        private void OnDrawGizmos()
        {
            // 绘制传送带范围
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_beltWidth, 0.1f, _beltLength));

            // 绘制传送带方向
            if (_isRunning)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Vector3.zero, Vector3.forward * 2f);
                Gizmos.DrawLine(Vector3.forward * 2f, Vector3.forward * 1.8f + Vector3.right * 0.2f);
                Gizmos.DrawLine(Vector3.forward * 2f, Vector3.forward * 1.8f + Vector3.left * 0.2f);
            }

            // 绘制重力分量（仅在 Scene 视图中显示）
#if UNITY_EDITOR
            if (_inclineAngle > 0f)
            {
                // 绘制沿传送带向下的重力分量（红色）
                Gizmos.color = Color.red;
                Vector3 downComponent = -Vector3.forward * GravityDownComponent * 0.1f;
                Gizmos.DrawLine(Vector3.zero, downComponent);

                // 绘制垂直于传送带的重力分量（蓝色）
                Gizmos.color = Color.blue;
                Vector3 normalComponent = -Vector3.up * GravityNormalComponent * 0.1f;
                Gizmos.DrawLine(Vector3.zero, normalComponent);

                // 在 Scene 视图中显示角度
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 2f,
                    $"α = {_inclineAngle:F1}°\n" +
                    $"mgsinα = {GravityDownComponent:F2}N\n" +
                    $"mgcosα = {GravityNormalComponent:F2}N\n" +
                    $"μ临界 = {CriticalFrictionCoefficient:F2}"
                );
            }
#endif
        }

        private void OnDestroy()
        {
            if (_beltMaterialInstance != null)
            {
                Destroy(_beltMaterialInstance);
            }
        }
    }
}
