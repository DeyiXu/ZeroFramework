// =============================================================
// 功能：缓存
// 作者：kevin
// Email：862860000@qq.com
// 时间：2017-11-20 14:35
// =============================================================
using System;
namespace Zero.Cache
{
    public interface ICache
    {
        string Get(string key);
        string Get(string key, Func<string> acquire, int expireMinutes = 0);
        T Get<T>(string key, Func<T> acquire, int expireMinutes = 0);

        bool Set<T>(string key, Func<T> acquire, int expireMinutes = 0);
        bool Set<T>(string key, T value, int expireMinutes = 0);
        bool Add(string key, string value, int expireMinutes = 0);

        void SetList(string key, params string[] values);
        void SetList(string key, int expireMinutes = 0, params string[] values);
        void AddList(string key, params string[] values);
        string[] GetList(string key);
        string[] GetList(string key, Func<string[]> acquire);
        int[] GetList(string key, Func<int[]> acquire);
        T[] GetList<T>(string key, Func<T[]> acquire);

        CacheHash[] GetHash(string key);
        CacheHash[] GetHash(string key, Func<CacheHash[]> acquire);

        CacheHash GetHash(string key, string hashField);
        void SetHash(string key, string hashField, string value);
        void SetHash(string key, params CacheHash[] cacheHashArray);
        void RemoveHash(string key, string hashField);

        bool Contains(string key);
        long ListLength(string key);
        long HashLength(string key);

        bool Remove(string key);
        void Remove(string[] keys);
        void RemoveAll();
        void RemoveStartWith(string key);
        void RemoveEndWith(string key);
        void RemoveContaions(string key);

        string Dequeue(string key);
        T Dequeue<T>(string key);

        void Enqueue(string key, string value);
        void Enqueue(string key, string[] values);
        void Enqueue<T>(string key, T value);
        TimeSpan? KeyTimeToLive(string key);
        string[] GetKeys();
        string[] GetKeysStartWith(string key);
        string[] GetKeysEndWith(string key);
        string[] GetKeysContains(string key);
    }
}
