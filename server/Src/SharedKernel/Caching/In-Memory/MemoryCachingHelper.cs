using Microsoft.Extensions.Caching.Memory;
using System;

namespace SharedKernel.Caching
{
    public static class MemoryCachingHelper
    {
        private static MemoryCache _cacheInstance = new MemoryCache(new MemoryCacheOptions());

        public static object Get(string key)
        {
            return _cacheInstance.Get(key);
        }

        public static string GetString(string key)
        {
            var result = Get(key);
            return result != null ? result.ToString() : string.Empty;
        }

        public static void Set(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var time = absoluteExpireTime ?? TimeSpan.FromMinutes(5);
            _cacheInstance.Set(key, value, time);
        }

        public static void Remove(string key)
        {
            _cacheInstance.Remove(key);
        }
    }
}
