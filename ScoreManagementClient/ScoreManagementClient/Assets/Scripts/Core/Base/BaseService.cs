namespace Core.Base
{
    /// <summary>
    /// Service抽象基类 - 服务基类
    /// </summary>
    public abstract class BaseService
    {
        protected bool _isInitialized = false;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 初始化服务
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
        /// 释放服务资源
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
    }
}
