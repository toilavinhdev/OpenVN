namespace SharedKernel.Caching
{
    public interface ISequenceCaching
    {
        TimeSpan DefaultSlidingExpireTime { get; }

        TimeSpan DefaultAbsoluteExpireTime { get; }

        Task<object> GetAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default);

        Task<T> GetAsync<T>(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default);

        Task<string> GetStringAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default);

        Task SetAsync(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CachingType onlyUseType = CachingType.Couple, CancellationToken cancellationToken = default);

        Task RemoveAsync(string key, CachingType type = CachingType.Couple, CancellationToken cancellationToken = default);
    }
}
