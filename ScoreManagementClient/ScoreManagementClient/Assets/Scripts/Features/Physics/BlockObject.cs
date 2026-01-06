using UnityEngine;

namespace Features.Physics
{
    /// <summary>
    /// 块状物理对象 - PhysicsObject 的具体实现类
    /// </summary>
    public class BlockObject : PhysicsObject
    {
        /// <summary>
        /// 可选：重写初始化方法以添加特定逻辑
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();
        }

        /// <summary>
        /// 可选：重写物理初始化方法以添加特定逻辑
        /// </summary>
        protected override void InitializePhysics()
        {
            base.InitializePhysics();
        }
    }
}
