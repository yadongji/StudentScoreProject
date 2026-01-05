using UnityEngine;
using Core.Base;
using Models;

namespace Features.Physics
{
    /// <summary>
    /// 物理对象基类 - 所有物理对象的基类
    /// </summary>
    public abstract class PhysicsObject : MonoBehaviour
    {
        [Header("物理属性")]
        [SerializeField] protected float _mass = 1f;
        [SerializeField] protected bool _isKinematic = false;
        [SerializeField] protected bool _useGravity = true;

        [Header("显示属性")]
        [SerializeField] protected string _objectName = "PhysicsObject";
        [SerializeField] protected Color _objectColor = Color.white;

        protected Rigidbody _rigidbody;
        protected Collider _collider;
        protected MeshRenderer _renderer;

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

        protected virtual void Awake()
        {
            InitializeComponents();
            InitializePhysics();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        protected virtual void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<MeshRenderer>();

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
    }
}
