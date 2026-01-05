using UnityEngine;

namespace Core.Base
{
    /// <summary>
    /// Manager抽象基类 - 管理器基类
    /// </summary>
    public abstract class BaseManager : MonoBehaviour
    {
        protected bool _isInitialized = false;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        protected virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化管理器
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
        /// 释放管理器资源
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
