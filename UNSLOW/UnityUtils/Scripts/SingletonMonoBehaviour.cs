using UnityEngine;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 最小のシングルトンなMonoBehaviour
    /// MonoBehaviourのコールバックには一切干渉しない
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected SingletonMonoBehaviour()
        {
            if( Instance != null )
            {
                Destroy( this );
                return;
            }

            Instance = this as T;
        }

        protected void Revoke()
        {
            Instance = null;
        }
    }
}
