using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// 扩展方法集合
    /// </summary>
    public static class ExtensionMethods
    {
        #region Transform 扩展

        /// <summary>
        /// 重置Transform
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        public static void SetX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        /// <summary>
        /// 设置Y坐标
        /// </summary>
        public static void SetY(this Transform transform, float y)
        {
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        /// <summary>
        /// 设置Z坐标
        /// </summary>
        public static void SetZ(this Transform transform, float z)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        /// <summary>
        /// 查找子物体（包含隐藏的）
        /// </summary>
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                var result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        #region GameObject 扩展

        /// <summary>
        /// 安全获取或添加组件
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// 获取父对象组件（包括自身）
        /// </summary>
        public static T GetComponentInParentIncludeSelf<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component != null)
                return component;
            return go.GetComponentInParent<T>();
        }

        #endregion

        #region Vector3 扩展

        /// <summary>
        /// 向量四舍五入
        /// </summary>
        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(
                Mathf.Round(v.x),
                Mathf.Round(v.y),
                Mathf.Round(v.z)
            );
        }

        /// <summary>
        /// 向量保留指定小数位
        /// </summary>
        public static Vector3 RoundToDecimals(this Vector3 v, int decimals)
        {
            float multiplier = Mathf.Pow(10f, decimals);
            return new Vector3(
                Mathf.Round(v.x * multiplier) / multiplier,
                Mathf.Round(v.y * multiplier) / multiplier,
                Mathf.Round(v.z * multiplier) / multiplier
            );
        }

        #endregion

        #region Color 扩展

        /// <summary>
        /// 设置颜色透明度
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// 颜色转十六进制字符串
        /// </summary>
        public static string ToHex(this Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        #endregion

        #region String 扩展

        /// <summary>
        /// 判断字符串是否为空或空白
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串是否为空或只有空白字符
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;
            return str.Substring(0, maxLength) + "...";
        }

        #endregion

        #region List 扩展

        /// <summary>
        /// 随机获取列表中的一个元素
        /// </summary>
        public static T RandomItem<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// 随机打乱列表
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null || list.Count < 2)
                return;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// 判断列表是否为空或null
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        #endregion

        #region Coroutine 扩展

        /// <summary>
        /// 安全启动协程
        /// </summary>
        public static Coroutine SafeStartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine)
        {
            if (monoBehaviour != null && coroutine != null)
                return monoBehaviour.StartCoroutine(coroutine);
            return null;
        }

        /// <summary>
        /// 安全停止协程
        /// </summary>
        public static void SafeStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine coroutine)
        {
            if (monoBehaviour != null && coroutine != null)
                monoBehaviour.StopCoroutine(coroutine);
        }

        #endregion
    }
}
