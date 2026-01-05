using UnityEngine;
using Core.Base;
using Utils;

namespace Hotfix
{
    /// <summary>
    /// 热更新管理器 - 负责Xlua热更新的管理
    /// 注意：当前为接口预留实现，实际Xlua集成需要引入Xlua包
    /// </summary>
    public class HotfixManager : BaseManager, IHotfixInterface
    {
        public static HotfixManager Instance { get; private set; }

    [Header("热更新配置")]
    [SerializeField] private bool _enableHotfix = false;
    [SerializeField] private string _hotfixAssetPath = "Hotfix";

    private new bool _isInitialized = false;

        /// <summary>
        /// 是否启用热更新
        /// </summary>
        public bool EnableHotfix => _enableHotfix;

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (_enableHotfix)
            {
                InitializeHotfix();
            }

            DebugHelper.Log($"✅ [HotfixManager] 初始化完成 | 热更新启用: {_enableHotfix}");
        }

        #region IHotfixInterface 实现

        /// <summary>
        /// 初始化热更新
        /// </summary>
        public void InitializeHotfix()
        {
            if (_isInitialized)
            {
                DebugHelper.LogWarning("⚠️ [HotfixManager] 热更新已初始化，无需重复初始化");
                return;
            }

            // TODO: 集成Xlua时，在这里初始化LuaEnv
            // 示例代码（需要引入Xlua）：
            // _luaEnv = new LuaEnv();
            // _luaEnv.AddLoader(CustomLoader);
            // _luaEnv.DoString("require 'main'");

            _isInitialized = true;
            DebugHelper.Log("✅ [HotfixManager] 热更新初始化完成");
        }

        /// <summary>
        /// 检查是否启用热更新
        /// </summary>
        public bool IsHotfixEnabled()
        {
            return _enableHotfix;
        }

        /// <summary>
        /// 执行热更新脚本
        /// </summary>
        /// <param name="scriptName">脚本名称</param>
        public void ExecuteScript(string scriptName)
        {
            if (!_enableHotfix || !_isInitialized)
            {
                DebugHelper.LogWarning("⚠️ [HotfixManager] 热更新未启用或未初始化，无法执行脚本");
                return;
            }

            // TODO: 集成Xlua时，在这里执行Lua脚本
            // 示例代码（需要引入Xlua）：
            // _luaEnv.DoString($"require '{scriptName}'");

            DebugHelper.Log($"📝 [HotfixManager] 执行热更新脚本: {scriptName}");
        }

        /// <summary>
        /// 加载热更新资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载的资源</returns>
        public T LoadAsset<T>(string assetName) where T : class
        {
            if (!_enableHotfix)
            {
                DebugHelper.LogWarning("⚠️ [HotfixManager] 热更新未启用");
                return null;
            }

            // TODO: 集成Xlua时，在这里加载热更新资源
            // 示例代码：
            // var asset = Resources.Load<T>($"{_hotfixAssetPath}/{assetName}");
            // return asset;

            DebugHelper.Log($"📦 [HotfixManager] 加载热更新资源: {assetName}");
            return null;
        }

        #endregion

        #region Xlua 集成预留接口

        // TODO: 集成Xlua时，取消注释以下代码

        /*
        private XLua.LuaEnv _luaEnv;

        /// <summary>
        /// 自定义Loader - 用于加载Lua脚本
        /// </summary>
        private byte[] CustomLoader(ref string filepath)
        {
            // 从Resources加载Lua文件
            string path = $"{_hotfixAssetPath}/{filepath}";
            TextAsset luaScript = Resources.Load<TextAsset>(path);

            if (luaScript != null)
            {
                return luaScript.bytes;
            }

            return null;
        }

        /// <summary>
        /// 调用Lua函数
        /// </summary>
        public void CallLuaFunction(string module, string function, params object[] args)
        {
            if (_luaEnv == null)
            {
                DebugHelper.LogError("❌ [HotfixManager] LuaEnv未初始化");
                return;
            }

            _luaEnv.Global.Get<XLua.LuaFunction>(module, function)?.Call(args);
        }

        /// <summary>
        /// 刷新Lua虚拟机
        /// </summary>
        public void ReloadLua()
        {
            if (_luaEnv == null)
            {
                InitializeHotfix();
                return;
            }

            _luaEnv.Dispose();
            _luaEnv = new XLua.LuaEnv();
            _luaEnv.AddLoader(CustomLoader);
            _luaEnv.DoString("require 'main'");
        }
        */

        #endregion

        protected override void OnDispose()
        {
            // TODO: 集成Xlua时，在这里释放LuaEnv
            // _luaEnv?.Dispose();
            // _luaEnv = null;

            _isInitialized = false;
            base.OnDispose();
            DebugHelper.Log("🗑️ [HotfixManager] 热更新已释放");
        }

        /// <summary>
        /// 清理垃圾回收
        /// </summary>
        public void LuaGC()
        {
            if (!_enableHotfix || !_isInitialized)
            {
                return;
            }

            // TODO: 集成Xlua时，执行Lua垃圾回收
            // _luaEnv?.Tick();

            DebugHelper.Log("🧹 [HotfixManager] Lua垃圾回收执行");
        }
    }
}
