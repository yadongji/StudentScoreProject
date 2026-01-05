using UnityEngine;
using Utils;

namespace Hotfix
{
    /// <summary>
    /// 热更新桥接器 - 用于C#与Lua之间的通信
    /// 注意：当前为接口预留实现，实际Xlua集成需要引入Xlua包
    /// </summary>
    public class HotfixBridge : MonoBehaviour
    {
        public static HotfixBridge Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        #region C#调用Lua

        /// <summary>
        /// 调用Lua函数
        /// </summary>
        /// <param name="luaTable">Lua表名</param>
        /// <param name="functionName">函数名</param>
        /// <param name="args">参数</param>
        public void CallLuaFunction(string luaTable, string functionName, params object[] args)
        {
            if (!HotfixManager.Instance.IsHotfixEnabled())
            {
                DebugHelper.LogWarning("⚠️ [HotfixBridge] 热更新未启用");
                return;
            }

            // TODO: 集成Xlua时，实现Lua函数调用
            DebugHelper.Log($"📞 [HotfixBridge] 调用Lua函数: {luaTable}.{functionName}");
        }

        /// <summary>
        /// 调用Lua函数并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        public T CallLuaFunction<T>(string luaTable, string functionName, params object[] args)
        {
            if (!HotfixManager.Instance.IsHotfixEnabled())
            {
                DebugHelper.LogWarning("⚠️ [HotfixBridge] 热更新未启用");
                return default(T);
            }

            // TODO: 集成Xlua时，实现Lua函数调用并返回结果
            DebugHelper.Log($"📞 [HotfixBridge] 调用Lua函数: {luaTable}.{functionName}");
            return default(T);
        }

        /// <summary>
        /// 获取Lua全局变量
        /// </summary>
        /// <typeparam name="T">变量类型</typeparam>
        public T GetLuaGlobal<T>(string globalName)
        {
            if (!HotfixManager.Instance.IsHotfixEnabled())
            {
                DebugHelper.LogWarning("⚠️ [HotfixBridge] 热更新未启用");
                return default(T);
            }

            // TODO: 集成Xlua时，获取Lua全局变量
            DebugHelper.Log($"🔍 [HotfixBridge] 获取Lua全局变量: {globalName}");
            return default(T);
        }

        #endregion

        #region Lua调用C#

        /// <summary>
        /// Lua可以调用此方法（需要使用XLua标记）
        /// [LuaCallCSharp]
        /// </summary>
        public static void LogToUnity(string message)
        {
            DebugHelper.Log($"[Lua] {message}");
        }

        /// <summary>
        /// Lua可以调用此方法（需要使用XLua标记）
        /// [LuaCallCSharp]
        /// </summary>
        public static void WarningToUnity(string message)
        {
            DebugHelper.LogWarning($"[Lua] {message}");
        }

        /// <summary>
        /// Lua可以调用此方法（需要使用XLua标记）
        /// [LuaCallCSharp]
        /// </summary>
        public static void ErrorToUnity(string message)
        {
            DebugHelper.LogError($"[Lua] {message}");
        }

        #endregion

        #region 热更新事件

        /// <summary>
        /// 热更新完成事件
        /// </summary>
        public void OnHotfixComplete()
        {
            DebugHelper.Log("✅ [HotfixBridge] 热更新完成");
            EventSystem.Publish("HotfixComplete", true);
        }

        /// <summary>
        /// 热更新失败事件
        /// </summary>
        public void OnHotfixFailed(string error)
        {
            DebugHelper.LogError($"❌ [HotfixBridge] 热更新失败: {error}");
            EventSystem.Publish("HotfixFailed", error);
        }

        #endregion
    }
}
