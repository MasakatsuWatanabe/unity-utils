using System;
using UnityEngine;

namespace UNSLOW.UnityUtils.Editor
{
    /// <summary>
    ///     開発メモ用属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DevelopmentMemoAttribute : PropertyAttribute
    {
        public string Label { get; }
        public int Lines { get; }

        public DevelopmentMemoAttribute(string label = "Development Memo", int lines = 3)
        {
            Label = label;
            Lines = lines;
        }
    }
}