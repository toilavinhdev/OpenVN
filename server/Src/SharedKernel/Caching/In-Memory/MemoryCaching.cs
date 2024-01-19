using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SharedKernel.Caching
{
    public class MemoryCaching : IMemoryCaching
    {
        public TimeSpan? DefaultAbsoluteExpireTime => TimeSpan.FromHours(2);

        public TimeSpan? DefaultSlidingExpireTime => null;

        public object Get(string key)
        {
            return MemoryCachingHelper.Get(key);
        }

        public Task<object> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get(key));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var result = await GetAsync(key, cancellationToken);
            if (result == null)
            {
                return JsonConvert.DeserializeObject<T>("");
            }
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(result));
        }

        public string GetString(string key)
        {
            return MemoryCachingHelper.GetString(key);
        }

        public Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetString(key));
        }

        public void Remove(string key)
        {
            MemoryCachingHelper.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var time = absoluteExpireTime ?? DefaultAbsoluteExpireTime;
            MemoryCachingHelper.Set(key, value, time);
        }

        public Task SetAsync(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default)
        {
            Set(key, value, absoluteExpireTime, slidingExpireTime);
            return Task.CompletedTask;
        }
    }
}
