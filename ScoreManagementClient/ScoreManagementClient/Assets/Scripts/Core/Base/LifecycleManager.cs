using System.Collections.Generic;
using UnityEngine;

namespace Core.Base
{
    /// <summary>
    /// 生命周期管理器 - 统一管理所有组件的生命周期
    /// </summary>
    public class LifecycleManager : MonoBehaviour
    {
        private static LifecycleManager _instance;

        public static LifecycleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LifecycleManager");
                    _instance = go.AddComponent<LifecycleManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private List<BaseModel> _models = new List<BaseModel>();
        private List<BaseController> _controllers = new List<BaseController>();
        private List<BaseService> _services = new List<BaseService>();
        private List<BaseManager> _managers = new List<BaseManager>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 注册Model
        /// </summary>
        public void RegisterModel(BaseModel model)
        {
            if (model != null && !_models.Contains(model))
            {
                _models.Add(model);
                model.Initialize();
            }
        }

        /// <summary>
        /// 注册Controller
        /// </summary>
        public void RegisterController(BaseController controller)
        {
            if (controller != null && !_controllers.Contains(controller))
            {
                _controllers.Add(controller);
                controller.Initialize();
            }
        }

        /// <summary>
        /// 注册Service
        /// </summary>
        public void RegisterService(BaseService service)
        {
            if (service != null && !_services.Contains(service))
            {
                _services.Add(service);
                service.Initialize();
            }
        }

        /// <summary>
        /// 注册Manager
        /// </summary>
        public void RegisterManager(BaseManager manager)
        {
            if (manager != null && !_managers.Contains(manager))
            {
                _managers.Add(manager);
                manager.Initialize();
            }
        }

        /// <summary>
        /// 注销Model
        /// </summary>
        public void UnregisterModel(BaseModel model)
        {
            if (model != null && _models.Contains(model))
            {
                model.Dispose();
                _models.Remove(model);
            }
        }

        /// <summary>
        /// 注销Controller
        /// </summary>
        public void UnregisterController(BaseController controller)
        {
            if (controller != null && _controllers.Contains(controller))
            {
                controller.Dispose();
                _controllers.Remove(controller);
            }
        }

        /// <summary>
        /// 注销Service
        /// </summary>
        public void UnregisterService(BaseService service)
        {
            if (service != null && _services.Contains(service))
            {
                service.Dispose();
                _services.Remove(service);
            }
        }

        /// <summary>
        /// 注销Manager
        /// </summary>
        public void UnregisterManager(BaseManager manager)
        {
            if (manager != null && _managers.Contains(manager))
            {
                manager.Dispose();
                _managers.Remove(manager);
            }
        }

        /// <summary>
        /// 清理所有组件
        /// </summary>
        public void CleanupAll()
        {
            foreach (var model in _models)
            {
                model?.Dispose();
            }
            _models.Clear();

            foreach (var controller in _controllers)
            {
                controller?.Dispose();
            }
            _controllers.Clear();

            foreach (var service in _services)
            {
                service?.Dispose();
            }
            _services.Clear();

            foreach (var manager in _managers)
            {
                manager?.Dispose();
            }
            _managers.Clear();
        }
    }
}
