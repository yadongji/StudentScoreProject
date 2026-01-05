using System;

namespace Hotfix
{
    /// <summary>
    /// 热更新标记特性 - 标记需要热更新的类或方法
    /// 注意：这是Xlua热更新的预留接口，实际使用Xlua时会用XLua的标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class HotfixAttribute : Attribute
    {
        /// <summary>
        /// 是否启用热更新
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        public HotfixAttribute() { }

        public HotfixAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        public HotfixAttribute(bool enabled, int priority)
        {
            Enabled = enabled;
            Priority = priority;
        }
    }

    /// <summary>
    /// 不需要热更新标记特性 - 标记不需要热更新的类或方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class NoHotfixAttribute : Attribute
    {
        public NoHotfixAttribute() { }
    }

    /// <summary>
    /// 热更新标签枚举 - 用于分类管理热更新模块
    /// </summary>
    [Flags]
    public enum HotfixTag
    {
        None = 0,
        UI = 1 << 0,
        Logic = 1 << 1,
        Network = 1 << 2,
        Data = 1 << 3,
        All = UI | Logic | Network | Data
    }

    /// <summary>
    /// 热更新标签特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HotfixTagAttribute : Attribute
    {
        public HotfixTag Tag { get; set; }

        public HotfixTagAttribute(HotfixTag tag)
        {
            Tag = tag;
        }
    }
}
