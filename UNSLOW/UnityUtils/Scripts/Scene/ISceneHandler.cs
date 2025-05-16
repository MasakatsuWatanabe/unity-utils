namespace UNSLOW.UnityUtils.Scene
{
    /// <summary>
    /// シーンの初期化、終了処理を行うためのインターフェース
    /// 初期化と終了のコンテキストの取り扱いを強制する
    /// </summary>
    public interface ISceneHandler
    {
        /// <summary>
        ///     初期化用コンテキスト
        /// </summary>
        public abstract class InitializationContext
        {
        }
        
        /// <summary>
        /// 終了時に返されるコンテキスト
        /// </summary>
        public abstract class FinalizationContext
        {
        }        

        public void SetInitializationContext(InitializationContext context);
        public FinalizationContext GetFinalizationContext();
    }
    
    /// <summary>
    ///     シーン間の情報を受け渡すためのインターフェース
    /// </summary>
    public interface ISceneHandler<in TInitContextFrom, out TFinalContext> : ISceneHandler
        where TInitContextFrom : ISceneHandler.InitializationContext
        where TFinalContext : ISceneHandler.FinalizationContext
    {
        public void SetInitializationContext(TInitContextFrom context);
        public new TFinalContext GetFinalizationContext();
    }
}
