using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UNSLOW.UnityUtils
{
    public static class Misc
    {
        ///  指定のオブジェクトを全削除する
        public static void DestroyObjects(List<GameObject> objects)
        {
            if (objects == null)
                return;

#if UNITY_EDITOR
            // エディタ上では遅延削除する
            if (!EditorApplication.isPlaying)
            {
                var os = objects.ToArray();

                EditorApplication.delayCall += () =>
                {
                    foreach (var o in os)
                        Object.DestroyImmediate(o);
                };
            }
            else
#endif
            {
                foreach (var o in objects)
                    Object.Destroy(o);
            }
        }

        //  指定のオブジェクトを全削除する
        public static void
            DestroyObjects(GameObject[] objects)
        {
#if UNITY_EDITOR
            // エディタ上では遅延削除する
            if (!EditorApplication.isPlaying)
            {
                var os = new GameObject[objects.Length];
                objects.CopyTo(os, 0);

                EditorApplication.delayCall += () =>
                {
                    foreach (var o in os)
                        Object.DestroyImmediate(o);
                };
            }
            else
#endif
            {
                foreach (var o in objects)
                    Object.Destroy(o);
            }
        }

        //  指定オブジェクトのメッシュを結合して返す
        public static Mesh CombineMeshes(GameObject[] objects)
        {
            if (objects.Length == 0)
                return null;

            var combine = new CombineInstance[objects.Length];

            var i = 0;
            foreach (var o in objects)
            {
                combine[i].mesh = o.GetComponent<MeshFilter>().sharedMesh;
                combine[i].transform = o.transform.localToWorldMatrix;
                ++i;
            }

            //  メッシュの結合
            var mesh = new Mesh
            {
                name = "Combined"
            };

            mesh.CombineMeshes(combine);

            return mesh;
        }

        //  Enumの長さを得る
        public static int
            EnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        //  タッチ的な操作があったらとりあえずtrueを返す
        public static bool
            HasTouch()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space))
                return true;
#endif
            return Input.touches.Any(t => t.phase == TouchPhase.Began);
        }

        //  uint32をColor32に変換する
        public static Color32 ToColor32(uint rgba)
        {
            return new Color32(
                (byte)(rgba >> 24),
                (byte)(rgba >> 16),
                (byte)(rgba >> 8),
                (byte)(rgba >> 0));
        }

        /// <summary>
        /// UI用のRectTransformから画像の範囲を返す
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            var v = new Vector3[4];
            transform.GetWorldCorners(v);

            var left = (int)(v.Select(c => c.x).Min() + 0.5f);
            var right = (int)(v.Select(c => c.x).Max() + 0.5f);
            var top = (int)(v.Select(c => c.y).Min() + 0.5f);
            var bottom = (int)(v.Select(c => c.y).Max() + 0.5f);

            return new Rect(left, top, right - left, bottom - top);
        }
    }

    public static class GameObjectExtensions
    {
        /// <summary>
        /// すべての子オブジェクトを返します
        /// </summary>
        /// <param name="self">GameObject 型のインスタンス</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
        /// <returns>すべての子オブジェクトを管理する配列</returns>
        public static GameObject[] GetChildren(
            this GameObject self,
            bool includeInactive = false)
        {
            return self.GetComponentsInChildren<Transform>(includeInactive)
                .Where(c => c != self.transform)
                .Select(c => c.gameObject)
                .ToArray();
        }

        /// <summary>
        /// すべての子オブジェクトを削除します
        /// </summary>
        /// <param name="self">GameObject 型のインスタンス</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
        /// <returns>すべての子オブジェクトを管理する配列</returns>
        public static void DestroyAllChildren(
            this GameObject self,
            bool includeInactive = false)
        {
            Misc.DestroyObjects(GetChildren(self, includeInactive));
        }

        /// <summary>
        /// 子オブジェクトを追加します
        /// </summary>
        public static GameObject PutChild(
            this GameObject self,
            GameObject prefab,
            Vector3 position)
        {
            var o = Object.Instantiate(prefab, self.transform, false);
            o.transform.localPosition = position;
            return o;
        }
    }

    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// ラムダ式で遅延実行を呼び出します
        /// </summary>
        public static void DelayCall(this MonoBehaviour self, Action act)
        {
            self.StartCoroutine(CoroutineDelayCall(act));
        }

        /// <summary>
        /// ラムダ式で遅延実行を呼び出します
        /// </summary>
        public static void DelayCall(this MonoBehaviour self, int delayFrame, Action act)
        {
            self.StartCoroutine(CoroutineDelayCall(delayFrame, act));
        }

        /// <summary>
        /// ラムダ式で遅延実行を呼び出します
        /// </summary>
        public static void DelayCall(this MonoBehaviour self, float sec, Action act)
        {
            self.StartCoroutine(CoroutineDelayCall(sec, act));
        }

        /// <summary>
        /// 1フレーム後にラムダ式を呼び出す
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private static IEnumerator CoroutineDelayCall(Action act)
        {
            yield return null;
            act();
        }

        /// <summary>
        /// 指定フレーム後にラムダ式を呼び出す
        /// </summary>
        /// <param name="delayFrame"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private static IEnumerator CoroutineDelayCall(int delayFrame, Action act)
        {
            while (delayFrame > 0)
            {
                --delayFrame;
                yield return null;
            }

            act();
        }

        /// <summary>
        /// 指定秒後にラムダ式を呼び出す
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private static IEnumerator CoroutineDelayCall(float sec, Action act)
        {
            yield return new WaitForSeconds(sec);
            act();
        }
    }

    public static class ComponentExtensions
    {
        /// <summary>
        /// すべての子オブジェクトを返す
        /// </summary>
        /// <param name="self">Component 型のインスタンス</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
        /// <returns>すべての子オブジェクトを管理する配列</returns>
        public static GameObject[] GetChildren(
            this Component self,
            bool includeInactive = false)
        {
            return self.GetComponentsInChildren<Transform>(includeInactive)
                .Where(c => c != self.transform)
                .Select(c => c.gameObject)
                .ToArray();
        }
    }
}