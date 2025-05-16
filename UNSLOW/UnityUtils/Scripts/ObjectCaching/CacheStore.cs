using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UNSLOW.UnityUtils.ObjectCaching
{
    /// <summary>
    /// キャッシュ辞書
    /// インスタンスIDから適切なキャッシュを取り出して利用する
    /// 辞書引きコストがあるため、自明な場合はキャッシュを個別に管理した方がよい
    /// </summary>
    public class CacheStore : MonoBehaviour
    {
        private readonly Dictionary<int, GameObjectCache> _cacheDic = new();

        public static CacheStore Instance { get; private set; }

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
        }

        /// <summary>
        /// キャッシュの取得
        /// 存在しない場合は新規に作成して返す
        /// Prefabが自明な場合はこちらの利用を推奨
        /// 返却時の親オブジェクトには自身を指定する
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObjectCache GetCache(GameObject prefab)
        {
            var id = prefab.GetInstanceID();

            if(_cacheDic.TryGetValue(id, out var cache))
                return cache;

            // 辞書にキャッシュが無いので新規作成
            var newCache =  new GameObjectCache(prefab, transform);
            
            _cacheDic[id] = newCache;
            return newCache;
        }
        
        /// <summary>
        /// オブジェクトを借りる
        /// </summary>
        public GameObject BorrowObject(GameObject prefab)
        {
            return GetCache(prefab).BorrowObject();
        }

        /// <summary>
        /// オブジェクトを借りる
        /// </summary>
        public  T BorrowObject<T>(GameObject prefab)
        {
            return BorrowObject(prefab).GetComponent<T>();
        }
    }
}
