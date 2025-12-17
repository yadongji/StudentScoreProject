using UnityEngine;


/// <summary>
/// UI 视图基类 - 所有视图的父类
/// </summary>
public abstract class BaseView : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 显示视图
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        OnShow();
    }

    /// <summary>
    /// 隐藏视图
    /// </summary>
    public virtual void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
        OnHide();
    }

    /// <summary>
    /// 视图显示时调用（子类重写）
    /// </summary>
    protected virtual void OnShow()
    {
    }

    /// <summary>
    /// 视图隐藏时调用（子类重写）
    /// </summary>
    protected virtual void OnHide()
    {
    }
}