using UnityEngine;
using System.Collections;

namespace Utils
{
    /// <summary>
    /// 协程运行器 - 用于在任何地方启动协程
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

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
        /// 启动协程
        /// </summary>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        public static void StopCoroutine(Coroutine coroutine)
        {
            if (Instance != null && coroutine != null)
            {
                Instance.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// 停止所有协程
        /// </summary>
        public static void StopAllCoroutines()
        {
            if (Instance != null)
            {
                Instance.StopAllCoroutines();
            }
        }
    }
}
