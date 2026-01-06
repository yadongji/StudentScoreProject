using UnityEngine;
using Utils;

namespace Features.Physics
{
    /// <summary>
    /// 物理材质控制器 - 动态调节物理材质属性
    /// </summary>
    public class PhysicsMaterialController : MonoBehaviour
    {
        [Header("物理材质配置")]
        [SerializeField] private PhysicMaterial _targetMaterial;

        [Header("初始值")]
        [SerializeField] private float _initialDynamicFriction = 0.6f;
        [SerializeField] private float _initialStaticFriction = 0.6f;
        [SerializeField] private float _initialBounciness = 0f;

        [Header("摩擦系数范围")]
        [SerializeField] private float _minFriction = 0f;
        [SerializeField] private float _maxFriction = 1f;

        [Header("实时数值")]
        [SerializeField] private float _currentDynamicFriction;
        [SerializeField] private float _currentStaticFriction;
        [SerializeField] private float _currentBounciness;

        /// <summary>
        /// 当前进动摩擦系数
        /// </summary>
        public float DynamicFriction => _currentDynamicFriction;

        /// <summary>
        /// 当前静摩擦系数
        /// </summary>
        public float StaticFriction => _currentStaticFriction;

        /// <summary>
        /// 当前弹性系数
        /// </summary>
        public float Bounciness => _currentBounciness;

        private void Awake()
        {
            InitializeMaterial();
        }

        /// <summary>
        /// 初始化物理材质
        /// </summary>
        private void InitializeMaterial()
        {
            if (_targetMaterial == null)
            {
                // 尝试从碰撞器获取物理材质
                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    if (collider.material != null)
                    {
                        _targetMaterial = collider.material;
                        break;
                    }
                }

                if (_targetMaterial == null)
                {
                    _targetMaterial = new PhysicMaterial(gameObject.name + "_Material");
                    DebugHelper.LogWarning($"⚠️ [PhysicsMaterialController] 未找到物理材质，已创建新材质: {_targetMaterial.name}");
                }
            }

            // 设置初始值
            SetDynamicFriction(_initialDynamicFriction);
            SetStaticFriction(_initialStaticFriction);
            SetBounciness(_initialBounciness);

            DebugHelper.Log($"✅ [PhysicsMaterialController] 初始化完成 | 物理材质: {_targetMaterial.name}");
        }

        /// <summary>
        /// 设置动摩擦系数
        /// </summary>
        public void SetDynamicFriction(float value)
        {
            _currentDynamicFriction = Mathf.Clamp(value, _minFriction, _maxFriction);

            if (_targetMaterial != null)
            {
                _targetMaterial.dynamicFriction = _currentDynamicFriction;
            }
        }

        /// <summary>
        /// 设置静摩擦系数
        /// </summary>
        public void SetStaticFriction(float value)
        {
            _currentStaticFriction = Mathf.Clamp(value, _minFriction, _maxFriction);

            if (_targetMaterial != null)
            {
                _targetMaterial.staticFriction = _currentStaticFriction;
            }
        }

        /// <summary>
        /// 设置弹性系数
        /// </summary>
        public void SetBounciness(float value)
        {
            _currentBounciness = Mathf.Clamp01(value);

            if (_targetMaterial != null)
            {
                _targetMaterial.bounciness = _currentBounciness;
            }
        }

        /// <summary>
        /// 同时设置动摩擦和静摩擦系数
        /// </summary>
        public void SetFriction(float friction)
        {
            SetDynamicFriction(friction);
            SetStaticFriction(friction);
        }

        /// <summary>
        /// 设置摩擦系数范围
        /// </summary>
        public void SetFrictionRange(float min, float max)
        {
            _minFriction = Mathf.Clamp01(min);
            _maxFriction = Mathf.Clamp01(max);
        }

        /// <summary>
        /// 重置为初始值
        /// </summary>
        public void ResetToInitial()
        {
            SetDynamicFriction(_initialDynamicFriction);
            SetStaticFriction(_initialStaticFriction);
            SetBounciness(_initialBounciness);

            DebugHelper.Log($"🔄 [PhysicsMaterialController] 重置为初始值");
        }

        /// <summary>
        /// 获取物理材质
        /// </summary>
        public PhysicMaterial GetMaterial()
        {
            return _targetMaterial;
        }

        /// <summary>
        /// 应用材质到碰撞器
        /// </summary>
        public void ApplyToCollider(Collider collider)
        {
            if (collider != null && _targetMaterial != null)
            {
                collider.material = _targetMaterial;
            }
        }

        /// <summary>
        /// 应用材质到所有碰撞器
        /// </summary>
        public void ApplyToAllColliders()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                ApplyToCollider(collider);
            }
            DebugHelper.Log($"✅ [PhysicsMaterialController] 材质已应用到 {colliders.Length} 个碰撞器");
        }

        /// <summary>
        /// 使摩擦系数为0（完全光滑）
        /// </summary>
        public void SetFrictionless()
        {
            SetDynamicFriction(0f);
            SetStaticFriction(0f);
            DebugHelper.Log("⚡ [PhysicsMaterialController] 设置为无摩擦");
        }

        /// <summary>
        /// 使摩擦系数为最大值（完全粗糙）
        /// </summary>
        public void SetMaximumFriction()
        {
            SetDynamicFriction(_maxFriction);
            SetStaticFriction(_maxFriction);
            DebugHelper.Log("🧱 [PhysicsMaterialController] 设置为最大摩擦");
        }
    }
}
