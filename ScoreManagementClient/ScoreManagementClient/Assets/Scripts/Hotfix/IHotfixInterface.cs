namespace Hotfix
{
    /// <summary>
    /// 热更新接口 - 用于Xlua热更新
    /// 注意：这是一个占位接口，实际使用Xlua时会通过LuaEnv调用
    /// </summary>
    public interface IHotfixInterface
    {
        /// <summary>
        /// 初始化热更新
        /// </summary>
        void InitializeHotfix();

        /// <summary>
        /// 检查是否启用热更新
        /// </summary>
        bool IsHotfixEnabled();

        /// <summary>
        /// 执行热更新脚本
        /// </summary>
        /// <param name="scriptName">脚本名称</param>
        void ExecuteScript(string scriptName);

        /// <summary>
        /// 加载热更新资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载的资源</returns>
        T LoadAsset<T>(string assetName) where T : class;
    }
}
