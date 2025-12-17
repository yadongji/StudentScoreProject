using System;
using System.Collections.Generic;

/// <summary>
/// 全局事件系统 - 用于模块间通信
/// </summary>
public static class EventSystem
{
    private static Dictionary<string, Delegate> _eventTable = new Dictionary<string, Delegate>();

    /// <summary>
    /// 订阅事件
    /// </summary>
    public static void Subscribe<T>(string eventName, Action<T> handler)
    {
        if (_eventTable.TryGetValue(eventName, out var existingDelegate))
        {
            _eventTable[eventName] = Delegate.Combine(existingDelegate, handler);
        }
        else
        {
            _eventTable[eventName] = handler;
        }
    }

    /// <summary>
    /// 发布事件
    /// </summary>
    public static void Publish<T>(string eventName, T data)
    {
        if (_eventTable.TryGetValue(eventName, out var eventDelegate))
        {
            (eventDelegate as Action<T>)?.Invoke(data);
        }
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    public static void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (_eventTable.TryGetValue(eventName, out var existingDelegate))
        {
            _eventTable[eventName] = Delegate.Remove(existingDelegate, handler);
        }
    }
}