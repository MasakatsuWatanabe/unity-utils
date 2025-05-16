using UnityEngine;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 便利系数学関数類
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// イプシロン値
        /// </summary>
        public const float Epsilon = 0.001f;

        /// <summary>
        /// 微小値ならtrue
        /// </summary>
        public static bool IsEpsilon(float r)
        {
            return r is >= -Epsilon and <= +Epsilon;
        }

        /// <summary>
        /// 誤差が微笑値未満ならtrue
        /// </summary>
        public static bool IsEpsilonEquals(float a, float b)
        {
            return IsEpsilon(a - b);
        }

        /// <summary>
        /// Vector2の外積を得る
        /// </summary>
        public static float Cross2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        /// <summary>
        /// 2Dの線分A-BとC-Dの交点を得る
        /// </summary>
        public static bool LineIntersection2D(
            Vector2 a,
            Vector2 b,
            Vector2 c,
            Vector2 d,
            out Vector2 intersection)
        {
            return RayIntersection2D(a, b - a, c, d - c, out intersection);
        }

        /// <summary>
        /// 2Dの線分A-BとC-Dの交点を得る
        /// </summary>
        public static bool LineIntersection2D(
                Vector2 a,
                Vector2 b,
                Vector2 c,
                Vector2 d,
                out Vector2 intersection,
                out float dR,
                out float dS)
        {
            return RayIntersection2D(a, b - a, c, d - c, out intersection, out dR, out dS);
        }

        /// <summary>
        /// p0基点のv0ベクトルとp1基点のv1ベクトルとの最近傍点を得る
        /// </summary>
        public static void RayNearest2D(
                Vector2 p0,
                Vector2 v0, //  B-A
                Vector2 p1,
                Vector2 v1, //  D-C
                out float dR,
                out float dS)
        {
            var cross = Cross2D(v0, v1);

            if (System.Math.Abs(cross) < Epsilon)
            {
                dR = dS = Mathf.Infinity;
                return;
            }

            var p01 = p1 - p0;

            dR =
                (v1.y * p01.x
                 - v1.x * p01.y) / cross;

            dS =
                (v0.y * p01.x
                 - v0.x * p01.y) / cross;
        }

        public static bool RayIntersection2D(
                Vector2 p0,
                Vector2 v0, //  B-A
                Vector2 p1,
                Vector2 v1, //  D-C
                out Vector2 intersection)
        {
            return RayIntersection2D(p0, v0, p1, v1, out intersection, out _, out _);
        }

        public static bool RayIntersection2D(
                Vector2 p0,
                Vector2 v0, //  B-A
                Vector2 p1,
                Vector2 v1, //  D-C
                out Vector2 intersection,
                out float dR,
                out float dS)
        {
            RayNearest2D(p0, v0, p1, v1, out dR, out dS);

            if (dR >= 0f && dR <= 1f &&
                dS >= 0f && dS <= 1f)
            {
                intersection = p0 + dR * v0;
                return true;
            }

            intersection = Vector2.zero;
            return false;
        }
        
        // Quaternionの拡張メソッド
        public static float Angle(this Quaternion q)
        {
            return 2f * Mathf.Acos(q.w);
        }

        public static float Angle(this Quaternion q, Quaternion other)
        {
            return Mathf.Acos(
                Mathf.Clamp(
                    2f * (q.w * other.w + q.x * other.x + q.y * other.y + q.z * other.z) - 1f,
                    -1f,
                    1f));
        }
        
        // シグモイド関数
        public static float Sigmoid(float x)
        {
            return 1f / (1f + Mathf.Exp(-x));
        }

        // SmoothStep
        public static float SmoothStep(float x)
        {
            return x * x * (3f - 2f * x);
        }
        
        // より柔らかいSmoothStep
        public static float SmootherStep(float x)
        {
            var ix = 1.0f - x;       
            x *= x;
            return x / (x + ix * ix);                
        }
        
        // 三次Ease-Out
        public static float EaseOutCubic(float x)
        {
            var ix = 1.0f - x;
            return 1.0f - ix * ix * ix;
        }
        
        // リマップ
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        }
    }
}
