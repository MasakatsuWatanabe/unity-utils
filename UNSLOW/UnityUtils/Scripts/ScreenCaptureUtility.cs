using System;
using System.Collections;
using UnityEngine;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// スクリーンのキャプチャユーティリティ
    /// Texture2Dを生成して返す
    /// </summary>
    public class ScreenCaptureUtility : MonoBehaviour
    {
        private static ScreenCaptureUtility _instance;

        private void Awake()
        {
            if (_instance)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        /// <summary>
        /// フルスクリーンでのキャプチャ
        /// </summary>
        public static IEnumerator DoCapture(Action<Texture2D> cap = null)
        {
            yield return CaptureRoutine(cap);
            yield break;

            static IEnumerator CaptureRoutine(Action<Texture2D> cap)
            {
                yield return new WaitForEndOfFrame();

                var tex =
                    new Texture2D(
                        Resolution.Width,
                        Resolution.Height,
                        TextureFormat.RGB24,
                        false
                    )
                    {
                        filterMode = FilterMode.Point
                    };

                tex.ReadPixels(new Rect(0, 0, Resolution.Width, Resolution.Height), 0, 0);
                tex.Apply();

                cap?.Invoke(tex);
            }
        }

        /// <summary>
        /// UIサイズでのキャプチャ
        /// </summary>
        /// <param name="cap">キャプチャー完了時のコールバック関数 必須</param>
        /// <param name="rectTransform">UI RectTransformで指定するキャプチャ範囲</param>
        public static IEnumerator DoCapture(RectTransform rectTransform, Action<Texture2D> cap = null)
        {
            yield return CaptureRoutine(rectTransform, cap);
            yield break;

            static IEnumerator CaptureRoutine(RectTransform rectTransform, Action<Texture2D> cap = null)
            {
                yield return new WaitForEndOfFrame();

                var rect = Misc.RectTransformToScreenSpace(rectTransform);

                var tex =
                    new Texture2D(
                        (int)rect.width,
                        (int)rect.height,
                        TextureFormat.RGB24,
                        false
                    )
                    {
                        filterMode = FilterMode.Point
                    };

                tex.ReadPixels(rect, 0, 0);
                tex.Apply();

                cap?.Invoke(tex);
            }
        }
    }
}
