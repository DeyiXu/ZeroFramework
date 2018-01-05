// =============================================================
// 功能：CacheHash
// 作者：kevin
// Email：862860000@qq.com
// 时间：2017-11-20 14:36
// =============================================================
namespace Zero.Cache
{
    public class CacheHash
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
    public static class CacheHashExtension
    {
        public static bool IsNull(this CacheHash cache)
        {
            if (cache == null || cache.Equals(new CacheHash()) || cache == new CacheHash())
            {
                return true;
            }
            return false;
        }
    }
}
