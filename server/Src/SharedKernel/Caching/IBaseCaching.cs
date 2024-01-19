namespace SharedKernel.Caching
{
    public interface IBaseCaching
    {
        TimeSpan? DefaultAbsoluteExpireTime { get; }

        TimeSpan? DefaultSlidingExpireTime { get; }

        object Get(string key);

        Task<object> GetAsync(string key, CancellationToken cancellationToken = default);

        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        string GetString(string key);

        Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default);

        void Set(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);

        Task SetAsync(string key, object value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default);

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
