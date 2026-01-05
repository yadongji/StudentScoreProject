using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 对象池 - 用于频繁创建和销毁的对象复用
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class ObjectPool<T> where T : new()
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly System.Func<T> _createFunc;
        private readonly System.Action<T> _resetAction;

        /// <summary>
        /// 对象池当前大小
        /// </summary>
        public int Count => _pool.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectPool() : this(null, null) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="createFunc">创建函数</param>
        public ObjectPool(System.Func<T> createFunc) : this(createFunc, null) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="createFunc">创建函数</param>
        /// <param name="resetAction">重置函数</param>
        public ObjectPool(System.Func<T> createFunc, System.Action<T> resetAction)
        {
            _createFunc = createFunc ?? (() => new T());
            _resetAction = resetAction;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public T Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
            return _createFunc();
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release(T item)
        {
            if (item != null)
            {
                _resetAction?.Invoke(item);
                _pool.Enqueue(item);
            }
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }
    }

    /// <summary>
    /// GameObject对象池
    /// </summary>
    public class GameObjectPool
    {
        private readonly string _name;
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        private readonly int _initialSize;

        /// <summary>
        /// 对象池当前大小
        /// </summary>
        public int Count => _pool.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GameObjectPool(GameObject prefab, Transform parent = null, int initialSize = 0, string name = "Pool")
        {
            _prefab = prefab;
            _parent = parent;
            _initialSize = initialSize;
            _name = name;

            // 预创建对象
            for (int i = 0; i < _initialSize; i++)
            {
                var obj = CreateObject();
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public GameObject Get()
        {
            GameObject obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = CreateObject();
            }

            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                if (_parent != null)
                {
                    obj.transform.SetParent(_parent);
                    obj.transform.Reset();
                }
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                if (obj != null)
                    Object.Destroy(obj);
            }
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        private GameObject CreateObject()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = $"{_name} (Pooled)";
            return obj;
        }
    }
}
