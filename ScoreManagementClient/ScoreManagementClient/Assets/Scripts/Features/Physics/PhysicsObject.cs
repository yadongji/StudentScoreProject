using UnityEngine;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 物理对象基类 - 所有物理对象的基类
    /// </summary>
    public class PhysicsObject : MonoBehaviour
    {
        [Header("物理属性")]
        [SerializeField] protected float _mass = 1f;
        [SerializeField] protected bool _isKinematic = false;
        [SerializeField] protected bool _useGravity = true;

        [Header("质点模式")]
        [SerializeField] [Tooltip("启用质点模式：物体将表现为没有大小和形状的理想质点")] private bool _isParticle = false;
        [SerializeField] [Tooltip("质点模式下的碰撞器大小（米）")] private float _particleColliderSize = 0.01f;
        [SerializeField] [Tooltip("质点可视化大小（米）")] private float _particleVisualSize = 0.1f;

        [Header("显示属性")]
        [SerializeField] protected string _objectName = "PhysicsObject";
        [SerializeField] protected Color _objectColor = Color.white;

        protected Rigidbody _rigidbody;
        protected Collider _collider;
        protected MeshRenderer _renderer;
        protected MeshFilter _meshFilter;

        // 质点模式下的组件
        private GameObject _particleVisual;
        private bool _originalRendererEnabled;
        private Vector3 _originalColliderSize;
        private Vector3 _originalColliderCenter;

        /// <summary>
        /// 物体质量
        /// </summary>
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                if (_rigidbody != null)
                {
                    _rigidbody.mass = _mass;
                }
            }
        }

        /// <summary>
        /// 是否为运动学物体
        /// </summary>
        public bool IsKinematic
        {
            get => _isKinematic;
            set
            {
                _isKinematic = value;
                if (_rigidbody != null)
                {
                    _rigidbody.isKinematic = _isKinematic;
                }
            }
        }

        /// <summary>
        /// 是否使用重力
        /// </summary>
        public bool UseGravity
        {
            get => _useGravity;
            set
            {
                _useGravity = value;
                if (_rigidbody != null)
                {
                    _rigidbody.useGravity = _useGravity;
                }
            }
        }

        /// <summary>
        /// 是否为质点模式
        /// </summary>
        public bool IsParticle
        {
            get => _isParticle;
            set
            {
                if (_isParticle != value)
                {
                    _isParticle = value;
                    ApplyParticleMode();
                }
            }
        }

        protected virtual void Awake()
        {
            InitializeComponents();
            InitializePhysics();
        }

        private void Start()
        {
            // 应用质点模式（如果已启用）
            if (_isParticle)
            {
                ApplyParticleMode();
            }
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        protected virtual void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();

            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            if (_collider == null)
            {
                _collider = gameObject.AddComponent<BoxCollider>();
            }

            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<MeshRenderer>();
            }
        }

        /// <summary>
        /// 初始化物理属性
        /// </summary>
        protected virtual void InitializePhysics()
        {
            if (_rigidbody != null)
            {
                _rigidbody.mass = _mass;
                _rigidbody.isKinematic = _isKinematic;
                _rigidbody.useGravity = _useGravity;
            }

            if (_renderer != null)
            {
                _renderer.material.color = _objectColor;
            }

            // 如果启用了质点模式，在Start后应用
            // 因为需要等所有组件初始化完成
        }

        /// <summary>
        /// 应用质点模式
        /// </summary>
        private void ApplyParticleMode()
        {
            if (_collider == null || _rigidbody == null)
                return;

            if (_isParticle)
            {
                // ===== 启用质点模式 =====

                // 1. 锁定旋转（质点没有转动惯量）
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                _rigidbody.angularDrag = 0f;

                // 2. 设置极小的碰撞器
                if (_collider is BoxCollider boxCollider)
                {
                    _originalColliderSize = boxCollider.size;
                    _originalColliderCenter = boxCollider.center;
                    boxCollider.size = Vector3.one * _particleColliderSize;
                    boxCollider.center = Vector3.zero;
                }
                else if (_collider is SphereCollider sphereCollider)
                {
                    _originalColliderSize = Vector3.one * sphereCollider.radius * 2;
                    _originalColliderCenter = Vector3.zero;
                    sphereCollider.radius = _particleColliderSize * 0.5f;
                    sphereCollider.center = Vector3.zero;
                }

                // 3. 创建质点可视化（小球）
                CreateParticleVisual();

                // 4. 禁用原始渲染器
                if (_renderer != null)
                {
                    _originalRendererEnabled = _renderer.enabled;
                    _renderer.enabled = false;
                }

                DebugHelper.Log($"🔵 [PhysicsObject] 质点模式已启用 | 碰撞器尺寸: {_particleColliderSize}m | 可视化: {_particleVisualSize}m");
            }
            else
            {
                // ===== 恢复普通模式 =====

                // 1. 恢复旋转约束
                _rigidbody.constraints = RigidbodyConstraints.None;

                // 2. 恢复碰撞器尺寸
                if (_collider is BoxCollider boxCollider)
                {
                    boxCollider.size = _originalColliderSize;
                    boxCollider.center = _originalColliderCenter;
                }
                else if (_collider is SphereCollider sphereCollider)
                {
                    sphereCollider.radius = _originalColliderSize.x * 0.5f;
                    sphereCollider.center = _originalColliderCenter;
                }

                // 3. 销毁质点可视化
                DestroyParticleVisual();

                // 4. 恢复原始渲染器
                if (_renderer != null)
                {
                    _renderer.enabled = _originalRendererEnabled;
                }

                DebugHelper.Log("📦 [PhysicsObject] 普通模式已恢复");
            }
        }

        /// <summary>
        /// 创建质点可视化
        /// </summary>
        private void CreateParticleVisual()
        {
            if (_particleVisual != null)
                return;

            // 创建一个小球作为质点的可视化
            _particleVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _particleVisual.name = "ParticleVisual";
            _particleVisual.transform.SetParent(transform);
            _particleVisual.transform.localPosition = Vector3.zero;
            _particleVisual.transform.localScale = Vector3.one * _particleVisualSize;

            // 设置材质
            Renderer particleRenderer = _particleVisual.GetComponent<Renderer>();
            if (particleRenderer != null)
            {
                particleRenderer.material.color = _objectColor;
                particleRenderer.material.shader = Shader.Find("Unlit/Color");
            }

            // 移除碰撞器（我们使用原始物体的碰撞器）
            Collider particleCollider = _particleVisual.GetComponent<Collider>();
            if (particleCollider != null)
            {
                Destroy(particleCollider);
            }
        }

        /// <summary>
        /// 销毁质点可视化
        /// </summary>
        private void DestroyParticleVisual()
        {
            if (_particleVisual != null)
            {
                DestroyImmediate(_particleVisual);
                _particleVisual = null;
            }
        }

        /// <summary>
        /// 施加力
        /// </summary>
        public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.AddForce(force, mode);
            }
        }

        /// <summary>
        /// 在指定点施加力
        /// </summary>
        public void ApplyForceAtPosition(Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force)
        {
            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.AddForceAtPosition(force, position, mode);
            }
        }

        /// <summary>
        /// 施加冲量
        /// </summary>
        public void ApplyImpulse(Vector3 impulse)
        {
            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// 施加扭矩
        /// </summary>
        public void ApplyTorque(Vector3 torque)
        {
            // 质点模式下忽略扭矩
            if (_isParticle)
                return;

            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.AddTorque(torque, ForceMode.Force);
            }
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        public void SetVelocity(Vector3 velocity)
        {
            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.velocity = velocity;
            }
        }

        /// <summary>
        /// 获取速度
        /// </summary>
        public Vector3 GetVelocity()
        {
            return _rigidbody != null ? _rigidbody.velocity : Vector3.zero;
        }

        /// <summary>
        /// 设置角速度
        /// </summary>
        public void SetAngularVelocity(Vector3 angularVelocity)
        {
            // 质点模式下忽略角速度
            if (_isParticle)
                return;

            if (_rigidbody != null && !_isKinematic)
            {
                _rigidbody.angularVelocity = angularVelocity;
            }
        }

        /// <summary>
        /// 获取角速度
        /// </summary>
        public Vector3 GetAngularVelocity()
        {
            return _rigidbody != null ? _rigidbody.angularVelocity : Vector3.zero;
        }

        /// <summary>
        /// 获取动能
        /// </summary>
        public float GetKineticEnergy()
        {
            if (_rigidbody == null || _isKinematic)
                return 0f;

            float velocity = _rigidbody.velocity.magnitude;
            return 0.5f * _mass * velocity * velocity;
        }

        /// <summary>
        /// 从数据初始化
        /// </summary>
        public virtual void InitializeFromData(PhysicsObjectData data)
        {
            if (data == null)
                return;

            _objectName = data.objectName;
            transform.position = data.position;
            transform.rotation = data.rotation;
            transform.localScale = data.scale;
            _mass = data.mass;

            InitializePhysics();
        }

        /// <summary>
        /// 转换为数据
        /// </summary>
        public virtual PhysicsObjectData ToData()
        {
            return new PhysicsObjectData
            {
                objectId = gameObject.GetInstanceID().ToString(),
                objectName = _objectName,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                mass = _mass
            };
        }

        /// <summary>
        /// 重置物理状态
        /// </summary>
        public virtual void ResetPhysics()
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// 切换质点模式
        /// </summary>
        [ContextMenu("切换质点模式")]
        public void ToggleParticleMode()
        {
            IsParticle = !_isParticle;
        }

        /// <summary>
        /// 启用质点模式
        /// </summary>
        [ContextMenu("启用质点模式")]
        public void EnableParticleMode()
        {
            IsParticle = true;
        }

        /// <summary>
        /// 禁用质点模式
        /// </summary>
        [ContextMenu("禁用质点模式")]
        public void DisableParticleMode()
        {
            IsParticle = false;
        }

        /// <summary>
        /// 绘制质点可视化（Gizmos）
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_isParticle)
            {
                // 质点模式：绘制一个更大的点来标识质心
                Gizmos.color = _objectColor;
                Gizmos.DrawWireSphere(transform.position, _particleVisualSize * 0.5f);

                // 绘制质心十字标记
                Gizmos.color = Color.white;
                float crossSize = _particleVisualSize * 0.3f;
                Gizmos.DrawLine(
                    transform.position + Vector3.up * crossSize,
                    transform.position - Vector3.up * crossSize
                );
                Gizmos.DrawLine(
                    transform.position + Vector3.right * crossSize,
                    transform.position - Vector3.right * crossSize
                );
                Gizmos.DrawLine(
                    transform.position + Vector3.forward * crossSize,
                    transform.position - Vector3.forward * crossSize
                );
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void OnDestroy()
        {
            DestroyParticleVisual();
        }
    }
}
