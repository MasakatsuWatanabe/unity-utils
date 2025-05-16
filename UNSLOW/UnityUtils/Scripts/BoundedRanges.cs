using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    ///     値が指定範囲内かを表すScriptableObject
    /// </summary>
    [CreateAssetMenu(menuName = "UNSLOW/CommonAssets/BoundedRanges")]
    public class BoundedRanges : ScriptableObject
    {
        /// <summary>
        ///     範囲
        /// </summary>
        [Serializable]
        public class Range
        {
            public float start;
            public float length;
            public float End => start + length;

            public Range(float start, float length)
            {
                this.start = start;
                this.length = length;
            }

            public bool Contains(float t)
            {
                return t >= start && t <= End;
            }
        }

        /// <summary>
        ///     範囲指定
        /// </summary>
        [SerializeField] private Range[] ranges;

        public Range[] Ranges => ranges;

        /// <summary>
        ///     現在時間に基づいてフラグの状態を取得
        /// </summary>
        public bool Contains(float t)
        {
            Assert.IsTrue(IsSorted());

            int low = 0, high = ranges.Length - 1;

            while (low <= high)
            {
                var mid = (low + high) / 2;
                if (t < ranges[mid].start)
                    high = mid - 1;
                else if (t > ranges[mid].End)
                    low = mid + 1;
                else
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     ソートされていればtrueを返す
        ///     指定がソートされているのは使用の前提条件なので、基本的にはエラーチェック用
        /// </summary>
        public bool IsSorted()
        {
            if (ranges == null)
                return true;

            for (var i = 1; i < ranges.Length; i++)
                if (ranges[i - 1].start > ranges[i].start)
                    return false;

            return true;
        }

        /// <summary>
        ///     Rangesをソートする
        /// </summary>
        public void SortRanges()
        {
            if (ranges == null)
                return;

            Array.Sort(ranges, (a, b) => a.start.CompareTo(b.start));
        }
    }
}