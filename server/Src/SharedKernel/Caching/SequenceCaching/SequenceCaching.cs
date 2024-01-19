namespace SharedKernel.Caching
{
    public class SequenceCaching : ISequenceCaching
    {
        private readonly IMemoryCaching _memCaching;
        private readonly IDistributedRedisCache _disCaching;

        public SequenceCaching(IMemoryCaching memCaching, IDistributedRedisCache disCaching)
        {
            _memCaching = memCaching;
            _disCaching = disCaching;
        }

        public TimeSpan DefaultSlidingExpireTime => TimeSpan.FromHours(3);

        public TimeSpan DefaultAbsoluteExpireTime => TimeSpan.FromHours(3);

        public async Task<object> GetAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default)
        {
            switch (type)
            {
                case CachingType.Couple:
                    var result = await _memCaching.GetAsync(key, cancellationToken);
                    if (result == null)
                    {
                        result = await _disCaching.GetAsync(key, cancellationToken);
                        if (result != null)
                        {
                            await _memCaching.SetAsync(key, result, DefaultAbsoluteExpireTime, cancellationToken: cancellationToken);
                        }
                    }
                    return result;
                case CachingType.Memory:
                    return await _memCaching.GetAsync(key, cancellationToken);
                case CachingType.Distributed:
                    return await _disCaching.GetAsync(key, cancellationToken);
            }
            throw new Exception("The caching type is invalid. Please re-check!!!");
        }

        public async Task<T> GetAsync<T>(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default)
        {
            switch (type)
            {
                case CachingType.Couple:
                    var result = await _memCaching.GetAsync<T>(key, cancellationToken);
                    if (result == null)
                    {
                        result = await _disCaching.GetAsync<T>(key, cancellationToken);
                        if (result != null)
                        {
                            await _memCaching.SetAsync(key, result, DefaultAbsoluteExpireTime, cancellationToken: cancellationToken);
                        }
                    }
                    return result;
                case CachingType.Memory:
                    return await _memCaching.GetAsync<T>(key, cancellationToken);
                case CachingType.Distributed:
                    return await _disCaching.GetAsync<T>(key, cancellationToken);
            }
            throw new Exception("The caching type is invalid. Please re-check!!!");
        }

        public async Task<string> GetStringAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default)
        {
            switch (type)
            {
                case CachingType.Couple:
                    var result = await _memCaching.GetStringAsync(key, cancellationToken);
                    if (string.IsNullOrEmpty(result))
                    {
                        result = await _disCaching.GetStringAsync(key, cancellationToken);
                        if (!string.IsNullOrEmpty(result))
                        {
                            await _memCaching.SetAsync(key, result, DefaultAbsoluteExpireTime, cancellationToken: cancellationToken);
                        }
                    }
                    return result;
                case CachingType.Memory:
                    return await _memCaching.GetStringAsync(key, cancellationToken);
                case CachingType.Distributed:
                    return await _disCaching.GetStringAsync(key, cancellationToken);
            }
            throw new Exception("The caching type is invalid. Please re-check!!!");
        }

        public async Task SetAsync(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CachingType onlyUseType = CachingType.Couple, CancellationToken cancellationToken = default)
        {
            switch (onlyUseType)
            {
                case CachingType.Couple:
                    await _memCaching.SetAsync(key, value, absoluteExpireTime, slidingExpireTime, cancellationToken);
                    await _disCaching.SetAsync(key, value, absoluteExpireTime, slidingExpireTime, cancellationToken);
                    return;
                case CachingType.Memory:
                    await _memCaching.SetAsync(key, value, absoluteExpireTime, slidingExpireTime, cancellationToken);
                    return;
                case CachingType.Distributed:
                    await _disCaching.SetAsync(key, value, absoluteExpireTime, slidingExpireTime, cancellationToken);
                    return;
            }
            throw new Exception("The caching type is invalid. Please re-check!!!");
        }

        public async Task RemoveAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default)
        {
            switch (type)
            {
                case CachingType.Couple:
                    await _memCaching.RemoveAsync(key, cancellationToken);
                    await _disCaching.RemoveAsync(key, cancellationToken);
                    return;
                case CachingType.Memory:
                    await _memCaching.RemoveAsync(key, cancellationToken);
                    return;
                case CachingType.Distributed:
                    await _disCaching.RemoveAsync(key, cancellationToken);
                    return;
            }
            throw new Exception("The caching type is invalid. Please re-check!!!");
        }
    }
}
