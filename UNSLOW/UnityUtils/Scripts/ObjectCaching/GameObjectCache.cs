using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UNSLOW.UnityUtils.ObjectCaching
{
    /// <summary>
    /// ゲームオブジェクトキャッシュ
    /// </summary>
    public class GameObjectCache
    {
        private readonly Stack<GameObject> _stack = new();
        private readonly GameObject _prefab;
        private readonly Transform _parentTransform;

        /// <summary>
        /// コンストラクタ
        /// prefabはICacheableを実装していなければならない
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parentTransform"></param>
        public GameObjectCache(GameObject prefab, Transform parentTransform)
        {
            Assert.IsNotNull(
                prefab.GetComponent<ICacheable>(),
                "The provided prefab must contain a component that implements ICacheable.");
            
            _prefab = prefab;
            _parentTransform = parentTransform;
        }

        /// <summary>
        /// キャッシュ内容の全廃棄
        /// 保持していたオブジェクトも全て破棄される
        /// </summary>
        public void CleanUp()
        {
            foreach (var go in _stack)
                Object.Destroy(go);
        }

        /// <summary>
        /// オブジェクトキャッシュからオブジェクトを貸し出す
        /// キャッシュ内に存在しない場合は生成する
        /// 指定があれば返却時に親を設定する
        /// </summary>
        /// <returns></returns>
        public GameObject BorrowObject()
        {
            GameObject instance;

            if (_stack.Count > 0)
            {
                instance = _stack.Pop();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = Object.Instantiate(_prefab, _parentTransform);
            }
            
            var cacheable = instance.GetComponent<ICacheable>().Returner;
            cacheable.SetCacheInfo(this);

            return instance;
        }

        /// <summary>
        /// オブジェクトキャッシュからオブジェクトを貸し出す
        /// キャッシュ内に存在しない場合は生成する
        /// </summary>
        /// <returns></returns>
        public T BorrowObject<T>()
        {
            return BorrowObject().GetComponent<T>();
        }

        /// <summary>
        /// オブジェクトを返却する
        /// </summary>
        internal void ReturnObject(CacheReturner returner)
        {
            var go = returner.gameObject;
            
            if(_parentTransform != null)
                go.transform.SetParent(_parentTransform);
            
            go.SetActive(false);
            
            _stack.Push(go);
        }
    }
}
