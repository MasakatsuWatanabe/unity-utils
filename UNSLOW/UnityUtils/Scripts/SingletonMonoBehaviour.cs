using UnityEngine;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    ///     シングルトン MonoBehaviour
    /// </summary>
    /// <typeparam name="T">シングルトン化するMonoBehaviour継承クラス</typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        /// <summary>
        ///     Instanceの初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstance() => Instance = null;
        
        /// <summary>
        ///     Awakeでシングルトンの初期化を行う
        ///     派生クラスで強引にAwake()を潰しまわないよう注意
        /// </summary>
        protected void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;

            OnSingletonAwake();
        }

        /// <summary>
        ///     シングルトンの破棄を行う
        ///     派生クラスで強引にOnDestroy()を潰しまわないよう注意
        /// </summary>
        protected void OnDestroy()
        {
            if (Instance != this)
                return;
            
            OnSingletonDestroy();
            
            Instance = null;
        }

        /// <summary>
        ///     初期化メソッド
        ///     SingletonMonoBehaviourがAwake()を使用しているため
        ///     派生クラスではOnSingletonAwake()の実装を必須としている
        /// </summary>
        protected abstract void OnSingletonAwake();

        /// <summary>
        ///     破棄メソッド
        ///     SingletonMonoBehaviourがOnDestroy()を使用しているため
        ///     派生クラスではOnSingletonDestroy()の実装を必須としている
        /// </summary>
        protected abstract void OnSingletonDestroy();
    }
}
