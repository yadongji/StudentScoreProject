using System;
using System.Collections.Generic;


/// <summary>
/// 全局事件系统 - 用于模块间通信
/// </summary>
public static class EventSystem
{
    private static Dictionary<string, Delegate> _eventTable = new Dictionary<string, Delegate>();

    /// <summary>
    /// 订阅事件（带数据）
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
    /// 订阅事件（无数据）
    /// </summary>
    public static void Subscribe(string eventName, Action handler)
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
    /// 发布事件（带数据）
    /// </summary>
    public static void Publish<T>(string eventName, T data)
    {
        if (_eventTable.TryGetValue(eventName, out var eventDelegate))
        {
            (eventDelegate as Action<T>)?.Invoke(data);
        }
    }

    /// <summary>
    /// 发布事件（无数据）
    /// </summary>
    public static void Publish(string eventName)
    {
        if (_eventTable.TryGetValue(eventName, out var eventDelegate))
        {
            (eventDelegate as Action)?.Invoke();
        }
    }

    /// <summary>
    /// 取消订阅（带数据）
    /// </summary>
    public static void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (_eventTable.TryGetValue(eventName, out var existingDelegate))
        {
            _eventTable[eventName] = Delegate.Remove(existingDelegate, handler);

            // 如果没有订阅者了，移除事件
            if (_eventTable[eventName] == null)
            {
                _eventTable.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 取消订阅（无数据）
    /// </summary>
    public static void Unsubscribe(string eventName, Action handler)
    {
        if (_eventTable.TryGetValue(eventName, out var existingDelegate))
        {
            _eventTable[eventName] = Delegate.Remove(existingDelegate, handler);

            // 如果没有订阅者了，移除事件
            if (_eventTable[eventName] == null)
            {
                _eventTable.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 清空所有事件
    /// </summary>
    public static void ClearAll()
    {
        _eventTable.Clear();
    }

    /// <summary>
    /// 检查事件是否存在订阅者
    /// </summary>
    public static bool HasSubscribers(string eventName)
    {
        if (_eventTable.TryGetValue(eventName, out var eventDelegate))
        {
            return eventDelegate != null;
        }

        return false;
    }
}