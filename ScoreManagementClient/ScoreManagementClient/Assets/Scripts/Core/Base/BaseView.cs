using UnityEngine;


/// <summary>
/// View抽象基类 - 视图层基类
/// </summary>
public abstract class BaseView : MonoBehaviour
{
    protected bool _isInitialized = false;
    protected bool _isVisible = true;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool IsVisible => _isVisible;

    protected virtual void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// 初始化视图
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
    protected virtual void OnInitialize()
    {
    }

    /// <summary>
    /// 显示视图
    /// </summary>
    public virtual void Show()
    {
        if (_isVisible) return;
        gameObject.SetActive(true);
        _isVisible = true;
        OnShow();
    }

    /// <summary>
    /// 显示时调用，子类可重写
    /// </summary>
    protected virtual void OnShow()
    {
    }

    /// <summary>
    /// 隐藏视图
    /// </summary>
    public virtual void Hide()
    {
        if (!_isVisible) return;
        OnHide();
        gameObject.SetActive(false);
        _isVisible = false;
    }

    /// <summary>
    /// 隐藏时调用，子类可重写
    /// </summary>
    protected virtual void OnHide()
    {
    }

    /// <summary>
    /// 释放视图资源
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
    protected virtual void OnDispose()
    {
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }
}