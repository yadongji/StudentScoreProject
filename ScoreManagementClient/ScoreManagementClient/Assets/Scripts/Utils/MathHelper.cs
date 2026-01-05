using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 数学帮助类
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// 角度转弧度
        /// </summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        public static float RadiansToDegrees(float radians)
        {
            return radians * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 线性插值（无限制）
        /// </summary>
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// 向量线性插值（无限制）
        /// </summary>
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        public static float SmoothDamp(float current, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDamp(current, target, ref velocity, smoothTime);
        }

        /// <summary>
        /// 向量平滑阻尼
        /// </summary>
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref velocity, smoothTime);
        }

        /// <summary>
        /// 判断浮点数是否相等
        /// </summary>
        public static bool Approximately(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        /// <summary>
        /// 限制数值范围（无限制）
        /// </summary>
        public static float ClampUnclamped(float value, float min, float max)
        {
            return value;
        }

        /// <summary>
        /// 循环限制数值范围
        /// </summary>
        public static float Repeat(float value, float length)
        {
            return Mathf.Repeat(value, length);
        }

        /// <summary>
        /// 计算两个向量的距离平方（比距离计算更快）
        /// </summary>
        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        /// <summary>
        /// 计算两个向量的距离
        /// </summary>
        public static float Distance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// 计算两点间角度
        /// </summary>
        public static float Angle(Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        /// <summary>
        /// 计算两点间角度（带符号）
        /// </summary>
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        /// <summary>
        /// 归一化角度到 -180 到 180 之间
        /// </summary>
        public static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// 归一化角度到 0 到 360 之间
        /// </summary>
        public static float NormalizeAnglePositive(float angle)
        {
            while (angle < 0f) angle += 360f;
            while (angle >= 360f) angle -= 360f;
            return angle;
        }

        /// <summary>
        /// 判断值是否在范围内
        /// </summary>
        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断值是否在范围内（不包含边界）
        /// </summary>
        public static bool InRangeExclusive(float value, float min, float max)
        {
            return value > min && value < max;
        }

        /// <summary>
        /// PingPong 往复运动
        /// </summary>
        public static float PingPong(float value, float length)
        {
            return Mathf.PingPong(value, length);
        }

        /// <summary>
        /// 随机范围浮点数
        /// </summary>
        public static float RandomRange(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 随机范围整数
        /// </summary>
        public static int RandomRange(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 随机点在圆内
        /// </summary>
        public static Vector2 RandomPointInCircle(float radius)
        {
            float angle = RandomRange(0f, 2f * Mathf.PI);
            float r = Mathf.Sqrt(RandomRange(0f, 1f)) * radius;
            return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
        }

        /// <summary>
        /// 随机点在球体内
        /// </summary>
        public static Vector3 RandomPointInSphere(float radius)
        {
            float theta = RandomRange(0f, 2f * Mathf.PI);
            float phi = Mathf.Acos(RandomRange(-1f, 1f));
            float r = Mathf.Pow(RandomRange(0f, 1f), 1f / 3f) * radius;

            float x = r * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = r * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = r * Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public static float Min(float a, float b)
        {
            return Mathf.Min(a, b);
        }

        /// <summary>
        /// 最小值（多个）
        /// </summary>
        public static float Min(params float[] values)
        {
            return Mathf.Min(values);
        }

        /// <summary>
        /// 最大值
        /// </summary>
        public static float Max(float a, float b)
        {
            return Mathf.Max(a, b);
        }

        /// <summary>
        /// 最大值（多个）
        /// </summary>
        public static float Max(params float[] values)
        {
            return Mathf.Max(values);
        }

        /// <summary>
        /// 绝对值
        /// </summary>
        public static float Abs(float value)
        {
            return Mathf.Abs(value);
        }

        /// <summary>
        /// 符号
        /// </summary>
        public static float Sign(float value)
        {
            return Mathf.Sign(value);
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        public static int Ceil(float value)
        {
            return Mathf.CeilToInt(value);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        public static int Floor(float value)
        {
            return Mathf.FloorToInt(value);
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        public static int Round(float value)
        {
            return Mathf.RoundToInt(value);
        }

        /// <summary>
        /// 取整（向零方向）
        /// </summary>
        public static int Truncate(float value)
        {
            return (int)value;
        }

        /// <summary>
        /// 幂运算
        /// </summary>
        public static float Pow(float x, float y)
        {
            return Mathf.Pow(x, y);
        }

        /// <summary>
        /// 平方根
        /// </summary>
        public static float Sqrt(float value)
        {
            return Mathf.Sqrt(value);
        }
    }
}
