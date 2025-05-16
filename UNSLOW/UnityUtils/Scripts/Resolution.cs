using UnityEditor;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 解像度取得クラス
    /// EditorではScreen.width/heightが取れないのでUnityStats.screenResに振り分ける
    /// </summary>
    public abstract class Resolution
    {
#if UNITY_EDITOR
        //  画面解像度
        public static int Width => int.Parse( UnityStats.screenRes.Split( 'x' )[0] );
        public static int Height => int.Parse( UnityStats.screenRes.Split( 'x' )[1] );
#else
        public static int Width => Screen.width;
        public static int Height => Screen.height;
#endif
    }
}
