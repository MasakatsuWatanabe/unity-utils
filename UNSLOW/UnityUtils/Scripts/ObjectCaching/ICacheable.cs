namespace UNSLOW.UnityUtils.ObjectCaching
{
    /// <summary>
    /// キャッシュ可能なオブジェクトのインターフェース
    /// </summary>
    public interface ICacheable
    {
        public CacheReturner Returner { get; }
    }
}
