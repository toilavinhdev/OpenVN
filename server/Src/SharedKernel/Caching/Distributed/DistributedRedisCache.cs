using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SharedKernel.Caching
{
    public class DistributedRedisCache : IDistributedRedisCache
    {
        private readonly IDistributedCache _cacheInstance;

        public TimeSpan? DefaultAbsoluteExpireTime => TimeSpan.FromHours(2);

        public TimeSpan? DefaultSlidingExpireTime => null;

        public DistributedRedisCache(IDistributedCache cache)
        {
            _cacheInstance = cache;
        }

        public IDistributedCache GetOriginalCaching()
        {
            return _cacheInstance;
        }

        public object Get(string key)
        {
            return this.GetAsync(key).GetAwaiter().GetResult();
        }

        public async Task<object> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var jsonData = await _cacheInstance.GetStringAsync(key, cancellationToken);
            if (jsonData is null)
            {
                return null;
            }
            else if (jsonData is string)
            {
                return jsonData;
            }

            return JsonConvert.DeserializeObject(jsonData);
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var jsonData = await _cacheInstance.GetStringAsync(key, cancellationToken);
            if (jsonData is null)
            {
                return JsonConvert.DeserializeObject<T>("");
            }

            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public string GetString(string key)
        {
            return _cacheInstance.GetString(key);
        }

        public async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _cacheInstance.GetStringAsync(key, cancellationToken) ?? string.Empty;
        }

        public void Set(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var jsonData = "";
            if (value.GetType() == typeof(string))
            {
                jsonData = value.ToString();
            }
            else
            {
                jsonData = JsonConvert.SerializeObject(value);
            }
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? DefaultAbsoluteExpireTime,
                SlidingExpiration = slidingExpireTime ?? DefaultSlidingExpireTime
            };

            _cacheInstance.SetString(key, jsonData, options);
        }

        public async Task SetAsync(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default)
        {
            var jsonData = string.Empty;
            if (value?.GetType() == typeof(string))
            {
                jsonData = value.ToString();
            }
            else
            {
                jsonData = JsonConvert.SerializeObject(value);
            }
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? DefaultAbsoluteExpireTime,
                SlidingExpiration = slidingExpireTime ?? DefaultSlidingExpireTime
            };

            await _cacheInstance.SetStringAsync(key, jsonData, options, cancellationToken);
        }

        public void Remove(string key)
        {
            _cacheInstance.Remove(key);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cacheInstance.RemoveAsync(key, cancellationToken);
        }
    }
}
