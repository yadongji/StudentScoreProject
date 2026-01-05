using UnityEngine;

namespace Core.Base
{
    /// <summary>
    /// Controller抽象基类 - 控制器基类
    /// </summary>
    public abstract class BaseController : MonoBehaviour
    {
        protected bool _isInitialized = false;
        protected bool _isEnabled = true;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled => _isEnabled;

        protected virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化控制器
        /// </summary>
        public virtual void Initialize()
        {
            if (_isInitialized) return;
            OnInitialize();
            _isInitialized = true;
        }

        /// <summary>
        /// 初始化时调用，子类可重写
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 启用控制器
        /// </summary>
        public virtual void Enable()
        {
            if (_isEnabled) return;
            _isEnabled = true;
            OnEnable();
        }

        /// <summary>
        /// 启用时调用，子类可重写
        /// </summary>
        protected virtual void OnEnable() { }

        /// <summary>
        /// 禁用控制器
        /// </summary>
        public virtual void Disable()
        {
            if (!_isEnabled) return;
            OnDisable();
            _isEnabled = false;
        }

        /// <summary>
        /// 禁用时调用，子类可重写
        /// </summary>
        protected virtual void OnDisable() { }

        /// <summary>
        /// 释放控制器资源
        /// </summary>
        public virtual void Dispose()
        {
            if (!_isInitialized) return;
            OnDispose();
            _isInitialized = false;
        }

        /// <summary>
        /// 释放时调用，子类可重写
        /// </summary>
        protected virtual void OnDispose() { }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
