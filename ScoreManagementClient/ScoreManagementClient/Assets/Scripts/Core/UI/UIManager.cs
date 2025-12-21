using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI 管理器 - 管理所有 UI 视图的显示和隐藏
/// </summary>
public class UIManager : MonoBehaviour
{
    private Dictionary<System.Type, BaseView> _viewCache = new Dictionary<System.Type, BaseView>();

    /// <summary>
    /// 显示指定类型的视图
    /// </summary>
    public T ShowView<T>() where T : BaseView
    {
        var viewType = typeof(T);

        // 从缓存获取或创建新视图
        if (!_viewCache.TryGetValue(viewType, out var view))
        {
            view = FindObjectOfType<T>();
            if (view == null)
            {
                DebugHelper.LogError($"❌ 未找到视图: {viewType.Name}");
                return null;
            }

            _viewCache[viewType] = view;
        }

        view.Show();
        return view as T;
    }

    /// <summary>
    /// 隐藏指定类型的视图
    /// </summary>
    public void HideView<T>() where T : BaseView
    {
        var viewType = typeof(T);
        if (_viewCache.TryGetValue(viewType, out var view))
        {
            view.Hide();
        }
    }

    /// <summary>
    /// 隐藏所有视图
    /// </summary>
    public void HideAllViews()
    {
        foreach (var view in _viewCache.Values)
        {
            view.Hide();
        }
    }
}