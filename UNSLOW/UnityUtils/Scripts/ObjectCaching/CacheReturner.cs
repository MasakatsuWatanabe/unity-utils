using UnityEngine;

namespace UNSLOW.UnityUtils.ObjectCaching
{
    /// <summary>
    /// CacheStoreから貸し出されたオブジェクトの返却のためのコンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public class CacheReturner : MonoBehaviour
    {
        private GameObjectCache _gameObjectCache;
        
        /// <summary>
        /// キャッシュ情報を登録する
        /// </summary>
        /// <param name="gameObjectCache"></param>
        internal void SetCacheInfo(GameObjectCache gameObjectCache)
        {
            _gameObjectCache = gameObjectCache;
        }

        /// <summary>
        /// オブジェクトの返却
        /// </summary>
        public void ReturnToCache()
        {
#if UNITY_EDITOR
            if (_gameObjectCache == null)
            {
                // 単体テストでは無視していい
                Debug.LogWarning("ObjectCache.Returner.Release() is called before SetCacheInfo(). This is not a problem in unit tests.", this);
                gameObject.SetActive(false);
                return;
            }
#endif
            _gameObjectCache.ReturnObject(this);
        }
    }
}
